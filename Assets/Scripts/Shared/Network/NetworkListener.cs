﻿using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using Shared.Logger;

namespace Shared.Network
{
    public class NetworkListener
    {
        public Action<NetworkCommunicator> OnCommunicatorConnected;
        public Action<NetworkCommunicator> OnCommunicatorDisconnected;

        public Action<NetworkCommunicator, ReceivedMessageInfo> OnReceivedMessage;

        public int ConnectionCount => _socketToCommunicators.Count;

        // Variables
        private Socket _listenFd;

        private readonly int _maxConnectionCount;
        private readonly bool _isNeedCheckOverReceived;

        private readonly Queue<NetworkCommunicator> _communicatorsToAdd;
        private readonly Queue<NetworkCommunicator> _communicatorsToClose;

        public Dictionary<Socket, NetworkCommunicator> SocketToCommunicators => _socketToCommunicators;

        private readonly Dictionary<Socket, NetworkCommunicator> _socketToCommunicators;

        private readonly NetworkCommunicatorPool _communicatorPool;

        public NetworkListener(int maxConnectionCount, bool isNeedCheckOverReceived)
        {
            _maxConnectionCount = maxConnectionCount;
            _isNeedCheckOverReceived = isNeedCheckOverReceived;
            ;

            _communicatorsToAdd = new Queue<NetworkCommunicator>();
            _communicatorsToClose = new Queue<NetworkCommunicator>();

            _communicatorPool = new NetworkCommunicatorPool(_maxConnectionCount);
            _socketToCommunicators = new Dictionary<Socket, NetworkCommunicator>();
        }

        public void Listen(string ip, int port)
        {
            var ipAddress = IPAddress.Parse(ip);
            var endPoint = new IPEndPoint(ipAddress, port);

            _listenFd = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            try
            {
                _listenFd.Bind(endPoint);
                _listenFd.Listen(_maxConnectionCount);
                Log.Info($"Start Listening at Port: {port}");

                var acceptEventArg = new SocketAsyncEventArgs(); // 所有Accept共用這個eventArgs
                acceptEventArg.Completed += OnAccept;

                AcceptAsync(acceptEventArg);
            }
            catch (Exception e)
            {
                Log.Error(e.Message);
            }
        }

        #region - Life Cycle -

        // 每個Frame要處理的順序： AddCommunicators -> HandleMessages -> CloseCommunicators

        /// <summary>
        /// 需在Main Thread上執行
        /// </summary>
        public void AddCommunicators()
        {
            if (_communicatorsToAdd.Count <= 0) return;

            lock (_communicatorsToAdd)
            {
                foreach (var communicator in _communicatorsToAdd)
                {
                    _socketToCommunicators.Add(communicator.Socket, communicator);

                    OnCommunicatorConnected?.Invoke(communicator);

                    // 開始接收clientFd傳來的訊息
                    communicator.ReceiveAsync();
                }

                _communicatorsToAdd.Clear();
            }
        }

        /// <summary>
        /// 需在Main Thread上執行
        /// </summary>
        public void CloseCommunicators()
        {
            if (_communicatorsToClose.Count <= 0) return;

            lock (_communicatorsToClose)
            {
                for (int i = 0; i < _communicatorsToClose.Count; i++)
                {
                    var communicator = _communicatorsToClose.Dequeue();
                    if (communicator.IsSending)
                    {
                        _communicatorsToClose.Enqueue(communicator);
                    }
                    else
                    {
                        ReleaseCommunicatorAndCloseSocket(communicator.Socket);
                    }
                }
            }
        }

        /// <summary>
        /// 需在Main Thread上執行
        /// </summary>
        public void HandleMessages()
        {
            foreach (var communicator in SocketToCommunicators.Values)
            {
                communicator.HandleMessages();
            }
        }

        #endregion

        #region - Accept -

        private void AcceptAsync(SocketAsyncEventArgs acceptEventArg)
        {
            try
            {
                if (!_listenFd.AcceptAsync(acceptEventArg))
                {
                    OnAccept(this, acceptEventArg);
                }
            }
            catch (Exception e)
            {
                Log.Error(e.ToString());
            }
        }

        private void OnAccept(object sender, SocketAsyncEventArgs acceptEventArg)
        {
            Socket clientFd = acceptEventArg.AcceptSocket;
            if (clientFd == null)
            {
                acceptEventArg.AcceptSocket = null;
                AcceptAsync(acceptEventArg);
                return;
            }

            //Log.Info($"A Client {clientFd.RemoteEndPoint?.ToString()} Connected!");

            // 加入CommunicatorList列表
            var communicator = _communicatorPool.Rent();

            if (communicator == null)
            {
                // 超過最大連線數
                CloseSocket(clientFd);
            }
            else
            {
                communicator.Init(clientFd, _isNeedCheckOverReceived);
                communicator.OnReceivedMessage += HandleReceivedMessage;
                communicator.OnClosed += HandleCommunicatorClosed;

                lock (_communicatorsToAdd)
                {
                    _communicatorsToAdd.Enqueue(communicator);
                }
            }

            // 重置acceptEventArg，並繼續監聽
            acceptEventArg.AcceptSocket = null;
            AcceptAsync(acceptEventArg);
        }

        #endregion

        #region - NetworkCommunicator Events -

        private void HandleReceivedMessage(NetworkCommunicator communicator, ReceivedMessageInfo receivedMessageInfo)
        {
            OnReceivedMessage?.Invoke(communicator, receivedMessageInfo);
        }

        private void HandleCommunicatorClosed(NetworkCommunicator communicator)
        {
            AddCommunicatorToCloseQueue(communicator);
        }

        #endregion

        #region - Send -

        public void SendAll(ushort messageId, byte[] message)
        {
            lock (_socketToCommunicators)
            {
                using var enumerator = _socketToCommunicators.GetEnumerator();
                while (enumerator.MoveNext())
                {
                    Send(enumerator.Current.Value, messageId, message);
                }
            }
        }

        public void Send(NetworkCommunicator communicator, ushort messageId, byte[] message)
        {
            if (communicator == null)
            {
                Log.Error("Send Failed, client is null or not connected");
                return;
            }

            communicator.Send(messageId, message);
        }

        #endregion

        #region - Close -

        private void AddCommunicatorToCloseQueue(NetworkCommunicator communicator)
        {
            if (communicator == null)
                return;

            lock (_communicatorsToClose)
            {
                _communicatorsToClose.Enqueue(communicator);
            }
        }

        private void ReleaseCommunicatorAndCloseSocket(Socket socket)
        {
            if (socket == null) return;

            if (!_socketToCommunicators.TryGetValue(socket, out var communicator))
            {
                Log.Error($"Close Socket Error: Cannot find communicator");
                return;
            }

            RemoveFromCommunicatorList();
            ReturnCommunicator();
            CloseConnection();
            return;

            void RemoveFromCommunicatorList()
            {
                OnCommunicatorDisconnected?.Invoke(communicator);
                if (_socketToCommunicators.ContainsKey(socket)) 
                    _socketToCommunicators.Remove(socket);
            }

            void ReturnCommunicator()
            {
                communicator.OnReceivedMessage -= HandleReceivedMessage;
                communicator.OnClosed -= HandleCommunicatorClosed;
                _communicatorPool.Return(communicator);
            }

            void CloseConnection()
            {
                CloseSocket(socket);
            }
        }

        private void CloseSocket(Socket socket)
        {
            try
            {
                socket.Shutdown(SocketShutdown.Send);
            }
            catch (Exception e)
            {
                Log.Error(e.ToString());
            }

            socket.Close();
        }

        #endregion
    }
}
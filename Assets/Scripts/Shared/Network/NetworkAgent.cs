﻿using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Threading.Tasks;
using Shared.Common;
using Shared.Logger;

namespace Shared.Network
{
    public class RequestInfo : IPoolable
    {
        public ReceivedMessageInfo ReceivedMessageInfo;
        public uint MessageId;
        public ushort RequestId;
        public Action<ReceivedMessageInfo> OnCompleted;
        public Action OnTimeOut;
        public long RequestTime;
        public bool IsCompleted;

        public void Reset()
        {
            ReceivedMessageInfo = default;
            MessageId = 0;
            RequestId = 0;
            OnCompleted = null;
            OnTimeOut = null;
            RequestTime = 0;
            IsCompleted = false;
        }
    }

    /// <summary>
    /// 實作發Request, 斷線重連
    /// </summary>
    public class NetworkAgent
    {
        public Action OnConnected;
        public Action OnDisconnected;

        public ConnectState ConnectState => _connector.ConnectState;

        private static readonly long REQUEST_TIME_OUT_MILLISECONDS = 10 * 1000;
        private static readonly long CHECK_REQUEST_TIME_OUT_MILLISECONDS = 1 * 1000;

        private MessageRouter<NetworkCommunicator> _messageRouter;
        private NetworkConnector _connector;

        private LinkedList<RequestInfo> _requestPacks;
        private ConcurrentQueue<RequestInfo> _responseQueue;
        private Queue<RequestInfo> _timeOutRequests;

        private ConcurrentPool<RequestInfo> _requestPool;

        private ushort _requestSerialId = 0;

        private long _lastCheckRequestTimeOutTime;

        private const int _checkReconnectIntervalMs = 3_000;

        private long _lastCheckReconnectTime;
        private string _cacheIp;
        private int _cachePort;

        public NetworkAgent()
        {
            _messageRouter = new MessageRouter<NetworkCommunicator>();
            _requestPacks = new LinkedList<RequestInfo>();
            _responseQueue = new ConcurrentQueue<RequestInfo>();
            _timeOutRequests = new Queue<RequestInfo>();

            _requestPool = new ConcurrentPool<RequestInfo>();
        }

        public void Update()
        {
            _connector.Update();

            if (_responseQueue.TryDequeue(out var requestPack))
            {
                requestPack.OnCompleted?.Invoke(requestPack.ReceivedMessageInfo);
                requestPack.IsCompleted = true;
                requestPack.ReceivedMessageInfo.Release();
            }

            CheckRequestTimeOut();
            CheckReconnect();
        }

        private void CheckRequestTimeOut()
        {
            if (TimeUtils.TimeSinceAppStart - _lastCheckRequestTimeOutTime <
                CHECK_REQUEST_TIME_OUT_MILLISECONDS) return;

            _lastCheckRequestTimeOutTime = TimeUtils.TimeSinceAppStart;

            _timeOutRequests.Clear();

            lock (_requestPacks)
            {
                var enumerator = _requestPacks.GetEnumerator();
                while (enumerator.MoveNext())
                {
                    var current = enumerator.Current;

                    if (TimeUtils.TimeSinceAppStart - current.RequestTime >= REQUEST_TIME_OUT_MILLISECONDS)
                    {
                        _timeOutRequests.Enqueue(current);
                    }
                }

                foreach (var request in _timeOutRequests)
                {
                    _requestPacks.Remove(request);
                }
            }

            foreach (var request in _timeOutRequests)
            {
                if (!request.IsCompleted)
                {
                    request.OnTimeOut?.Invoke();
                }

                _requestPool.Return(request);
            }
        }

        private void CheckReconnect()
        {
            if (TimeUtils.GetTimeStamp() - _lastCheckReconnectTime < _checkReconnectIntervalMs) return;

            _lastCheckReconnectTime = TimeUtils.GetTimeStamp();
            Reconnect().Await();
        }

        private async Task Reconnect()
        {
            if (_connector.ConnectState != ConnectState.Disconnected) return;

            await Connect(_cacheIp, _cachePort);
        }

        public async Task Connect(string ip, int port)
        {
            _cacheIp = ip;
            _cachePort = port;
            _lastCheckReconnectTime = TimeUtils.GetTimeStamp();

            _connector = new NetworkConnector();
            _connector.OnReceivedMessage += HandleReceivedMessage;
            _connector.OnConnected += HandleConnected;
            _connector.OnClosed += HandleClosed;

            await _connector.Connect(ip, port);
        }

        private void HandleReceivedMessage(NetworkCommunicator communicator, ReceivedMessageInfo receivedMessageInfo)
        {
            if (receivedMessageInfo.IsRequest)
            {
                lock (_requestPacks)
                {
                    if (TryGetRequestInfo(receivedMessageInfo.RequestId, out var requestPack))
                    {
                        requestPack.ReceivedMessageInfo = receivedMessageInfo;
                        _responseQueue.Enqueue(requestPack);
                    }
                }
            }
            else
            {
                _messageRouter.ReceiveMessage(communicator, receivedMessageInfo);
            }
        }

        private void HandleConnected(NetworkCommunicator communicator)
        {
            OnConnected?.Invoke();
        }

        private void HandleClosed(Socket socket)
        {
            OnDisconnected?.Invoke();

            _requestPacks.Clear();
            _responseQueue.Clear();
            _timeOutRequests.Clear();

            _connector.OnReceivedMessage -= HandleReceivedMessage;
            _connector.OnConnected -= HandleConnected;
            _connector.OnClosed -= HandleClosed;

            _lastCheckReconnectTime = TimeUtils.GetTimeStamp();
        }

        private bool TryGetRequestInfo(uint requestId, out RequestInfo outRequestInfo)
        {
            var enumerator = _requestPacks.GetEnumerator();
            while (enumerator.MoveNext())
            {
                var current = enumerator.Current;
                if (current.RequestId != requestId) continue;

                outRequestInfo = current;
                return true;
            }

            outRequestInfo = default;
            return false;
        }

        #region - Public Methods -

        public void RegisterMessageHandler(ushort messageId, Action<NetworkCommunicator, ByteBuffer> handler)
        {
            _messageRouter.RegisterMessageHandler(messageId,
                (communicator, receivedMessageInfo) => { handler(communicator, receivedMessageInfo.Message); });
        }

        public void SendMessage(ushort messageId, byte[] message)
        {
            if (_connector.ConnectState != ConnectState.Connected) return;
            _connector.Send(messageId, message);
        }

        public void SendMessage(ushort messageId, ByteBuffer message)
        {
            if (_connector.ConnectState != ConnectState.Connected) return;
            _connector.Send(messageId, message);
        }

        public Task<ReceivedMessageInfo> SendRequest(ushort messageId, byte[] request, Action onTimeOut = null)
        {
            var taskCompletionSource =
                new TaskCompletionSource<ReceivedMessageInfo>(TaskCreationOptions.RunContinuationsAsynchronously);
            try
            {
                SendRequest(messageId, request,
                    (info) =>
                    {
                        try
                        {
                            taskCompletionSource.SetResult(info);
                        }
                        catch (Exception e)
                        {
                            Log.Error(e.ToString());
                        }
                    },
                    onTimeOut);
            }
            catch (Exception ex)
            {
                taskCompletionSource.SetException(ex);
            }

            return taskCompletionSource.Task;
        }

        public Task<ReceivedMessageInfo> SendRequest(ushort messageId, ByteBuffer request, Action onTimeOut = null)
        {
            var taskCompletionSource =
                new TaskCompletionSource<ReceivedMessageInfo>(TaskCreationOptions.RunContinuationsAsynchronously);
            try
            {
                SendRequest(messageId, request,
                    (info) =>
                    {
                        try
                        {
                            taskCompletionSource.SetResult(info);
                        }
                        catch (Exception e)
                        {
                            Log.Error(e.ToString());
                        }
                    },
                    onTimeOut);
            }
            catch (Exception ex)
            {
                taskCompletionSource.SetException(ex);
            }

            return taskCompletionSource.Task;
        }

        public void SendRequest(ushort messageId, byte[] request, Action<ReceivedMessageInfo> onCompleted,
            Action onTimeOut = null)
        {
            if (_connector.ConnectState != ConnectState.Connected) return;

            AddRequest(messageId, onCompleted, onTimeOut);

            _connector.Send(messageId, request, true, _requestSerialId);
        }

        public void SendRequest(ushort messageId, ByteBuffer request, Action<ReceivedMessageInfo> onCompleted,
            Action onTimeOut = null)
        {
            if (_connector.ConnectState != ConnectState.Connected) return;

            AddRequest(messageId, onCompleted, onTimeOut);

            _connector.Send(messageId, request, true, _requestSerialId);
        }

        private void AddRequest(ushort messageId, Action<ReceivedMessageInfo> onCompleted, Action onTimeOut)
        {
            lock (this)
            {
                ++_requestSerialId;
            }

            var requestPack = _requestPool.Rent();
            requestPack.MessageId = messageId;
            requestPack.RequestId = _requestSerialId;
            requestPack.OnCompleted = onCompleted;
            requestPack.OnTimeOut = onTimeOut;
            requestPack.RequestTime = TimeUtils.TimeSinceAppStart;

            lock (_requestPacks)
            {
                _requestPacks.AddLast(requestPack);
            }
        }

        public void Close()
        {
            _connector.Close();
        }

        #endregion
    }
}
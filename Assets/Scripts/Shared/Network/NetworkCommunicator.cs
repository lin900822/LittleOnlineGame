﻿using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Net.Sockets;
using Shared.Logger;
using Shared.Metrics;

namespace Shared.Network
{
    public class NetworkCommunicator
    {
        private bool _isNeedCheckOverReceived = false;

        private bool _isClosing = false;

        public Socket Socket { get; private set; }

        public Action<NetworkCommunicator, ReceivedMessageInfo> OnReceivedMessage;
        public Action<NetworkCommunicator> OnClose;

        private readonly ByteBuffer _receiveBuffer;
        private readonly Queue<ByteBuffer> _sendQueue;

        private readonly ConcurrentQueue<ReceivedMessageInfo> _receivedMessageInfos;

        private readonly SocketAsyncEventArgs _receiveArgs;
        private readonly SocketAsyncEventArgs _sendArgs;

        // Const
        private const int MaxReceivedCount = 300;

        private const int ShortPacketLength = 2;
        private const int LongPacketLength = 4;
        private const int MessageIdLength = 2;
        private const int RequestIdLength = 2;

        private const int WarningPacketSize = 1024 * 4;
        private const int MaxPacketSize = (int)(uint.MaxValue >> 2);

        private const uint LongPacketFlag = 0b_00000000_00000000_10000000_00000000;
        private const uint RequestFlag = 0b_00000000_00000000_01000000_00000000;

        public NetworkCommunicator(int bufferSize)
        {
            var receiveArg = new SocketAsyncEventArgs();
            var sendArg = new SocketAsyncEventArgs();
            receiveArg.SetBuffer(new byte[bufferSize], 0, bufferSize);
            sendArg.SetBuffer(new byte[bufferSize], 0, bufferSize);

            _receiveBuffer = new ByteBuffer(bufferSize);
            _sendQueue = new Queue<ByteBuffer>();

            _receivedMessageInfos = new ConcurrentQueue<ReceivedMessageInfo>();

            _receiveArgs = receiveArg;
            _sendArgs = sendArg;
        }

        public virtual void Init(Socket socket, bool isNeedCheckOverReceived = false)
        {
            Socket = socket;
            _receiveArgs.AcceptSocket = socket;
            _sendArgs.AcceptSocket = socket;

            _isNeedCheckOverReceived = isNeedCheckOverReceived;

            _receiveArgs.Completed += OnReceive;
            _sendArgs.Completed += OnSend;
        }

        public virtual void Release()
        {
            Socket = null;
            _receiveArgs.AcceptSocket = null;
            _sendArgs.AcceptSocket = null;

            _isNeedCheckOverReceived = false;

            _isClosing = false;

            lock (_receiveBuffer)
            {
                _receiveBuffer.SetReadIndex(0);
                _receiveBuffer.SetWriteIndex(0);
            }

            lock (_sendQueue)
            {
                var sendQueueCount = _sendQueue.Count;
                for (int i = 0; i < sendQueueCount; i++)
                {
                    var item = _sendQueue.Dequeue();
                    ByteBufferPool.Shared.Return(item);
                }
            }

            _receivedMessageInfos.Clear();

            _receiveArgs.Completed -= OnReceive;
            _sendArgs.Completed -= OnSend;
        }

        public void HandleMessages()
        {
            for (var i = 0; i < 10; i++)
            {
                if (_receivedMessageInfos.Count <= 0)
                {
                    return;
                }

                if (!_receivedMessageInfos.TryDequeue(out var messageInfo))
                {
                    return;
                }

                // 分發收到的 Message
                OnReceivedMessage?.Invoke(this, messageInfo);
            }

            SystemMetrics.RemainMessageCount += _receivedMessageInfos.Count;
        }

        #region - Receive -

        public void ReceiveAsync()
        {
            try
            {
                if (!IsSocketValid())
                {
                    return;
                }

                if (!Socket.ReceiveAsync(_receiveArgs))
                {
                    OnReceive(this, _receiveArgs);
                }
            }
            catch (Exception e)
            {
                Log.Error(e.ToString());
            }
        }

        private void OnReceive(object sender, SocketAsyncEventArgs args)
        {
            if (!IsSocketValid() || !ReadDataToReceiveBuffer(args))
            {
                // 收到 0個 Byte代表 Client已關閉
                Close();
                return;
            }

            ParseReceivedData();
            ReceiveAsync();
        }

        private bool ReadDataToReceiveBuffer(SocketAsyncEventArgs args)
        {
            var receiveCount = args.BytesTransferred;
            var isNotSuccess = args.SocketError != SocketError.Success;

            if (receiveCount <= 0 || isNotSuccess)
            {
                return false;
            }

            _receiveBuffer.Write(args.Buffer, args.Offset, receiveCount);

            return true;
        }

        private void ParseReceivedData()
        {
            while (_receiveBuffer.Length > 2)
            {
                if (!TryUnpackMessage(out var messageInfo))
                {
                    return;
                }

                _receivedMessageInfos.Enqueue(messageInfo);

                if (IsOverReceived())
                {
                    Log.Warn($"{Socket.RemoteEndPoint.ToString()} Sent Too Much Packets.");
                    Close();
                    return;
                }
            }
        }

        private bool IsOverReceived()
        {
            if (!_isNeedCheckOverReceived)
            {
                return false;
            }

            return _receivedMessageInfos.Count >= MaxReceivedCount;
        }

        #endregion

        #region - Send -

        public void Send(ushort messageId, byte[] message, bool isRequest = false, ushort requestId = 0)
        {
            if (!IsSocketValid())
            {
                return;
            }

            // 打包資料
            var packetLength = (message.Length > short.MaxValue) ? ShortPacketLength : LongPacketLength;
            var byteBuffer = ByteBufferPool.Shared.Rent(packetLength + MessageIdLength + message.Length);
            PackMessage(byteBuffer, messageId, message, message.Length, 0, isRequest, requestId);

            AddMessageToSendQueue(byteBuffer);
        }

        public void Send(
            ushort messageId,
            ByteBuffer message,
            bool isRequest = false,
            ushort requestId = 0)
        {
            if (!IsSocketValid())
            {
                return;
            }

            // 打包資料
            var packetLength = (message.Length > short.MaxValue) ? ShortPacketLength : LongPacketLength;
            var byteBuffer = ByteBufferPool.Shared.Rent(packetLength + MessageIdLength + message.Length);
            PackMessage(byteBuffer, messageId, message.RawData, message.Length, message.ReadIndex, isRequest,
                requestId);

            AddMessageToSendQueue(byteBuffer);
        }

        private void AddMessageToSendQueue(ByteBuffer byteBuffer)
        {
            // 透過 SendQueue處理發送不完整問題
            int count = 0;
            lock (_sendQueue)
            {
                _sendQueue.Enqueue(byteBuffer);
                count = _sendQueue.Count;
            }

            // 當 SendQueue只有 1個時發送
            // SendQueue.Count > 1時, 在 OnSend()裡面會持續發送, 直到發送完
            if (count == 1)
            {
                var copyCount = Math.Min(byteBuffer.Length, NetworkConfig.BufferSize);
                _sendArgs.SetBuffer(_sendArgs.Offset, copyCount);
                Array.Copy(byteBuffer.RawData, byteBuffer.ReadIndex, _sendArgs.Buffer, _sendArgs.Offset, copyCount);
                SendAsync();
            }
        }

        private void SendAsync()
        {
            try
            {
                if (!IsSocketValid())
                {
                    return;
                }

                if (!Socket.SendAsync(_sendArgs))
                {
                    OnSend(this, _sendArgs);
                }
            }
            catch (Exception e)
            {
                Log.Error(e.ToString());
            }
        }

        private void OnSend(object sender, SocketAsyncEventArgs args)
        {
            if (!IsSocketValid())
            {
                return;
            }

            if (args.SocketError != SocketError.Success)
            {
                return;
            }

            CheckSendQueue();
        }

        private void CheckSendQueue()
        {
            if (_sendQueue.Count <= 0)
            {
                return;
            }

            var count = _sendArgs.BytesTransferred;

            ByteBuffer byteBuffer;
            lock (_sendQueue)
            {
                byteBuffer = _sendQueue.Peek();
            }

            byteBuffer.SetReadIndex(byteBuffer.ReadIndex + count);

            // 完整發送完一個ByteBuffer的資料
            if (byteBuffer.Length <= 0)
            {
                ByteBuffer dequeueBuffer;
                lock (_sendQueue)
                {
                    dequeueBuffer = _sendQueue.Dequeue();
                    if (_sendQueue.Count >= 1)
                    {
                        byteBuffer = _sendQueue.Peek();
                    }
                    else
                    {
                        byteBuffer = null;
                    }
                }

                ByteBufferPool.Shared.Return(dequeueBuffer);
            }

            if (byteBuffer != null)
            {
                // SendQueue還有資料，繼續發送
                var copyCount = Math.Min(byteBuffer.Length, NetworkConfig.BufferSize);
                _sendArgs.SetBuffer(_sendArgs.Offset, copyCount);
                Array.Copy(byteBuffer.RawData, byteBuffer.ReadIndex, _sendArgs.Buffer, _sendArgs.Offset, copyCount);
                SendAsync();
            }
        }

        #endregion

        private bool IsSocketValid()
        {
            if (_isClosing)
            {
                return false;
            }

            if (Socket == null)
            {
                return false;
            }

            return Socket.Connected;
        }

        #region - Close -

        private void Close()
        {
            OnClose?.Invoke(this);
            _isClosing = true;
        }

        #endregion

        #region - Handle Packet -

        /// <summary>
        /// 短封包:
        /// | 總長度 2 Byte | MessageId 4 Byte | 資料內容 x Byte |
        /// 
        /// 長封包:
        /// | 總長度 4 Byte | MessageId 4 Byte | 資料內容 x Byte |
        /// 
        /// Request:
        /// | 總長度 4 Byte | MessageId 4 Byte | RequestId 2 Byte | 資料內容 x Byte |
        /// </summary>
        private bool TryUnpackMessage(out ReceivedMessageInfo receivedMessageInfo)
        {
            receivedMessageInfo = new ReceivedMessageInfo();

            // 連表示總長度的 2 Byte都沒收到
            if (_receiveBuffer.Length < ShortPacketLength) return false;

            // 檢查是否是長封包
            var isLongPacket = false;
            var isRequest = false;
            var totalLength = (int)_receiveBuffer.CheckUInt16();
            if (HasLongPacketFlag(totalLength))
            {
                if (_receiveBuffer.Length < LongPacketLength) return false;

                isLongPacket = true;
                isRequest = HasRequestFlag(totalLength);

                totalLength = (int)(totalLength & ~LongPacketFlag);
                totalLength = (int)(totalLength & ~RequestFlag);
                totalLength = (totalLength << 16) | _receiveBuffer.CheckUInt16(2);
            }

            // 資料不完整
            if (_receiveBuffer.Length < totalLength) return false;

            // 資料完整，開始解析
            if (isLongPacket)
            {
                totalLength = (int)(_receiveBuffer.ReadUInt16() & ~LongPacketFlag);
                totalLength = (int)(totalLength & ~RequestFlag);
                totalLength = (totalLength << 16) | _receiveBuffer.ReadUInt16();
            }
            else
            {
                totalLength = _receiveBuffer.ReadUInt16();
            }

            var bodyLength = totalLength - (isLongPacket ? LongPacketLength : ShortPacketLength) - MessageIdLength;

            if (isRequest)
            {
                receivedMessageInfo.IsRequest = true;
                receivedMessageInfo.RequestId = _receiveBuffer.ReadUInt16();

                bodyLength -= RequestIdLength;
            }

            receivedMessageInfo.MessageId = _receiveBuffer.ReadUInt16();
            receivedMessageInfo.Allocate(bodyLength);
            _receiveBuffer.Read(receivedMessageInfo.Message, bodyLength);

            return true;
        }

        /// <summary>
        /// 10000000 00000000 => 定義第一個Bit如果是1的話代表長封包, 0代表短封包
        /// </summary>
        private static bool HasLongPacketFlag(int value) => value > short.MaxValue;

        /// <summary>
        /// 01000000 00000000 => 定義第二個Bit如果是1的話代表是Request, 0代表普通的Message
        /// </summary>
        private static bool HasRequestFlag(int value) => (value & RequestFlag) > 0;

        private static void PackMessage(
            ByteBuffer byteBuffer,
            ushort messageId,
            byte[] message,
            int bodyLength,
            int offset = 0,
            bool isRequest = false,
            ushort requestId = 0)
        {
            if (bodyLength >= MaxPacketSize)
                throw new Exception($"MessageId({messageId}) length({bodyLength}) is over size.");
            if (bodyLength >= WarningPacketSize)
                Log.Warn($"MessageId({messageId}) length({bodyLength}) is too big.");

            int totalLength;

            if (bodyLength > short.MaxValue || isRequest)
            {
                ushort upperTwoByte;
                ushort lowerTwoByte;

                if (isRequest)
                {
                    totalLength = LongPacketLength + MessageIdLength + RequestIdLength + bodyLength;

                    upperTwoByte = (ushort)((totalLength >> 16) | LongPacketFlag);
                    upperTwoByte = (ushort)(upperTwoByte | RequestFlag);
                    lowerTwoByte = (ushort)totalLength;

                    byteBuffer.WriteUInt16(upperTwoByte);
                    byteBuffer.WriteUInt16(lowerTwoByte);
                    byteBuffer.WriteUInt16(requestId);
                }
                else
                {
                    totalLength = LongPacketLength + MessageIdLength + bodyLength;

                    upperTwoByte = (ushort)((totalLength >> 16) | LongPacketFlag);
                    lowerTwoByte = (ushort)totalLength;

                    byteBuffer.WriteUInt16(upperTwoByte);
                    byteBuffer.WriteUInt16(lowerTwoByte);
                }
            }
            else
            {
                totalLength = ShortPacketLength + MessageIdLength + bodyLength;
                byteBuffer.WriteUInt16((ushort)totalLength);
            }

            byteBuffer.WriteUInt16(messageId);
            byteBuffer.Write(message, offset, bodyLength);
        }

        #endregion
    }
}
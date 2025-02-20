﻿using System;
using System.Collections.Generic;
using Shared.Logger;
using Shared.Metrics;

namespace Shared.Network
{
    public class MessageRouter<T>
    {
        private Dictionary<uint, Action<T, ReceivedMessageInfo>> _routeTable;

        public MessageRouter()
        {
            _routeTable = new Dictionary<uint, Action<T, ReceivedMessageInfo>>();
        }

        public void RegisterMessageHandler(ushort messageId, Action<T, ReceivedMessageInfo> handler)
        {
            if (_routeTable.TryGetValue(messageId, out var handlers))
            {
                handlers += handler;
            }
            else
            {
                _routeTable[messageId] = handler;
            }
        }

        public void ReceiveMessage(T obj, ReceivedMessageInfo receivedMessageInfo)
        {
            if (_routeTable.TryGetValue(receivedMessageInfo.MessageId, out var handler))
            {
                handler?.Invoke(obj, receivedMessageInfo);
            }
            else
            {
                Log.Warn($"Message Router: Received Unregistered Message, messageId = {receivedMessageInfo.MessageId}");
            }

            receivedMessageInfo.Release();
            SystemMetrics.HandledMessageCount++;
        }
    }
}
﻿using System;
using Google.Protobuf;

namespace Shared.Network
{
    public static class ProtoUtils
    {
        public static byte[] Encode(IMessage message)
        {
            return message.ToByteArray();
        }

        public static bool TryDecode<T>(byte[] message, out T outMessage) where T : IMessage, new()
        {
            try
            {
                outMessage = new T();
                outMessage.MergeFrom(message);
                return true;
            }
            catch (Exception e)
            {
                Logger.Log.Error(e.ToString());
                outMessage = default(T);
                return false;
            }
        }
    }
}
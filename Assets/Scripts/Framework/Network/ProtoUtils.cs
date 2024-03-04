﻿using System;
using Common;
using Google.Protobuf;
using UnityEngine;

namespace Core.Network
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
                Log.Error(e.ToString());
                outMessage = default(T);
                return false;
            }
        }
    }
}
﻿using System.Collections.Concurrent;

namespace Shared.Network
{
    public class ByteBufferPool
    {
        private static ByteBufferPool _shared;

        public static ByteBufferPool Shared
        {
            get
            {
                if (_shared == null)
                {
                    _shared = new ByteBufferPool();
                }

                return _shared;
            }
        }

        private ConcurrentQueue<ByteBuffer> _byteBufferQueue = new ConcurrentQueue<ByteBuffer>();

        public ByteBufferPool()
        {
        }

        public ByteBuffer Rent(int size = 10)
        {
            if (!_byteBufferQueue.TryDequeue(out var byteBuffer))
                return new ByteBuffer(size);

            if (byteBuffer.Capacity < size)
            {
                byteBuffer.Resize(size);
            }

            byteBuffer.SetReadIndex(0);
            byteBuffer.SetWriteIndex(0);
            byteBuffer.IsInPool = false;
            return byteBuffer;
        }

        public void Return(ByteBuffer byteBuffer)
        {
            if (byteBuffer.IsInPool)
            {
                return;
            }

            _byteBufferQueue.Enqueue(byteBuffer);
            byteBuffer.IsInPool = true;
        }
    }
}
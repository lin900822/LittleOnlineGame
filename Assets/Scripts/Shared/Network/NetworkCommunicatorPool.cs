using System.Collections.Concurrent;

namespace Shared.Network
{
    public class NetworkCommunicatorPool
    {
        private ConcurrentQueue<NetworkCommunicator> _communicatorQueue;

        public NetworkCommunicatorPool(int maxConnectionCount)
        {
            var bufferSize = NetworkConfig.BufferSize;

            _communicatorQueue = new ConcurrentQueue<NetworkCommunicator>();

            for (int i = 0; i < maxConnectionCount; i++)
            {
                var communicator = new NetworkCommunicator(bufferSize);

                _communicatorQueue.Enqueue(communicator);
            }
        }

        public NetworkCommunicator Rent()
        {
            if (_communicatorQueue.TryDequeue(out var communicator)) return communicator;

            return null;
        }

        public void Return(NetworkCommunicator communicator)
        {
            communicator.Release();
            _communicatorQueue.Enqueue(communicator);
        }
    }
}
using System;
using Framework.UI;
using Game.UI;
using Shared;
using Shared.Common;
using Shared.Logger;
using Shared.Network;

namespace Game
{
    public class NetworkSystem
    {
        private NetworkAgent _networkAgent;
        
        private static NetworkSystem _instance;

        public Action OnConnected;

        public static NetworkSystem Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new NetworkSystem();
                }

                return _instance;
            }
        }

        private NetworkSystem()
        {
            
        }

        public void Init()
        {
            _networkAgent = new NetworkAgent();

            _networkAgent.Connect("127.0.0.1", 50001).Await(OnConnected);
        }

        public void Update()
        {
            _networkAgent.Update();
        }

        public void RegisterMessageHandler(MessageId messageId, Action<NetworkCommunicator, ByteBuffer> action)
        {
            _networkAgent.RegisterMessageHandler((ushort)messageId, action);
        }

        public void SendMessage(ushort messageId, byte[] message)
        {
            _networkAgent.SendMessage(messageId, message);
        }

        public void SendMessage(ushort messageId, ByteBuffer message)
        {
            _networkAgent.SendMessage(messageId, message);
        }

        public void Close()
        {
            _networkAgent.Close();
        }
    }
}
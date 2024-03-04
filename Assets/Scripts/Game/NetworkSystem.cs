using System.Threading.Tasks;
using Common;
using Core.Network;
using Network;
using Protocols;

namespace Game
{
    public class NetworkSystem
    {
        private NetworkAgent _networkAgent;

        public void Init()
        {
            _networkAgent = new NetworkAgent();

            _networkAgent.Connect("127.0.0.1", 10001).Await(RegisterMessageRoute);
        }

        public void Update()
        {
            _networkAgent.Update();
        }

        private void RegisterMessageRoute()
        {
            _networkAgent.RegisterMessageHandler((ushort)MessageId.Login, (communicator, info) =>
            {
                var response = info.Message.ReadUInt16();
                Log.Info($"{response.ToString()}");
            });
            _networkAgent.RegisterMessageHandler((ushort)MessageId.Register, (communicator, info) =>
            {
                var response = info.Message.ReadUInt16();
                Log.Info($"{response.ToString()}");
            });
        }

        public void SendLogin(User user)
        {
            var userData = ProtoUtils.Encode(user);
            _networkAgent.SendMessage((ushort)MessageId.Login, userData);
        }

        public void SendRegister(User user)
        {
            var userData = ProtoUtils.Encode(user);
            _networkAgent.SendMessage((ushort)MessageId.Register, userData);
        }
    }
}
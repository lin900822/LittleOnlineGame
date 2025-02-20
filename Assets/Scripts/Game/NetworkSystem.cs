using Shared;
using Shared.Common;
using Shared.Logger;
using Shared.Network;

namespace Game
{
    public class NetworkSystem
    {
        private NetworkAgent _networkAgent;

        public void Init()
        {
            _networkAgent = new NetworkAgent();

            _networkAgent.Connect("127.0.0.1", 50001).Await(RegisterMessageRoute);
        }

        public void Update()
        {
            _networkAgent.Update();
        }

        private void RegisterMessageRoute()
        {
            _networkAgent.RegisterMessageHandler((ushort)MessageId.Login, (communicator, info) =>
            {
                var response = info.ReadUInt16();
                Log.Info($"{response.ToString()}");
            });
            _networkAgent.RegisterMessageHandler((ushort)MessageId.Register, (communicator, info) =>
            {
                var response = info.ReadUInt16();
                Log.Info($"{response.ToString()}");
            });
            _networkAgent.RegisterMessageHandler((ushort)MessageId.Hello, (communicator, byteBuffer) =>
            {
                if (!byteBuffer.TryDecode(out Hello hello))
                    return;
                Log.Info($"{hello.Content.ToString()}");
            });
            _networkAgent.RegisterMessageHandler((ushort)MessageId.Match, (communicator, byteBuffer) =>
            {
                if (!byteBuffer.TryDecode(out Match match))
                    return;
                Log.Info($"{match.PlayerId}");
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

        public void SendHello(Hello hello)
        {
            var helloData = ProtoUtils.Encode(hello);
            _networkAgent.SendMessage((ushort)MessageId.Hello, helloData); 
        }
        
        public void SendMatch(Match match)
        {
            var matchData = ProtoUtils.Encode(match);
            _networkAgent.SendMessage((ushort)MessageId.Match, matchData); 
        }
    }
}
using Game.UI;
using Shared;
using Shared.Common;
using Shared.Logger;
using Shared.Network;

namespace Game
{
    public class BattleService : ServiceBase
    {
        private NetworkAgent _battleAgent;
        
        public override void Init()
        {
            _networkSystem.RegisterMessageHandler(MessageId.M2C_RoomMatched, M2C_RoomMatched);
        }

        private void M2C_RoomMatched(NetworkCommunicator communicator, ByteBuffer byteBuffer)
        {
            if (!byteBuffer.TryDecode(out M2C_RoomMatched m2CRoomMatched))
                return;
            
            _uIManager.HideWindow<Window_Matching>();
            
            _battleAgent = new NetworkAgent();
            _battleAgent.Connect(m2CRoomMatched.Ip, m2CRoomMatched.Port).Await(() =>
            {
                var c2BJoinRoom = new C2B_JoinRoom()
                {
                    KeyToEnterRoom = m2CRoomMatched.KeyToEnterRoom,
                    PlayerId = 1,
                };
                var data = ProtoUtils.Encode(c2BJoinRoom);
                _battleAgent.SendMessage((ushort)MessageId.C2B_JoinRoom, data);
            });
        }
    }
}
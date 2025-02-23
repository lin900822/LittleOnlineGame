using Shared;
using Shared.Logger;

namespace Game
{
    public class PlayerService : ServiceBase
    {
        private uint _playerId;
        private string _username;
        private uint _coins;
        
        public override void Init()
        {
            _networkSystem.RegisterMessageHandler(MessageId.M2C_LoginSync, (communicator, byteBuffer) =>
            {
                if (!byteBuffer.TryDecode<M2C_LoginSync>(out M2C_LoginSync m2CLoginSync))
                    return;
                
                Log.Info($"{m2CLoginSync.PlayerId} {m2CLoginSync.Username} {m2CLoginSync.Coins}");

                _playerId = m2CLoginSync.PlayerId;
                _username = m2CLoginSync.Username;
                _coins = m2CLoginSync.Coins;
            });
        }
    }
}
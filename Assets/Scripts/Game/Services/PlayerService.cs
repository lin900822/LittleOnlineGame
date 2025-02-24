using Shared;
using Shared.Logger;

namespace Game
{
    public class PlayerService : ServiceBase
    {
        public uint PlayerId { get; private set; }
        public string Username { get; private set; }
        public uint Coins { get; private set; }

        public override void Init()
        {
            _networkSystem.RegisterMessageHandler(MessageId.M2C_LoginSync, (communicator, byteBuffer) =>
            {
                if (!byteBuffer.TryDecode<M2C_LoginSync>(out M2C_LoginSync m2CLoginSync))
                    return;

                Log.Info($"{m2CLoginSync.PlayerId} {m2CLoginSync.Username} {m2CLoginSync.Coins}");

                PlayerId = m2CLoginSync.PlayerId;
                Username = m2CLoginSync.Username;
                Coins = m2CLoginSync.Coins;
            });
        }
    }
}
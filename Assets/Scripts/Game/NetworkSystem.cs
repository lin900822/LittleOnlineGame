using System.Diagnostics;
using Framework.UI;
using Game.UI;
using Shared;
using Shared.Common;
using Shared.Logger;
using Shared.Network;
using Unity.VisualScripting;

namespace Game
{
    public class NetworkSystem
    {
        private NetworkAgent _networkAgent;
        
        private static NetworkSystem _instance;

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

            _networkAgent.Connect("127.0.0.1", 50001).Await(RegisterMessageRoute);
        }

        public void Update()
        {
            _networkAgent.Update();
        }

        private void RegisterMessageRoute()
        {
            _networkAgent.RegisterMessageHandler((ushort)MessageId.M2C_StateCode, (communicator, byteBuffer) =>
            {
                var stateCode = (StateCode)byteBuffer.ReadUInt32();
                switch (stateCode)
                {
                    case StateCode.Login_Success:
                    case StateCode.Register_Success:
                        UIManager.Instance.HideWindow<Window_Login>();
                        UIManager.Instance.PopUpWindow<Window_Lobby>();
                        UIManager.Instance.PopUpWindow<Window_SystemInfo>(new UIData_SystemInfo()
                        {
                            Message = "登入成功!",
                            IsNeedShowCancelButton = false,
                        });
                        break;
                    case StateCode.LoginOrRegister_Failed_InfoEmpty:
                        UIManager.Instance.PopUpWindow<Window_SystemInfo>(new UIData_SystemInfo()
                        {
                            Message = "Username or Password 不可為空!",
                            IsNeedShowCancelButton = true,
                        });
                        break;
                    default:
                        Log.Warn($"{stateCode} 未處理");
                        break;
                }
            });
        }

        public void SendLogin(C2M_PlayerLoginOrRegister playerData)
        {
            var bytes = ProtoUtils.Encode(playerData);
            _networkAgent.SendMessage((ushort)MessageId.C2M_PlayerLoginOrRegister, bytes);
        }
    }
}
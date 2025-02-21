using System.Diagnostics;
using Framework.UI;
using Game.UI;
using Shared;
using Shared.Common;
using Shared.Logger;
using Shared.Network;
using Unity.VisualScripting;
using UnityEngine.SceneManagement;

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
                        UIManager.Instance.HideWindow<Window_Init>();
                        UIManager.Instance.HideWindow<Window_Login>();
                        UIManager.Instance.PopUpWindow<Window_Lobby>();
                        UIManager.Instance.PopUpWindow<Window_SystemInfo>(new UIData_SystemInfo()
                        {
                            Message = "登入成功!",
                        });
                        break;
                    case StateCode.LoginOrRegister_Failed_InfoEmpty:
                        UIManager.Instance.PopUpWindow<Window_SystemInfo>(new UIData_SystemInfo()
                        {
                            Message = "Username or Password 不可為空!",
                        });
                        break;
                    case StateCode.Login_Failed_InfoWrong:
                        UIManager.Instance.PopUpWindow<Window_SystemInfo>(new UIData_SystemInfo()
                        {
                            Message = "Username or Password 錯誤!",
                        });
                        break;
                    case StateCode.Register_Failed_UserExist:
                        UIManager.Instance.PopUpWindow<Window_SystemInfo>(new UIData_SystemInfo()
                        {
                            Message = "Username已存在",
                        });
                        break;
                    case StateCode.TimeOut:
                        UIManager.Instance.PopUpWindow<Window_SystemInfo>(new UIData_SystemInfo()
                        {
                            Message = "連線遇時",
                            OnConfirm = () =>
                            {
                                UIManager.Instance.HideAllWindows();
                                
                                UIManager.Instance.PopUpWindow<Window_Init>();
                                UIManager.Instance.PopUpWindow<Window_Login>();
                                Init();
                            }
                        });
                        break;
                    case StateCode.Another_User_LoggedIn:
                        Log.Warn("此帳號已在其他裝置登入");
                        UIManager.Instance.PopUpWindow<Window_SystemInfo>(new UIData_SystemInfo()
                        {
                            Message = "此帳號已在其他裝置登入",
                            OnConfirm = () =>
                            {
                                UIManager.Instance.HideAllWindows();
                                
                                UIManager.Instance.PopUpWindow<Window_Init>();
                                UIManager.Instance.PopUpWindow<Window_Login>();
                                Close();
                                Init();
                            }
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

        public void Close()
        {
            _networkAgent.Close();
        }
    }
}
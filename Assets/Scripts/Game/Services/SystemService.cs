using Game.UI;
using Shared;
using Shared.Logger;

namespace Game
{
    public class SystemService : ServiceBase
    {
        public override void Init()
        {
            _networkSystem.RegisterMessageHandler(MessageId.M2C_StateCode, (communicator, byteBuffer) =>
            {
                var stateCode = (StateCode)byteBuffer.ReadUInt32();
                switch (stateCode)
                {
                    case StateCode.Login_Success:
                    case StateCode.Register_Success:
                        _uIManager.HideWindow<Window_Init>();
                        _uIManager.HideWindow<Window_Login>();
                        _uIManager.PopUpWindow<Window_Lobby>();
                        _uIManager.PopUpWindow<Window_SystemInfo>(new UIData_SystemInfo()
                        {
                            Message = "登入成功!",
                        });
                        break;
                    case StateCode.LoginOrRegister_Failed_InfoEmpty:
                        _uIManager.PopUpWindow<Window_SystemInfo>(new UIData_SystemInfo()
                        {
                            Message = "Username or Password 不可為空!",
                        });
                        break;
                    case StateCode.Login_Failed_InfoWrong:
                        _uIManager.PopUpWindow<Window_SystemInfo>(new UIData_SystemInfo()
                        {
                            Message = "Username or Password 錯誤!",
                        });
                        break;
                    case StateCode.Register_Failed_UserExist:
                        _uIManager.PopUpWindow<Window_SystemInfo>(new UIData_SystemInfo()
                        {
                            Message = "Username已存在",
                        });
                        break;
                    case StateCode.TimeOut:
                        _uIManager.PopUpWindow<Window_SystemInfo>(new UIData_SystemInfo()
                        {
                            Message = "連線遇時",
                            OnConfirm = () =>
                            {
                                _uIManager.HideAllWindows();
                                
                                _uIManager.PopUpWindow<Window_Init>();
                                _uIManager.PopUpWindow<Window_Login>();
                                Init();
                            }
                        });
                        break;
                    case StateCode.Another_User_LoggedIn:
                        Log.Warn("此帳號已在其他裝置登入");
                        _uIManager.PopUpWindow<Window_SystemInfo>(new UIData_SystemInfo()
                        {
                            Message = "此帳號已在其他裝置登入",
                            OnConfirm = () =>
                            {
                                _uIManager.HideAllWindows();
                                
                                _uIManager.PopUpWindow<Window_Init>();
                                _uIManager.PopUpWindow<Window_Login>();
                                _networkSystem.Close();
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
    }
}
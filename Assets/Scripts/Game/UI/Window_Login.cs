/*---------------------------------
 - Title: UI腳本生成工具
 - Created Date: 2/20/2025 4:57:08 PM
 - 注意事項:
 - 1. 請不要刪除或修改 "// Start XXX" 和 "// End XXX" 等相關的註解, 自動生成器會依賴他們
 - 2. 請不要在 "Start UI Components Fields" 和 "End UI Components Fields" 之間加入新的程式碼
 - 3. 請不要在 "Start Start InitUIComponent" 和 "End Start InitUIComponent" 之間加入新的程式碼
---------------------------------*/
using UnityEngine.UI;
using UnityEngine;
using Framework.UI;
using Shared;
using Shared.Network;

namespace Game.UI
{
    public class Window_Login : WindowBase
    {
        #region - UI Components Fields -

        // Start UI Components Fields
        private Button CloseButton;
        private InputField UsernameInputField;
        private InputField PasswordInputField;
        private Button RegisterButton;
        private Button LoginButton;
        // End UI Components Fields

        #endregion

        #region - Life Cycle -

        public override void OnLoaded()
        {
            base.OnLoaded();
            InitUIComponent();
        }

        private void InitUIComponent()
        {
            // Start InitUIComponent
            CloseButton = UIComponentContainer[0].GetComponent<Button>();
            UsernameInputField = UIComponentContainer[1].GetComponent<InputField>();
            PasswordInputField = UIComponentContainer[2].GetComponent<InputField>();
            RegisterButton = UIComponentContainer[3].GetComponent<Button>();
            LoginButton = UIComponentContainer[4].GetComponent<Button>();
            AddButtonClickListener(CloseButton, OnCloseButtonClick);
            AddInputFieldListener(UsernameInputField, OnUsernameInputChange, OnUsernameInputEnd);
            AddInputFieldListener(PasswordInputField, OnPasswordInputChange, OnPasswordInputEnd);
            AddButtonClickListener(RegisterButton, OnRegisterButtonClick);
            AddButtonClickListener(LoginButton, OnLoginButtonClick);
            // End InitUIComponent
        }

        public override void OnShow(UIData uiData = null)
        {
            base.OnShow(uiData);
        }

        public override void OnHide()
        {
            base.OnHide();
        }

        public override void OnUnloaded()
        {
            base.OnUnloaded();
        }

        #endregion

        #region - UI Component Events -
        
        // Start UI Component Events

        private void OnCloseButtonClick()
        {
            HideWindow();
        }

        private void OnUsernameInputChange(string text)
        {
            
        }

        private void OnUsernameInputEnd(string text)
        {
            
        }

        private void OnPasswordInputChange(string text)
        {
            
        }

        private void OnPasswordInputEnd(string text)
        {
            
        }

        private void OnRegisterButtonClick()
        {
            UIManager.Instance.PopUpWindow<Window_Register>();
        }

        private void OnLoginButtonClick()
        {
            var username = UsernameInputField.text;
            var password = PasswordInputField.text;
            
            var playerData = new C2M_PlayerLoginOrRegister()
            {
                Username = username,
                Password = password,
                IsLogin = true,
            };
            var bytes = ProtoUtils.Encode(playerData);
            
            NetworkSystem.Instance.SendMessage((ushort)MessageId.C2M_PlayerLoginOrRegister, bytes);
        }

        // End UI Component Events
        
        #endregion

        #region - Custom Logic -
        
        
        
        #endregion
    }
}

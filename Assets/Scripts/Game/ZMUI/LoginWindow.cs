/*---------------------------------
 - Title: UI腳本生成工具
 - Date: 2024/5/12 下午 02:39:36
 - 注意事項:
 - 1. 請不要刪除或修改 "// Start XXX" 和 "// End XXX" 等相關的註解, 自動生成器會依賴他們
 - 2. 請不要在 "Start UI Components Fields" 和 "End UI Components Fields" 之間加入新的程式碼
 - 3. 請不要在 "Start Start InitUIComponent" 和 "End Start InitUIComponent" 之間加入新的程式碼
---------------------------------*/

using UnityEngine.UI;
using UnityEngine;
using Framework.ZMUI;

namespace Game.ZMUI
{
    public class LoginWindow : WindowBase
    {
        #region - UI Components Fields -

        // Start UI Components Fields
        private Button     CloseButton;
        private InputField AccountInputField;
        private InputField PasswordInputField;
        private Toggle     RememberToggle;
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
            CloseButton        = UIComponentContainer[0].GetComponent<Button>();
            AccountInputField  = UIComponentContainer[1].GetComponent<InputField>();
            PasswordInputField = UIComponentContainer[2].GetComponent<InputField>();
            RememberToggle     = UIComponentContainer[3].GetComponent<Toggle>();
            AddButtonClickListener(CloseButton, OnCloseButtonClick);
            AddInputFieldListener(AccountInputField, OnAccountInputChange, OnAccountInputEnd);
            AddInputFieldListener(PasswordInputField, OnPasswordInputChange, OnPasswordInputEnd);
            AddToggleClickListener(RememberToggle, OnRememberToggleChange);
            // End InitUIComponent
        }

        public override void OnShow()
        {
            base.OnShow();
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

        private void OnAccountInputChange(string text)
        {
        }

        private void OnAccountInputEnd(string text)
        {
        }

        private void OnPasswordInputChange(string text)
        {
        }

        private void OnPasswordInputEnd(string text)
        {
            Debug.Log($"OnPasswordInputEnd: {text}");
        }

        private void OnRememberToggleChange(bool state, Toggle toggle)
        {
            Debug.Log($"OnRememberToggleChange: {state}");
        }
        // End UI Component Events

        #endregion

        #region - Custom Logic -

        #endregion
    }
}
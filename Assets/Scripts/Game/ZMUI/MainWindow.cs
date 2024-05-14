/*---------------------------------
 - Title: UI腳本生成工具
 - Date: 2024/5/12 下午 02:39:48
 - 注意事項:
 - 1. 請不要刪除或修改 "// Start XXX" 和 "// End XXX" 等相關的註解, 自動生成器會依賴他們
 - 2. 請不要在 "Start UI Components Fields" 和 "End UI Components Fields" 之間加入新的程式碼
 - 3. 請不要在 "Start Start InitUIComponent" 和 "End Start InitUIComponent" 之間加入新的程式碼
---------------------------------*/

using System;
using System.Threading.Tasks;
using Framework.ZMUI;
using UnityEngine;
using UnityEngine.UI;

namespace Game.ZMUI
{
    public class MainWindow : WindowBase
    {
        #region - UI Components Fields -

        // Start UI Components Fields
        private Text       TitleText;
        private Button     CloseButton;
        private Button     OpenLoginButton;
        private Button     OpenFriendButton;
        private Button     OpenInfoButton;
        private Button     OpenActivityButton;
        private Button     ChargeButton;
        private GameObject ActivityBannerGameObject;
        private InputField NameInputField;
        private InputField DescriptionInputField;
        private Toggle     RememberToggle;
        private Image      CharacterImage;
        // End UI Components Fields

        #endregion

        #region - Life Cycle -

        public override void OnLoaded()
        {
            base.OnLoaded();
            InitUIComponent();

            CharacterImage.gameObject.SetActive(false);
            ActivityBannerGameObject.SetActive(false);
        }

        private void InitUIComponent()
        {
            // Start InitUIComponent
            TitleText                = UIComponentContainer[0].GetComponent<Text>();
            CloseButton              = UIComponentContainer[1].GetComponent<Button>();
            OpenLoginButton          = UIComponentContainer[2].GetComponent<Button>();
            OpenFriendButton         = UIComponentContainer[3].GetComponent<Button>();
            OpenInfoButton           = UIComponentContainer[4].GetComponent<Button>();
            OpenActivityButton       = UIComponentContainer[5].GetComponent<Button>();
            ChargeButton             = UIComponentContainer[6].GetComponent<Button>();
            ActivityBannerGameObject = UIComponentContainer[7].gameObject;
            NameInputField           = UIComponentContainer[8].GetComponent<InputField>();
            DescriptionInputField    = UIComponentContainer[9].GetComponent<InputField>();
            RememberToggle           = UIComponentContainer[10].GetComponent<Toggle>();
            CharacterImage           = UIComponentContainer[11].GetComponent<Image>();
            AddButtonClickListener(CloseButton,        OnCloseButtonClick);
            AddButtonClickListener(OpenLoginButton,    OnOpenLoginButtonClick);
            AddButtonClickListener(OpenFriendButton,   OnOpenFriendButtonClick);
            AddButtonClickListener(OpenInfoButton,     OnOpenInfoButtonClick);
            AddButtonClickListener(OpenActivityButton, OnOpenActivityButtonClick);
            AddButtonClickListener(ChargeButton,       OnChargeButtonClick);
            AddInputFieldListener(NameInputField,        OnNameInputChange,        OnNameInputEnd);
            AddInputFieldListener(DescriptionInputField, OnDescriptionInputChange, OnDescriptionInputEnd);
            AddToggleClickListener(RememberToggle, OnRememberToggleChange);
            // End InitUIComponent
        }

        public override void OnShow()
        {
            base.OnShow();

            Debug.Log("OnShow");
        }

        public override void OnHide()
        {
            base.OnHide();

            Debug.Log("OnHide");
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

        private void OnOpenFriendButtonClick()
        {
            TestTask();
        }

        private void OnOpenInfoButtonClick()
        {
            ActivityBannerGameObject.SetActive(false);
            CharacterImage.gameObject.SetActive(true);
        }

        private void OnOpenLoginButtonClick()
        {
            UIModule.Instance.PopUpWindow<LoginWindow>();
        }

        private void OnOpenActivityButtonClick()
        {
            ActivityBannerGameObject.SetActive(true);
            CharacterImage.gameObject.SetActive(false);
        }

        private void OnNameInputChange(string text)
        {
        }

        private void OnNameInputEnd(string text)
        {
            Debug.Log($"OnNameInputEnd {text}");
        }

        private void OnRememberToggleChange(bool state, Toggle toggle)
        {
        }

        private void OnDescriptionInputChange(string text)
        {
        }

        private void OnDescriptionInputEnd(string text)
        {
            Debug.Log($"OnDescriptionInputEnd {text}");
        }

        private void OnChargeButtonClick()
        {
        }

        // End UI Component Events

        #endregion

        #region - Custom Logic -

        private async Task TestTask()
        {
            Debug.Log($"Before Task Thread Id: {Environment.CurrentManagedThreadId}");
            for (int i = 0; i < 100; i++)
            {
                await Task.Run(() => { Debug.Log($"In Task Thread Id: {Environment.CurrentManagedThreadId}"); });
            }

            Debug.Log($"After Task Thread Id: {Environment.CurrentManagedThreadId}");
        }

        #endregion
    }
}
/*---------------------------------
 - Title: UI腳本生成工具
 - Date: 2024/5/12 下午 02:39:48
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
    public class MainWindow : WindowBase
    {
        #region - UI Components Fields -

        // Start UI Components Fields
        private Text       TitleText;
        private GameObject ImageGameObject;
        private Button     CloseButton;
        private Button     OpenFriendButton;
        private Button     OpenInfoButton;
        private Button     OpenMissionButton;
        private Button     OpenLoginButton;
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
            TitleText         = UIComponentContainer[0].GetComponent<Text>();
            ImageGameObject   = UIComponentContainer[1].gameObject;
            CloseButton       = UIComponentContainer[2].GetComponent<Button>();
            OpenFriendButton  = UIComponentContainer[3].GetComponent<Button>();
            OpenInfoButton    = UIComponentContainer[4].GetComponent<Button>();
            OpenMissionButton = UIComponentContainer[5].GetComponent<Button>();
            OpenLoginButton   = UIComponentContainer[6].GetComponent<Button>();
            AddButtonClickListener(CloseButton, OnCloseButtonClick);
            AddButtonClickListener(OpenFriendButton, OnOpenFriendButtonClick);
            AddButtonClickListener(OpenInfoButton, OnOpenInfoButtonClick);
            AddButtonClickListener(OpenMissionButton, OnOpenMissionButtonClick);
            AddButtonClickListener(OpenLoginButton, OnOpenLoginButtonClick);
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

        private void OnOpenFriendButtonClick()
        {
        }

        private void OnOpenInfoButtonClick()
        {
        }

        private void OnOpenMissionButtonClick()
        {
        }

        private void OnOpenLoginButtonClick()
        {
            UIModule.Instance.PopUpWindow<LoginWindow>();
        }
        // End UI Component Events

        #endregion

        #region - Custom Logic -

        #endregion
    }
}
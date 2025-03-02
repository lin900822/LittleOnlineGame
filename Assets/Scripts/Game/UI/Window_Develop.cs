/*---------------------------------
 - Title: UI腳本生成工具
 - Created Date: 3/2/2025 12:58:26 PM
 - 注意事項:
 - 1. 請不要刪除或修改 "// Start XXX" 和 "// End XXX" 等相關的註解, 自動生成器會依賴他們
 - 2. 請不要在 "Start UI Components Fields" 和 "End UI Components Fields" 之間加入新的程式碼
 - 3. 請不要在 "Start Start InitUIComponent" 和 "End Start InitUIComponent" 之間加入新的程式碼
---------------------------------*/

using UnityEngine.UI;
using UnityEngine;
using Framework.UI;

namespace Game.UI
{
    public class Window_Develop : WindowBase
    {
        #region - UI Components Fields -

        // Start UI Components Fields
        private Button CloseButton;
        private InputField IPInputField;
        private InputField PortInputField;
        private Button ConnectButton;
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
            IPInputField = UIComponentContainer[1].GetComponent<InputField>();
            PortInputField = UIComponentContainer[2].GetComponent<InputField>();
            ConnectButton = UIComponentContainer[3].GetComponent<Button>();
            AddButtonClickListener(CloseButton, OnCloseButtonClick);
            AddInputFieldListener(IPInputField, OnIPInputChange, OnIPInputEnd);
            AddInputFieldListener(PortInputField, OnPortInputChange, OnPortInputEnd);
            AddButtonClickListener(ConnectButton, OnConnectButtonClick);
            // End InitUIComponent
        }

        public override void OnShow(UIData uiData = null)
        {
            base.OnShow(uiData);

            var ip = PlayerPrefs.GetString("IP");
            if (!string.IsNullOrWhiteSpace(ip))
                IPInputField.text = ip;
            
            var port = PlayerPrefs.GetInt("Port");
            if (port > 0)
                PortInputField.text = port.ToString();
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

        private void OnIPInputChange(string text)
        {
        }

        private void OnIPInputEnd(string text)
        {
        }

        private void OnPortInputChange(string text)
        {
        }

        private void OnPortInputEnd(string text)
        {
        }

        private void OnConnectButtonClick()
        {
            if (!string.IsNullOrWhiteSpace(IPInputField.text))
            {
                var ip = IPInputField.text;
                NetworkSystem.Instance.IP = ip;
                PlayerPrefs.SetString("IP", ip);
            }

            if (!string.IsNullOrWhiteSpace(PortInputField.text))
            {
                var port = int.Parse(PortInputField.text);
                NetworkSystem.Instance.Port = port;
                PlayerPrefs.SetInt("Port", port);
            }

            NetworkSystem.Instance.Init();
        }

        // End UI Component Events

        #endregion

        #region - Custom Logic -

        #endregion
    }
}
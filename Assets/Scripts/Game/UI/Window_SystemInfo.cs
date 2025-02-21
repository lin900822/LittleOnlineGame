/*---------------------------------
 - Title: UI腳本生成工具
 - Created Date: 2/20/2025 5:28:11 PM
 - 注意事項:
 - 1. 請不要刪除或修改 "// Start XXX" 和 "// End XXX" 等相關的註解, 自動生成器會依賴他們
 - 2. 請不要在 "Start UI Components Fields" 和 "End UI Components Fields" 之間加入新的程式碼
 - 3. 請不要在 "Start Start InitUIComponent" 和 "End Start InitUIComponent" 之間加入新的程式碼
---------------------------------*/

using System;
using UnityEngine.UI;
using UnityEngine;
using Framework.UI;

namespace Game.UI
{
    public class UIData_SystemInfo : UIData
    {
        public string Message { get; set; }
        public bool IsNeedShowCancelButton { get; set; }
        public Action OnConfirm { get; set; }
        public Action OnCancel { get; set; }
    }
    
    public class Window_SystemInfo : WindowBase
    {
        private UIData_SystemInfo _uiDataSystemInfo;
        
        #region - UI Components Fields -

        // Start UI Components Fields
        private Button CloseButton;
        private Text MessageText;
        private Button CancelButton;
        private Button ConfirmButton;
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
            MessageText = UIComponentContainer[1].GetComponent<Text>();
            CancelButton = UIComponentContainer[2].GetComponent<Button>();
            ConfirmButton = UIComponentContainer[3].GetComponent<Button>();
            AddButtonClickListener(CloseButton, OnCloseButtonClick);
            AddButtonClickListener(CancelButton, OnCancelButtonClick);
            AddButtonClickListener(ConfirmButton, OnConfirmButtonClick);
            // End InitUIComponent
        }

        public override void OnShow(UIData uiData = null)
        {
            base.OnShow(uiData);

            if (uiData is UIData_SystemInfo uiDataSystemInfo)
            {
                _uiDataSystemInfo = uiDataSystemInfo;
                MessageText.text = _uiDataSystemInfo.Message;
                CancelButton.gameObject.SetActive(_uiDataSystemInfo.IsNeedShowCancelButton);
            }
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

        private void OnCancelButtonClick()
        {
            HideWindow();
            
            _uiDataSystemInfo.OnCancel?.Invoke();
        }

        private void OnConfirmButtonClick()
        {
            HideWindow();
            
            _uiDataSystemInfo.OnConfirm?.Invoke();
        }

        // End UI Component Events
        
        #endregion

        #region - Custom Logic -
        
        
        
        #endregion
    }
}

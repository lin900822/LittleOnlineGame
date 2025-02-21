/*---------------------------------
 - Title: UI腳本生成工具
 - Created Date: 2/21/2025 8:45:24 PM
 - 注意事項:
 - 1. 請不要刪除或修改 "// Start XXX" 和 "// End XXX" 等相關的註解, 自動生成器會依賴他們
 - 2. 請不要在 "Start UI Components Fields" 和 "End UI Components Fields" 之間加入新的程式碼
 - 3. 請不要在 "Start Start InitUIComponent" 和 "End Start InitUIComponent" 之間加入新的程式碼
---------------------------------*/

using System;
using UnityEngine.UI;
using UnityEngine;
using Framework.UI;
using Shared;

namespace Game.UI
{
    public class Window_Matching : WindowBase
    {
        private float _timer;
        private int _dotCount;
        
        #region - UI Components Fields -

        // Start UI Components Fields
        private Button CloseButton;
        private Text MessageText;
        private Button CancelButton;
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
            AddButtonClickListener(CloseButton, OnCloseButtonClick);
            AddButtonClickListener(CancelButton, OnCancelButtonClick);
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

        public override void OnUpdate()
        {
            if (_timer >= .5f)
            {
                _dotCount++;
                _dotCount %= 4;
                MessageText.text = "正在配對實力相近的對手";
                for (int i = 0; i < _dotCount; i++)
                {
                    MessageText.text += ".";
                }
                _timer = 0f;
            }

            _timer += Time.deltaTime;
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
            NetworkSystem.Instance.SendMessage((ushort)MessageId.C2M_CancelJoinMatchQueue, Array.Empty<byte>());
            HideWindow();
        }

        // End UI Component Events
        
        #endregion

        #region - Custom Logic -
        
        
        
        #endregion
    }
}

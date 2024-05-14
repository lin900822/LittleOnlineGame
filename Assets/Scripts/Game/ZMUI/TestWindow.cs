/*---------------------------------
 - Title: UI腳本生成工具
 - Created Date: 5/15/2024 12:40:13 AM
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
    public class TestWindow : WindowBase
    {
        #region - UI Components Fields -

        // Start UI Components Fields
        private Button CloseButton;
        private Button TestButton;
        private InputField InfoInputField;
        private Button SubmitButton;
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
            TestButton = UIComponentContainer[1].GetComponent<Button>();
            InfoInputField = UIComponentContainer[2].GetComponent<InputField>();
            SubmitButton = UIComponentContainer[3].GetComponent<Button>();
            AddButtonClickListener(CloseButton, OnCloseButtonClick);
            AddButtonClickListener(TestButton, OnTestButtonClick);
            AddInputFieldListener(InfoInputField, OnInfoInputChange, OnInfoInputEnd);
            AddButtonClickListener(SubmitButton, OnSubmitButtonClick);
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

        private void OnTestButtonClick()
        {
            Debug.Log("123");
        }

        private void OnInfoInputChange(string text)
        {
            
        }

        private void OnInfoInputEnd(string text)
        {
            
        }

        private void OnSubmitButtonClick()
        {
            Debug.Log($"{InfoInputField.text}");
        }

        // End UI Component Events
        
        #endregion

        #region - Custom Logic -
        
        
        
        #endregion
    }
}

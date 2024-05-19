/*---------------------------------
 - Title: UI腳本生成工具
 - Created Date: 2024/5/19 下午 08:00:21
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
    public class SettingsWindow : WindowBase
    {
        #region - UI Components Fields -

        // Start UI Components Fields
        private Button CloseButton;
        private Text TitleText;
        private Button Stack1Button;
        private Button Stack2Button;
        private Button Stack3Button;
        private Button Stack4Button;
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
            TitleText = UIComponentContainer[1].GetComponent<Text>();
            Stack1Button = UIComponentContainer[2].GetComponent<Button>();
            Stack2Button = UIComponentContainer[3].GetComponent<Button>();
            Stack3Button = UIComponentContainer[4].GetComponent<Button>();
            Stack4Button = UIComponentContainer[5].GetComponent<Button>();
            AddButtonClickListener(CloseButton, OnCloseButtonClick);
            AddButtonClickListener(Stack1Button, OnStack1ButtonClick);
            AddButtonClickListener(Stack2Button, OnStack2ButtonClick);
            AddButtonClickListener(Stack3Button, OnStack3ButtonClick);
            AddButtonClickListener(Stack4Button, OnStack4ButtonClick);
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

        private void OnStack1ButtonClick()
        {
            HideWindow();
            UIManager.Instance.PushWindowToStack<TestStackWindow1>();
        }

        private void OnStack2ButtonClick()
        {
            HideWindow();
            UIManager.Instance.PushWindowToStack<TestStackWindow2>();
        }

        private void OnStack3ButtonClick()
        {
            HideWindow();
            UIManager.Instance.PushWindowToStack<TestStackWindow3>();
        }

        private void OnStack4ButtonClick()
        {
            HideWindow();
            UIManager.Instance.PushWindowToStack<TestStackWindow4>();
        }

        // End UI Component Events
        
        #endregion

        #region - Custom Logic -
        
        
        
        #endregion
    }
}

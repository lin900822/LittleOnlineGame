/*---------------------------------
 - Title: UI腳本生成工具
 - Created Date: 5/23/2024 5:39:14 PM
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
    public class Window_Function : WindowBase
    {
        #region - UI Components Fields -

        // Start UI Components Fields
        private Text CoinAmountText;
        private Button AddCoinButton;
        private Button SettingsButton;
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
            CoinAmountText = UIComponentContainer[0].GetComponent<Text>();
            AddCoinButton = UIComponentContainer[1].GetComponent<Button>();
            SettingsButton = UIComponentContainer[2].GetComponent<Button>();
            AddButtonClickListener(AddCoinButton, OnAddCoinButtonClick);
            AddButtonClickListener(SettingsButton, OnSettingsButtonClick);
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

        private void OnAddCoinButtonClick()
        {
            
        }

        private void OnSettingsButtonClick()
        {
            UIManager.Instance.PopUpWindow<Window_Settings>();
        }

        // End UI Component Events
        
        #endregion

        #region - Custom Logic -
        
        
        
        #endregion
    }
}

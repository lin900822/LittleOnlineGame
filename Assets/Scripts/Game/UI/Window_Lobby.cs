/*---------------------------------
 - Title: UI腳本生成工具
 - Created Date: 2024/5/23 上午 01:33:01
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
    public class Window_Lobby : WindowBase
    {
        #region - UI Components Fields -

        // Start UI Components Fields
        private Button PlayerInfoButton;
        private Image ExpProgressImage;
        private Text ExpText;
        private Text PlayerLevelText;
        private Text PlayerNameText;
        private Button GiftButton;
        private Button AdButton;
        private Button PlayButton;
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
            PlayerInfoButton = UIComponentContainer[0].GetComponent<Button>();
            ExpProgressImage = UIComponentContainer[1].GetComponent<Image>();
            ExpText = UIComponentContainer[2].GetComponent<Text>();
            PlayerLevelText = UIComponentContainer[3].GetComponent<Text>();
            PlayerNameText = UIComponentContainer[4].GetComponent<Text>();
            GiftButton = UIComponentContainer[5].GetComponent<Button>();
            AdButton = UIComponentContainer[6].GetComponent<Button>();
            PlayButton = UIComponentContainer[7].GetComponent<Button>();
            AddButtonClickListener(PlayerInfoButton, OnPlayerInfoButtonClick);
            AddButtonClickListener(GiftButton, OnGiftButtonClick);
            AddButtonClickListener(AdButton, OnAdButtonClick);
            AddButtonClickListener(PlayButton, OnPlayButtonClick);
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

        private void OnPlayerInfoButtonClick()
        {
            UIManager.Instance.PopUpWindow<Window_PlayerInfo>();
        }

        private void OnGiftButtonClick()
        {
            UIManager.Instance.PopUpWindow<Window_Gift>();
        }

        private void OnPlayButtonClick()
        {
            
        }
        private void OnAdButtonClick()
        {
            
        }


        // End UI Component Events
        
        #endregion

        #region - Custom Logic -
        
        
        
        #endregion
    }
}

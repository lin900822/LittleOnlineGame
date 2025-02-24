/*---------------------------------
 - Title: UI腳本生成工具
 - Created Date: 2/24/2025 12:45:50 AM
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
    public class UIMatchedData : UIData
    {
        public string Player1Name { get; set; }
        public string Player2Name { get; set; }
    }
    
    public class Window_Matched : WindowBase
    {
        private UIMatchedData _uiMatchedData;
        
        #region - UI Components Fields -

        // Start UI Components Fields
        private Text Player1Text;
        private Text Player2Text;
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
            Player1Text = UIComponentContainer[0].GetComponent<Text>();
            Player2Text = UIComponentContainer[1].GetComponent<Text>();
            // End InitUIComponent
        }

        public override void OnShow(UIData uiData = null)
        {
            base.OnShow(uiData);

            if (uiData is UIMatchedData uiMatchedData)
            {
                _uiMatchedData = uiMatchedData;

                Player1Text.text = _uiMatchedData.Player1Name;
                Player2Text.text = _uiMatchedData.Player2Name;
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

        // End UI Component Events
        
        #endregion

        #region - Custom Logic -
        
        
        
        #endregion
    }
}

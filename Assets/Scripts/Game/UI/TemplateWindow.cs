/*---------------------------------
 - Title: UI腳本生成工具
 - Date: 2024/5/14 上午 11:06:07
 - 注意事項:
 - 1. 請不要刪除或修改 "// Start XXX" 和 "// End XXX" 等相關的註解, 自動生成器會依賴他們
 - 2. 請不要在 "Start UI Components Fields" 和 "End UI Components Fields" 之間加入新的程式碼
 - 3. 請不要在 "Start Start InitUIComponent" 和 "End Start InitUIComponent" 之間加入新的程式碼
---------------------------------*/

using Framework.UI;
using UnityEngine.UI;

namespace Game.UI
{
    public class TemplateWindow : WindowBase
    {
        #region - UI Components Fields -

        // Start UI Components Fields
        private Button CloseButton;
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
            AddButtonClickListener(CloseButton, OnCloseButtonClick);
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
        // End UI Component Events

        #endregion

        #region - Custom Logic -

        #endregion
    }
}
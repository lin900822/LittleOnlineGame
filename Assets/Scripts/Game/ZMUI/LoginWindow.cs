using Framework.ZMUI;
using UnityEngine;
using UnityEngine.UI;

namespace Game.ZMUI
{
    public class LoginWindow : WindowBase
    {
        private Button CloseButton;
        
        #region 声明周期函数
        //调用机制与Mono Awake一致
        public override void OnLoaded()
        {
            base.OnLoaded();
            
            CloseButton = transform.Find("Content/Background/[Button]Close").GetComponent<Button>();
            AddButtonClickListener(CloseButton, OnCloseButtonClick);
        }
        //物体显示时执行
        public override void OnShow()
        {
            base.OnShow();
        }
        //物体隐藏时执行
        public override void OnHide()
        {
            base.OnHide();
        }
        //物体销毁时执行
        public override void OnUnloaded()
        {
            base.OnUnloaded();
        }
        #endregion
        #region API Function

        #endregion
        #region UI组件事件
        public void OnCloseButtonClick()
        {
            HideWindow();
        }
        #endregion
    }
}
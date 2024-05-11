/*---------------------------------
 - Title: UI腳本生成工具
 - Date: 2024/5/11 下午 02:35:20
 - Description:UI 表现层，该层只负责界面的交互、表现相关的更新，不允许编写任何业务逻辑代码
 - 注意:以下文件是自动生成的，再次生成不会覆盖原有的代码，会在原有的代码上进行新增，可放心使用
---------------------------------*/
using UnityEngine.UI;
using UnityEngine;
using Framework.ZMUI;

namespace Game.ZMUI
{
	public class TemplateWindow : WindowBase
	{
	
		 #region - UI Components Fields -

		 private Button CloseButton;
		 private Button OpenLoginButton;
		 private Button OpenFriendButton;
	
		 #endregion

		 #region - Life Cycle -
	
		 public override void OnLoaded()
		 {
			 base.OnLoaded();
			 CloseButton = transform.Find("Content/Background/[Button]Close").GetComponent<Button>();
			 OpenLoginButton = transform.Find("Content/Background/[Button]OpenLogin").GetComponent<Button>();
			 OpenFriendButton = transform.Find("Content/Background/[Button]OpenFriend").GetComponent<Button>();
			 AddButtonClickListener(CloseButton, OnCloseButtonClick);
			 AddButtonClickListener(OpenLoginButton, OnOpenLoginButtonClick);
			 AddButtonClickListener(OpenFriendButton, OnOpenFriendButtonClick);
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
	
		 #region API Function


		    
		 #endregion
	
		 #region - UI Component Events -
	
		 private void OnCloseButtonClick()
		 {
		
			HideWindow();
		 }
	
		 private void OnOpenLoginButtonClick()
		 {
		
		 }
	
		 private void OnOpenFriendButtonClick()
		 {
		
		 }
	
		 #endregion
	}
}

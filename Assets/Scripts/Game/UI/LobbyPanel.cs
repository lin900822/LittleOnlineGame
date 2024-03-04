using Framework.UI;
using UnityEngine;
using UnityEngine.UI;

namespace Game.UI
{
    public class LobbyPanel : UIPanel
    {
        [SerializeField]
        private Button _testPanelBtn;

        protected override void OnStartShow()
        {
            _testPanelBtn.onClick.AddListener(OnTemplatePanelBtnClicked);
        }

        protected override void OnStartHide()
        {
            _testPanelBtn.onClick.RemoveAllListeners();
        }

        private async void OnTemplatePanelBtnClicked()
        {
            await GameApp.Instance.UISystem.ShowPanel(PanelId.LoginPanel.ToString());
        }
    }
}
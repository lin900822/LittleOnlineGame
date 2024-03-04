using System.Threading.Tasks;
using Common;
using Framework.UI;
using Game.UI;

namespace Game
{
    public class GameApp
    {
        private static GameApp _instance;

        public static GameApp Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new GameApp();
                }
                return _instance;
            }
        }

        private UISettings _uiSettings;


        #region - Systems -

        public UISystem      UISystem      { get; private set; }
        public NetworkSystem NetworkSystem { get; private set; }

        #endregion

        public void InjectSettings(UISettings uiSettings)
        {
            _uiSettings = uiSettings;
        }

        public void Init()
        {
            UISystem      = new UISystem();
            NetworkSystem = new NetworkSystem();

            UISystem.Init(_uiSettings);
            NetworkSystem.Init();

            InitBasicUI().Await();
        }

        public void Update()
        {
            NetworkSystem.Update();
        }

        private async Task InitBasicUI()
        {
            await UISystem.ShowPanel(PanelId.LobbyPanel.ToString());
        }

        
    }
}
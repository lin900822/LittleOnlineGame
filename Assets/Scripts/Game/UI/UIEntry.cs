using System;
using Framework.UI;
using UnityEngine;
using UnityEngine.UI;

namespace Game.UI
{
    public class UIEntry : MonoBehaviour
    {
        [SerializeField]
        public Button SettingsButton;
        
        private void Awake()
        {
            UIManager.Instance.Init();
        }
    
        void Start()
        {
            SettingsButton.onClick.AddListener(OpenSettingsWindow);
        }

        private void OnDestroy()
        {
            SettingsButton.onClick.RemoveListener(OpenSettingsWindow);
        }

        void OpenSettingsWindow()
        {
            UIManager.Instance.PopUpWindow<SettingsWindow>();
        }
    
        public void Update()
        {
            if (Input.GetKeyDown(KeyCode.W))
            {
                UIManager.Instance.PopUpWindow<TestWindow>();
            }
            if (Input.GetKeyDown(KeyCode.E))
            {
                UIManager.Instance.PopUpWindow<MainWindow>();
            }
            
            if (Input.GetKeyDown(KeyCode.G))
            {
                UIManager.Instance.PopUpWindow<Window_Lobby>();
            }
        }
    }
}

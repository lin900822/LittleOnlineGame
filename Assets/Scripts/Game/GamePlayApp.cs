using System.Collections.Generic;
using Framework.UI;
using Game.UI;
using Shared;
using Shared.Network;
using UnityEngine;

namespace Game
{
    public class GamePlayApp : MonoBehaviour
    {
        private bool IsInitDone = false;

        public GameObject BattleGO;
        public Camera UICamera;
        public RectTransform UICanvas;

        public static GamePlayApp Instance;
        
        private void Awake()
        {
            Instance = this;
            
            NetworkSystem.Instance.OnConnected += HandleNetworkConnected;
            
            UIManager.Instance.Init();
            NetworkSystem.Instance.Init();
        }

        private void HandleNetworkConnected()
        {
            ServiceSystem.Instance.Init();
            
            UIManager.Instance.PopUpWindow<Window_Init>();
            UIManager.Instance.PopUpWindow<Window_Login>();

            IsInitDone = true;
        }

        private void Update()
        {
            if (!IsInitDone)
                return;
            
            NetworkSystem.Instance.Update();
            UIManager.Instance.Update();
            ServiceSystem.Instance.Update();

            if (Input.GetKeyDown(KeyCode.G))
            {
                UIManager.Instance.PopUpWindow<Window_SystemInfo>(new UIData_SystemInfo()
                {
                    Message = "測試!",
                    IsNeedShowCancelButton = false,
                });
            }
            
            if (Input.GetKeyDown(KeyCode.X))
            {
                NetworkSystem.Instance.SendMessage((ushort)MessageId.Debug, ProtoUtils.Encode(new C2M_Debug()));
            }
        }

        public GameObject Find(string name)
        {
            return GameObject.Find(name);
        }

        private void OnApplicationQuit()
        {
            NetworkSystem.Instance.Close();
        }
    }
}
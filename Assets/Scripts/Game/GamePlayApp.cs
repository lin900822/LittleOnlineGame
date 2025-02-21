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
        private List<ServiceBase> _services;

        private bool IsInitDone = false;
        
        private void Awake()
        {
            NetworkSystem.Instance.OnConnected += HandleNetworkConnected;
            
            UIManager.Instance.Init();
            NetworkSystem.Instance.Init();
        }

        private void HandleNetworkConnected()
        {
            _services = new List<ServiceBase>();
            _services.Add(new SystemService());
            _services.Add(new PlayerService());

            foreach (var serviceBase in _services)
            {
                serviceBase.Init();
            }

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
            
            foreach (var serviceBase in _services)
            {
                serviceBase.Update();
            }

            if (Input.GetKeyDown(KeyCode.G))
            {
                UIManager.Instance.PopUpWindow<Window_SystemInfo>(new UIData_SystemInfo()
                {
                    Message = "測試!",
                    IsNeedShowCancelButton = false,
                });
            }
            
            if (Input.GetKeyDown(KeyCode.D))
            {
                NetworkSystem.Instance.SendMessage((ushort)MessageId.Debug, ProtoUtils.Encode(new C2M_Debug()));
            }
        }

        private void OnApplicationQuit()
        {
            NetworkSystem.Instance.Close();
        }
    }
}
using System;
using Framework.UI;
using Game.UI;
using Shared.Logger;
using UnityEngine;

namespace Game
{
    public class GamePlayApp : MonoBehaviour
    {
        private void Start()
        {
            UIManager.Instance.Init();
            NetworkSystem.Instance.Init();

            UIManager.Instance.PopUpWindow<Window_Login>();
        }

        private void Update()
        {
            NetworkSystem.Instance.Update();

            if (Input.GetKeyDown(KeyCode.G))
            {
                UIManager.Instance.PopUpWindow<Window_SystemInfo>(new UIData_SystemInfo()
                {
                    Message = "測試!",
                    IsNeedShowCancelButton = false,
                });
            }
        }
    }
}
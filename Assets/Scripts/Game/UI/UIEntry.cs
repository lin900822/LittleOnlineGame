using System;
using Framework.UI;
using Shared.Logger;
using UnityEngine;
using UnityEngine.UI;

namespace Game.UI
{
    public class UIEntry : MonoBehaviour
    {
        private void Awake()
        {
            UIManager.Instance.Init();
            Log.Info("UIEntry");
        }
    
        public void Update()
        {
            if (Input.GetKeyDown(KeyCode.G))
            {
                UIManager.Instance.PopUpWindow<Window_Lobby>();
                UIManager.Instance.PopUpWindow<Window_Function>();
            }
            if (Input.GetKeyDown(KeyCode.Q))
            {
                UIManager.Instance.PopUpWindow<Window_Settings>();
            }
            if (Input.GetKeyDown(KeyCode.W))
            {
                UIManager.Instance.PopUpWindow<Window_PlayerInfo>();
            }
        }
    }
}

using System;
using Framework.UI;
using UnityEngine;
using UnityEngine.UI;

namespace Game.UI
{
    public class UIEntry : MonoBehaviour
    {
        private void Awake()
        {
            UIManager.Instance.Init();
        }
    
        public void Update()
        {
            if (Input.GetKeyDown(KeyCode.G))
            {
                UIManager.Instance.PopUpWindow<Window_Lobby>();
                UIManager.Instance.PopUpWindow<Window_Function>();
            }
        }
    }
}

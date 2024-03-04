using System;
using Framework.UI;
using UnityEngine;

namespace Game
{
    public class GameEntry : MonoBehaviour
    {
        [SerializeField]
        private UISettings _uiSettings;

        private void Awake()
        {
            GameApp.Instance.InjectSettings(_uiSettings);
            GameApp.Instance.Init();
        }

        private void Update()
        {
            GameApp.Instance.Update();
        }
    }
}
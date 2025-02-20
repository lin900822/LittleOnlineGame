using System;
using Shared.Logger;
using UnityEngine;

namespace Game
{
    public class GamePlayApp : MonoBehaviour
    {
        private NetworkSystem _networkSystem;
        
        private void Start()
        {
            _networkSystem = new NetworkSystem();
            _networkSystem.Init();
            
            Log.Info("GamePlay Init");
        }

        private void Update()
        {
            _networkSystem.Update();

            if (Input.GetKeyDown(KeyCode.H))
            {
                _networkSystem.SendHello(new Hello()
                {
                    Content = "Unity Client",
                });
            }
            
            if (Input.GetKeyDown(KeyCode.M))
            {
                _networkSystem.SendMatch(new Match()
                {
                    PlayerId = 1120,
                });
            }
        }
    }
}
using System.Collections.Generic;

namespace Game
{
    public class ServiceSystem
    {
        public PlayerService PlayerService { get; private set; }
        
        private List<ServiceBase> _services;
        
        public static ServiceSystem Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new ServiceSystem();
                }

                return _instance;
            }
        }

        private static ServiceSystem _instance;

        public ServiceSystem()
        {
            _services = new List<ServiceBase>();
            
            _services.Add(new SystemService());
            
            PlayerService = new PlayerService();
            _services.Add(PlayerService);
            
            _services.Add(new BattleService());
        }

        public void Init()
        {
            foreach (var serviceBase in _services)
            {
                serviceBase.Init();
            }
        }

        public void Update()
        {
            foreach (var serviceBase in _services)
            {
                serviceBase.Update();
            }
        }
    }
}
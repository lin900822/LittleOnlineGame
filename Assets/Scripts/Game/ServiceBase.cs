using Framework.UI;

namespace Game
{
    public abstract class ServiceBase
    {
        protected UIManager _uIManager => UIManager.Instance;
        protected NetworkSystem _networkSystem => NetworkSystem.Instance;
        
        public virtual void Init()
        {
            
        }

        public virtual void Update()
        {
            
        }
    }
}
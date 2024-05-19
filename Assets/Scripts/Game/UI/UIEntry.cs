using Framework.UI;
using UnityEngine;

namespace Game.UI
{
    public class UIEntry : MonoBehaviour
    {
        private void Awake()
        {
            UIManager.Instance.Init();
        }
    
        void Start()
        {
            UIManager.Instance.PushWindowToStack<TestStackWindow1>();
            UIManager.Instance.PushWindowToStack<TestStackWindow2>();
            UIManager.Instance.PushWindowToStack<TestStackWindow3>();
            UIManager.Instance.PushWindowToStack<TestStackWindow4>();
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
        
            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                UIManager.Instance.PushWindowToStack<TestStackWindow1>();
            }
            if (Input.GetKeyDown(KeyCode.Alpha2))
            {
                UIManager.Instance.PushWindowToStack<TestStackWindow2>();
            }
            if (Input.GetKeyDown(KeyCode.Alpha3))
            {
                UIManager.Instance.PushWindowToStack<TestStackWindow3>();
            }
            if (Input.GetKeyDown(KeyCode.Alpha4))
            {
                UIManager.Instance.PushWindowToStack<TestStackWindow4>();
            }
            
            if (Input.GetKeyDown(KeyCode.Alpha5))
            {
                UIManager.Instance.PopUpWindow<TestStackWindow1>();
            }
            if (Input.GetKeyDown(KeyCode.Alpha6))
            {
                UIManager.Instance.PopUpWindow<TestStackWindow2>();
            }
            if (Input.GetKeyDown(KeyCode.Alpha7))
            {
                UIManager.Instance.PopUpWindow<TestStackWindow3>();
            }
            if (Input.GetKeyDown(KeyCode.Alpha8))
            {
                UIManager.Instance.PopUpWindow<TestStackWindow4>();
            }
        }
    }
}

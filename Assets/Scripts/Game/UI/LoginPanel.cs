using Common;
using Framework.UI;
using UnityEngine;
using UnityEngine.UI;

namespace Game.UI
{
    public class LoginPanel : UIPanel
    {
        [Space(30)]
        [SerializeField]
        private Button _closeBtn;
        
        [SerializeField]
        private InputField _usernameInputField;
        
        [SerializeField]
        private InputField _passwordInputField;

        [SerializeField]
        private Button _loginBtn;
        
        private void Awake()
        {
            _closeBtn.onClick.AddListener(OnCloseBtnClicked);
            _loginBtn.onClick.AddListener(OnLoginBtnClicked);
        }

        private void OnDestroy()
        {
            _closeBtn.onClick.RemoveAllListeners();
        }

        private void OnCloseBtnClicked()
        {
            _uiSystem.HidePeekPanel();
        }

        private void OnLoginBtnClicked()
        {
            var username = _usernameInputField.text;
            var password = _passwordInputField.text;

            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
            {
                Log.Error($"帳號或密碼不得為空");
                return;
            }

            var user = new User()
            {
                Username = username,
                Password = password
            };
            
            GameApp.Instance.NetworkSystem.SendLogin(user);
            
            _usernameInputField.text = string.Empty;
            _passwordInputField.text = string.Empty;
        }
    }
}
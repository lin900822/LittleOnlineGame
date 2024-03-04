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

        [SerializeField]
        private Button _registerBtn;

        private void Awake()
        {
            _closeBtn.onClick.AddListener(OnCloseBtnClicked);
            _loginBtn.onClick.AddListener(OnLoginBtnClicked);
            _registerBtn.onClick.AddListener(OnRegisterBtnClicked);
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
            if (!TryPackUser(out var user))
                return;

            GameApp.Instance.NetworkSystem.SendLogin(user);

            Clear();
        }

        private void OnRegisterBtnClicked()
        {
            if (!TryPackUser(out var user))
                return;

            GameApp.Instance.NetworkSystem.SendRegister(user);

            Clear();
        }

        private bool TryPackUser(out User user)
        {
            var username = _usernameInputField.text;
            var password = _passwordInputField.text;

            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
            {
                Log.Error($"帳號或密碼不得為空");
                user = null;
                return false;
            }

            user = new User()
            {
                Username = username,
                Password = password
            };
            return true;
        }

        private void Clear()
        {
            _usernameInputField.text = string.Empty;
            _passwordInputField.text = string.Empty;
        }
    }
}
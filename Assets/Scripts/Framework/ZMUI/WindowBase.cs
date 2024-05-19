using System.Collections.Generic;
using System.Threading.Tasks;
using Common;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Framework.ZMUI
{
    public class WindowBase : WindowBehaviour
    {
        private List<Button>     _buttonList     = new List<Button>();
        private List<Toggle>     _toggleList     = new List<Toggle>();
        private List<InputField> _inputFieldList = new List<InputField>();

        private   CanvasGroup _uiMask;
        private   CanvasGroup _canvasGroup;
        protected Transform   _uiContent;
        protected bool        _isDisableAnim = false;

        /// <summary>
        /// 初始化基本元件
        /// </summary>
        private void InitComponent()
        {
            _canvasGroup = transform.GetComponent<CanvasGroup>();
            _uiMask      = transform.Find("Mask").GetComponent<CanvasGroup>();
            _uiContent   = transform.Find("Content").transform;
        }

        #region - Life Cycle -

        public override void OnLoaded()
        {
            base.OnLoaded();
            InitComponent();
        }

        public override void OnShow()
        {
            base.OnShow();
            PlayShowAnimation().Await(null,
                (ex) => { Debug.LogError(ex.ToString()); });
        }

        public override void OnUpdate()
        {
            base.OnUpdate();
        }

        public override void OnHide()
        {
            base.OnHide();
        }

        public override void OnUnloaded()
        {
            base.OnUnloaded();
            RemoveAllButtonListener();
            RemoveAllToggleListener();
            RemoveAllInputListener();
            _buttonList.Clear();
            _toggleList.Clear();
            _inputFieldList.Clear();
        }

        #endregion

        #region - Animation -

        private async Task PlayShowAnimation()
        {
            await PlayAnimation(ShowAnimationClip);
        }

        private async Task PlayHideAnimation()
        {
            await PlayAnimation(HideAnimationClip);
        }

        private async Task PlayAnimation(UIAnimationClip clip)
        {
            await UIAnimationPlayer.PlayClip(clip);
        }

        #endregion

        public void HideWindow()
        {
            PlayHideAnimation().Await(() => { UIModule.Instance.HideWindow(Name); },
                (ex) => { Debug.LogError(ex.ToString()); });
        }

        public override void SetVisible(bool isVisible)
        {
            _canvasGroup.alpha          = isVisible ? 1 : 0;
            _canvasGroup.blocksRaycasts = isVisible;
            IsVisible                   = isVisible;
        }

        public void SetMaskVisible(bool isVisible)
        {
            _uiMask.alpha = isVisible ? 1 : 0;
        }

        #region - UI Events -

        public void AddButtonClickListener(Button button, UnityAction action)
        {
            if (button == null) return;
            if (!_buttonList.Contains(button))
            {
                _buttonList.Add(button);
            }

            button.onClick.RemoveAllListeners();
            button.onClick.AddListener(action);
        }

        public void AddToggleClickListener(Toggle toggle, UnityAction<bool, Toggle> action)
        {
            if (toggle == null) return;
            if (!_toggleList.Contains(toggle))
            {
                _toggleList.Add(toggle);
            }

            toggle.onValueChanged.RemoveAllListeners();
            toggle.onValueChanged.AddListener((isOn) => { action?.Invoke(isOn, toggle); });
        }

        public void AddInputFieldListener(
            InputField          inputField,
            UnityAction<string> onChangeAction,
            UnityAction<string> endAction)
        {
            if (inputField == null) return;
            if (!_inputFieldList.Contains(inputField))
            {
                _inputFieldList.Add(inputField);
            }

            inputField.onValueChanged.RemoveAllListeners();
            inputField.onEndEdit.RemoveAllListeners();
            inputField.onValueChanged.AddListener(onChangeAction);
            inputField.onEndEdit.AddListener(endAction);
        }

        public void RemoveAllButtonListener()
        {
            foreach (var item in _buttonList)
            {
                item.onClick.RemoveAllListeners();
            }
        }

        public void RemoveAllToggleListener()
        {
            foreach (var item in _toggleList)
            {
                item.onValueChanged.RemoveAllListeners();
            }
        }

        public void RemoveAllInputListener()
        {
            foreach (var item in _inputFieldList)
            {
                item.onValueChanged.RemoveAllListeners();
                item.onEndEdit.RemoveAllListeners();
            }
        }

        #endregion
    }
}
﻿using System.Collections.Generic;
using Shared.Logger;
using UnityEngine;

namespace Framework.UI
{
    public class UIManager
    {
        private static UIManager _instance;

        public static UIManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new UIManager();
                }

                return _instance;
            }
        }

        private Camera    _uiCamera;
        private Transform _uiRoot;

        private Dictionary<string, WindowBase> _loadedWindowByName = new Dictionary<string, WindowBase>();
        private List<WindowBase>               _loadedWindowList   = new List<WindowBase>();
        private List<WindowBase>               _visibleWindowList  = new List<WindowBase>();

        private Stack<WindowBase> _windowStack = new Stack<WindowBase>();

        public void Init()
        {
            _uiCamera = GameObject.Find("UICamera").GetComponent<Camera>();
            _uiRoot   = GameObject.Find("UIRoot").transform;
        }

        #region - PopUp -

        /// <summary>
        /// 顯示一個Window
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T PopUpWindow<T>(UIData uiData = null) where T : WindowBase, new()
        {
            var type         = typeof(T);
            var windowName   = type.Name;
            var loadedWindow = GetLoadedWindow(windowName);
            if (loadedWindow != null)
            {
                return ShowLoadedWindow(windowName, uiData) as T;
            }

            var newWindow = LoadWindow<T>();
            _visibleWindowList.Add(newWindow);
            newWindow.transform.SetAsLastSibling();
            newWindow.SetVisible(true);
            newWindow.OnShow(uiData);

            RefreshWindowMask();

            return newWindow;
        }

        private WindowBase ShowLoadedWindow(string windowName, UIData uiData = null)
        {
            if (_loadedWindowByName.TryGetValue(windowName, out var windowBase))
            {
                if (windowBase.gameObject == null || windowBase.IsVisible)
                {
                    return windowBase;
                }

                _visibleWindowList.Add(windowBase);
                windowBase.transform.SetAsLastSibling();
                windowBase.SetVisible(true);
                RefreshWindowMask();

                windowBase.OnShow(uiData);
                return windowBase;
            }
            
            Log.Error($"{windowName} 未加載");
            return null;
        }

        /// <summary>
        /// 只加載Window, 不顯示
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T LoadWindow<T>() where T : WindowBase, new()
        {
            var windowName = typeof(T).Name;
            if (_loadedWindowByName.TryGetValue(windowName, out var loadedWindow))
            {
                return loadedWindow as T;
            }

            var windowBase = new T();
            var windowGO   = InstantiateWindowGO(windowName);

            if (windowGO != null)
            {
                windowBase.gameObject             = windowGO;
                windowBase.transform              = windowGO.transform;
                windowBase.Canvas                 = windowGO.GetComponent<Canvas>();
                windowBase.UIComponentContainer   = windowGO.GetComponent<UIComponentContainer>();
                windowBase.UIAnimationPlayer      = windowBase.UIComponentContainer.UIAnimationPlayer;
                windowBase.ShowAnimationClip      = windowBase.UIComponentContainer.ShowAnimationClip;
                windowBase.HideAnimationClip      = windowBase.UIComponentContainer.HideAnimationClip;
                windowBase.Name                   = windowGO.name;
                windowBase.Canvas.worldCamera     = _uiCamera;

                var rectTrans = windowGO.GetComponent<RectTransform>();
                rectTrans.anchorMax = Vector2.one;
                rectTrans.offsetMax = Vector2.zero;
                rectTrans.offsetMin = Vector2.zero;

                windowBase.OnLoaded();
                windowBase.SetVisible(false);

                _loadedWindowByName.Add(windowName, windowBase);
                _loadedWindowList.Add(windowBase);

                return windowBase;
            }

            Log.Error($"加載{windowName}失敗");
            return null;
        }


        private GameObject InstantiateWindowGO(string windowName)
        {
            var windowGO = Object.Instantiate(Resources.Load<GameObject>($"Window/{windowName}"), _uiRoot);

            windowGO.transform.localScale    = Vector3.one;
            windowGO.transform.localPosition = Vector3.zero;
            windowGO.transform.rotation      = Quaternion.identity;
            windowGO.name                    = windowName;
            return windowGO;
        }

        #endregion

        #region - Hide -

        public void HideWindow<T>() where T : WindowBase
        {
            HideWindow(typeof(T).Name);
        }

        public void HideWindow(string windowName)
        {
            var window = GetLoadedWindow(windowName);
            HideWindow(window);
        }

        public void HideAllWindows()
        {
            foreach (var entry in _loadedWindowByName)
            {
                HideWindow(entry.Value);
            }
        }

        private void HideWindow(WindowBase window)
        {
            if (window != null && window.IsVisible)
            {
                _visibleWindowList.Remove(window);
                window.SetVisible(false);
                RefreshWindowMask();
                window.OnHide();
            }

            PopUpNextWindowFromStack(window);
        }

        #endregion

        #region - Destory -

        public void DestroyWindow<T>() where T : WindowBase
        {
            DestroyWindow(typeof(T).Name);
        }

        private void DestroyWindow(string wndName)
        {
            var window = GetLoadedWindow(wndName);
            DestroyWindow(window);
        }

        private void DestroyWindow(WindowBase window)
        {
            if (window == null) 
                return;

            if (_windowStack.Contains(window))
            {
                Log.Error($"{window.Name}還在Stack中, 無法釋放");
                return;
            }
            
            if (_loadedWindowByName.ContainsKey(window.Name))
            {
                _loadedWindowByName.Remove(window.Name);
                _loadedWindowList.Remove(window);
                _visibleWindowList.Remove(window);
            }

            window.SetVisible(false);
            RefreshWindowMask();
            window.OnHide();
            window.OnUnloaded();
            Object.Destroy(window.gameObject);

            PopUpNextWindowFromStack(window);
        }

        public void DestroyAllWindow(List<string> filterList = null)
        {
            for (int i = _loadedWindowList.Count - 1; i >= 0; i--)
            {
                var window = _loadedWindowList[i];
                if (window == null || (filterList != null && filterList.Contains(window.Name)))
                {
                    continue;
                }

                DestroyWindow(window.Name);
            }

            Resources.UnloadUnusedAssets();
        }

        #endregion

        #region - Common -

        private WindowBase GetLoadedWindow(string windowName)
        {
            return _loadedWindowByName.GetValueOrDefault(windowName);
        }

        /// <summary>
        /// 獲得已顯示的Window
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T GetVisibleWindow<T>() where T : WindowBase
        {
            var type = typeof(T);
            foreach (var item in _visibleWindowList)
            {
                if (item.Name == type.Name)
                {
                    return (T)item;
                }
            }

            Log.Error($"找不到{type.Name}");
            return null;
        }

        // 顯示最上層 Window的 Mask
        private void RefreshWindowMask()
        {
            WindowBase maxOrderWindow = null;
            var        maxOrder       = 0;
            var        maxIndex       = 0;

            foreach (var window in _visibleWindowList)
            {
                if (window != null && window.gameObject != null)
                {
                    window.SetMaskVisible(false);
                    if (maxOrderWindow == null)
                    {
                        maxOrderWindow = window;
                        maxOrder       = window.Canvas.sortingOrder;
                        maxIndex       = window.transform.GetSiblingIndex();
                    }
                    else
                    {
                        if (maxOrder < window.Canvas.sortingOrder)
                        {
                            maxOrderWindow = window;
                            maxOrder       = window.Canvas.sortingOrder;
                        }
                        else if (maxOrder == window.Canvas.sortingOrder &&
                                 maxIndex < window.transform.GetSiblingIndex())
                        {
                            maxOrderWindow = window;
                            maxIndex       = window.transform.GetSiblingIndex();
                        }
                    }
                }
            }

            if (maxOrderWindow != null)
            {
                maxOrderWindow.SetMaskVisible(true);
            }
        }

        #endregion

        #region - Stack -

        public void PushWindowToStack<T>() where T : WindowBase, new()
        {
            var window = PopUpWindow<T>();
            if (_windowStack.TryPeek(out var peekWindow))
            {
                _windowStack.Push(window);
                peekWindow.HideWindow();
            }
            else
            {
                _windowStack.Push(window);
            }
        }

        private void PopUpNextWindowFromStack(WindowBase currentWindow)
        {
            if (!_windowStack.TryPeek(out var peekWindow)) 
                return;
            if (peekWindow != currentWindow) 
                return;
            _ = _windowStack.Pop();

            if (!_windowStack.TryPeek(out peekWindow)) 
                return;
            peekWindow.OnPopFromStack();
            ShowLoadedWindow(peekWindow.Name);
        }

        #endregion

        #region - Life Cycle -

        public void Update()
        {
            foreach (var windowBase in _visibleWindowList)
            {
                windowBase.OnUpdate();
            }
        }

        #endregion
    }
}
using System;
using UnityEngine;

namespace Framework.ZMUI
{
    public abstract class WindowBehaviour
    {
        public GameObject         gameObject       { get; set; }
        public Transform          transform        { get; set; }
        public Canvas             Canvas           { get; set; }
        public string             Name             { get; set; }
        public bool               IsVisible        { get; set; }
        public bool               IsPopStack       { get; set; }
        public Action<WindowBase> PopStackListener { get; set; }

        public virtual void OnLoaded()
        {
        }

        public virtual void OnShow()
        {
        }

        public virtual void OnUpdate()
        {
        }

        public virtual void OnHide()
        {
        }

        public virtual void OnUnloaded()
        {
        }

        public virtual void SetVisible(bool isVisible)
        {
        }
    }
}
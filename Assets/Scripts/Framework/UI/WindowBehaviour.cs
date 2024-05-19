using UnityEngine;

namespace Framework.UI
{
    public abstract class WindowBehaviour
    {
        public GameObject           gameObject           { get; set; }
        public Transform            transform            { get; set; }
        public Canvas               Canvas               { get; set; }
        public UIComponentContainer UIComponentContainer { get; set; }
        public UIAnimationPlayer    UIAnimationPlayer    { get; set; }
        public UIAnimationClip      ShowAnimationClip    { get; set; }
        public UIAnimationClip      HideAnimationClip    { get; set; }
        public string               Name                 { get; set; }
        public bool                 IsVisible            { get; protected set; }

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

        public virtual void OnPopFromStack()
        {
        }
    }
}
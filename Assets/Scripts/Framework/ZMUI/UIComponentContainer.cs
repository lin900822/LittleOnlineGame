using System.Collections.Generic;
using Framework.Common;
using UnityEngine;

namespace Framework.ZMUI
{
    public class UIComponentContainer : MonoBehaviour
    {
        [Title("[動畫元件]")]
        public UIAnimationPlayer UIAnimationPlayer = new UIAnimationPlayer();
        
        public UIAnimationClip ShowAnimationClip;
        public UIAnimationClip HideAnimationClip;
        
        [Space(20)]
        [Title("[UI元件]")]
        [InspectorReadOnly]
        public List<GameObject> UIComponents = new List<GameObject>();
        
        public GameObject this[int index] => UIComponents[index];
    }
}
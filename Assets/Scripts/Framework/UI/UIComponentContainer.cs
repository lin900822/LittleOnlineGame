using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Framework.UI
{
    public class UIComponentContainer : MonoBehaviour
    {
        [Title("動畫")]
        public UIAnimationPlayer UIAnimationPlayer = new UIAnimationPlayer();
        
        public UIAnimationClip ShowAnimationClip;
        public UIAnimationClip HideAnimationClip;
        
        [Space(10)]
        [Title("自動綁定")]
        [ReadOnly]
        public List<GameObject> UIComponents = new List<GameObject>();
        
        public GameObject this[int index] => UIComponents[index];
    }
}
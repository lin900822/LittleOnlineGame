using System.Collections.Generic;
using UnityEngine;

namespace Framework.ZMUI
{
    public class UIComponentContainer : MonoBehaviour
    {
        public List<GameObject> UIComponents = new List<GameObject>();
        
        public GameObject this[int index] => UIComponents[index];
    }
}
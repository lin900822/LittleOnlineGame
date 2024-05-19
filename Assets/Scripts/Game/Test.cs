using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class Test : MonoBehaviour
{
    [ContextMenu("SpawnButtons")]
    public void SpawnButtons()
    {
        for (int i = 0; i < 1000; i++)
        {
            var newGO = new GameObject();
            newGO.transform.SetParent(transform);
            newGO.name = $"[Button] Test{i}";
            newGO.AddComponent<Button>();
        }
        
    }
}

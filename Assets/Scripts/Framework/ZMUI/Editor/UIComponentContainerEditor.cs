using UnityEditor;
using UnityEngine;

namespace Framework.ZMUI.Editor
{
    [CustomEditor(typeof(UIComponentContainer))]
    public class UIComponentContainerEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            GUI.enabled = false;

            EditorGUILayout.HelpBox("此元件由自動生成工具生成, 會將Prefab中符合格式的UI物件加入到UI Components", MessageType.Info);

            DrawDefaultInspector();
        }
    }
}
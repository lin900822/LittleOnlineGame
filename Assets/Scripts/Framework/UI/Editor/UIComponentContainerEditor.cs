using UnityEditor;

namespace Framework.UI.Editor
{
    [CustomEditor(typeof(UIComponentContainer))]
    public class UIComponentContainerEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            EditorGUILayout.HelpBox("此元件由自動生成工具生成", MessageType.Info);

            DrawDefaultInspector();
        }
    }
}
using Framework.Common;
using UnityEditor;
using UnityEngine;

namespace Framework.Editor
{
    [CustomPropertyDrawer(typeof(InspectorReadOnlyAttribute))]
    public class ReadOnlyDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            GUI.enabled = false;
            EditorGUI.PropertyField(position, property, label);
            GUI.enabled = true;
        }
    }
    
    [CustomPropertyDrawer(typeof(TitleAttribute))]
    public class TitleDrawer : DecoratorDrawer
    {
        public override void OnGUI(Rect position)
        {
            TitleAttribute titleAttribute = (TitleAttribute)attribute;
            GUIStyle       style          = new GUIStyle(GUI.skin.label);
            style.fontStyle = FontStyle.Bold;
            style.fontSize = 16;
            style.alignment = TextAnchor.MiddleLeft;
            EditorGUI.LabelField(position, titleAttribute.Title, style);
        }

        public override float GetHeight()
        {
            return base.GetHeight() + 20f;
        }
    }
    
    [CustomPropertyDrawer(typeof(InfoAttribute))]
    public class InfoDrawer : DecoratorDrawer
    {
        public override void OnGUI(Rect position)
        {
            InfoAttribute titleAttribute = (InfoAttribute)attribute;
            GUIStyle      style          = new GUIStyle(GUI.skin.box);
            style.fontStyle         = FontStyle.Normal;
            style.normal.textColor  = Color.gray;
            style.alignment         = TextAnchor.MiddleCenter;
            style.padding           = new RectOffset(10, 10, 5, 5);
            style.normal.background = MakeTex(1, 1, new Color(0.25f, 0.5f, 0.75f, 1f));
            EditorGUI.LabelField(position, titleAttribute.Info, style);
        }
        
        public override float GetHeight()
        {
            return base.GetHeight() + 5f;
        }

        private Texture2D MakeTex(int width, int height, Color col)
        {
            Color[] pix = new Color[width * height];
            for (int i = 0; i < pix.Length; i++)
                pix[i] = col;
            Texture2D result = new Texture2D(width, height);
            result.SetPixels(pix);
            result.Apply();
            return result;
        }
    }
}
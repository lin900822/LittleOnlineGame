using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;

namespace Framework.ZMUI.Editor
{
    public class CodePreviewWindow : EditorWindow
    {
        private string  _codeContent;
        private string  _filePath;
        private Vector2 _scroll = new Vector2();

        /// <summary>
        /// 显示代码展示窗口
        /// </summary>
        public static void ShowCodePreviewWindow(
            string                     content,
            string                     filePath,
            Dictionary<string, string> methodCodeByMethodName = null)
        {
            var window = (CodePreviewWindow)GetWindowWithRect(typeof(CodePreviewWindow),
                new Rect(100, 50, 1000, 800), true, "Code Generate Preview");
            window._codeContent = content;
            window._filePath    = filePath;

            // 腳本已存在, 只新增必要程式碼
            if (File.Exists(filePath) && methodCodeByMethodName != null)
            {
                var originCode = File.ReadAllText(filePath);

                originCode = ReplaceContentBetween(originCode, content, "// Start UI Components Fields",
                    "// End UI Components Fields");
                originCode = ReplaceContentBetween(originCode, content, "// Start InitUIComponent",
                    "// End InitUIComponent");

                // 新增方法
                var methodCodeStartIndex  = originCode.IndexOf("// Start UI Component Events");
                var methodCodeEndIndex  = originCode.IndexOf("// End UI Component Events");
                var originMethodCode = originCode.Substring(methodCodeStartIndex, methodCodeEndIndex - methodCodeStartIndex);

                foreach (var method in methodCodeByMethodName)
                {
                    if (originMethodCode.Contains(method.Key)) continue;

                    var index = window.GetInsertMethodIndex(originCode);
                    originCode = originCode.Insert(index, "\n" + method.Value);
                }

                window._codeContent = originCode;
            }

            window.Show();
        }

        public void OnGUI()
        {
            _scroll = EditorGUILayout.BeginScrollView(_scroll, GUILayout.Height(720), GUILayout.Width(1000));
            EditorGUILayout.TextArea(_codeContent);
            EditorGUILayout.EndScrollView();
            EditorGUILayout.Space();

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.TextArea("Code Generate Path：" + _filePath);
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.Space();

            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Generate", GUILayout.Height(30)))
            {
                OnGenerateButtonClick();
            }

            EditorGUILayout.EndHorizontal();
        }

        private void OnGenerateButtonClick()
        {
            if (File.Exists(_filePath))
            {
                File.Delete(_filePath);
            }

            using var writer = File.CreateText(_filePath);
            writer.Write(_codeContent);
            writer.Close();
            AssetDatabase.Refresh();
            if (EditorUtility.DisplayDialog("Code Generator", "Generate Succeed！", "Confirm"))
            {
                Close();
            }
        }

        private int GetInsertMethodIndex(string content)
        {
            var regex = new Regex("// End UI Component Events");
            var match = regex.Match(content);

            var regex1          = new Regex("}");
            var matchCollection = regex1.Matches(content);

            var targetIndex = 0;

            for (var i = 0; i < matchCollection.Count; i++)
            {
                if (matchCollection[i].Index > match.Index)
                {
                    return targetIndex + 1;
                }
                else
                {
                    targetIndex = matchCollection[i].Index;
                }
            }

            return -1;
        }

        private static string ReplaceContentBetween(string original,    string contentToExtractAndReplace,
            string                                         startMarker, string endMarker)
        {
            // Find the start and end in A
            int startA = original.IndexOf(startMarker);
            int endA   = original.IndexOf(endMarker, startA + startMarker.Length);
            if (startA == -1 || endA == -1)
            {
                return original; // Markers not found in A, return A unchanged
            }

            // Find the start and end in B
            int startB = contentToExtractAndReplace.IndexOf(startMarker);
            int endB   = contentToExtractAndReplace.IndexOf(endMarker, startB + startMarker.Length);
            if (startB == -1 || endB == -1)
            {
                return original; // Markers not found in B, return A unchanged
            }

            // Extract the replacement content from B
            string replacement = contentToExtractAndReplace.Substring(startB, endB + endMarker.Length - startB);

            // Construct the new string with replaced content
            string pre  = original.Substring(0, startA);
            string post = original.Substring(endA + endMarker.Length);

            return pre + replacement + post;
        }
    }
}
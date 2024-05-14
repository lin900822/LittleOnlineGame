using System;
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
        private string  _originContent;
        private string  _filePath;
        private Vector2 _scroll;
        private Action  _generateBtnCallback;

        public static void ShowCodePreviewWindow(
            string                     content,
            string                     filePath,
            Dictionary<string, string> methodCodeByMethodName = null,
            Action                     generateBtnCallback    = null)
        {
            var window = (CodePreviewWindow)GetWindowWithRect(typeof(CodePreviewWindow),
                new Rect(100, 50, 1000, 800), true, "Code Generate Preview");
            window._codeContent = content;
            window._filePath    = filePath;

            // 腳本已存在, 只新增必要程式碼
            if (File.Exists(filePath) && methodCodeByMethodName != null)
            {
                var originCode = File.ReadAllText(filePath);
                window._originContent = originCode;

                originCode = ReplaceContentBetween(
                    originCode,
                    content,
                    "// Start UI Components Fields",
                    "// End UI Components Fields");
                originCode = ReplaceContentBetween(
                    originCode,
                    content,
                    "// Start InitUIComponent",
                    "// End InitUIComponent");

                // 新增方法
                var methodCodeStartIndex = originCode.IndexOf("// Start UI Component Events");
                var methodCodeEndIndex   = originCode.IndexOf("// End UI Component Events");
                var originMethodCode =
                    originCode.Substring(methodCodeStartIndex, methodCodeEndIndex - methodCodeStartIndex);

                foreach (var method in methodCodeByMethodName)
                {
                    if (originMethodCode.Contains(method.Key)) continue;

                    var index = window.GetInsertMethodIndex(originCode);
                    originCode = originCode.Insert(index + 2, method.Value);
                }

                window._codeContent = originCode;
            }

            window._generateBtnCallback = generateBtnCallback;

            window.Show();
        }

        public void OnGUI()
        {
            _scroll = EditorGUILayout.BeginScrollView(_scroll, GUILayout.Height(720), GUILayout.Width(1000));
            
            string coloredContent = GetColoredContent(_originContent, _codeContent);
            // Use EditorGUILayout.TextArea for displaying the result.
            GUIStyle style = new GUIStyle(EditorStyles.textArea);
            style.richText  = true;
            style.fontSize  = 14;
            EditorGUILayout.TextArea(coloredContent, style);

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
            _generateBtnCallback?.Invoke();

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
                _generateBtnCallback = null;
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

        private static string GetColoredContent(string oldContent, string newContent)
        {
            string[] oldLines = oldContent.Split('\n');
            string[] newLines = newContent.Split('\n');

            int[,]                    lcsMatrix      = BuildLCSMatrix(oldLines, newLines);
            System.Text.StringBuilder coloredContent = new System.Text.StringBuilder();

            int oldIndex = oldLines.Length;
            int newIndex = newLines.Length;

            while (oldIndex > 0 || newIndex > 0)
            {
                if (oldIndex > 0 && newIndex > 0 && oldLines[oldIndex - 1] == newLines[newIndex - 1])
                {
                    // Line is unchanged
                    coloredContent.Insert(0, $"<color=#D3D3D3>{newLines[newIndex - 1]}</color>\n");
                    oldIndex--;
                    newIndex--;
                }
                else if (newIndex > 0 &&
                         (oldIndex == 0 || lcsMatrix[oldIndex, newIndex - 1] >= lcsMatrix[oldIndex - 1, newIndex]))
                {
                    // New line is added
                    coloredContent.Insert(0, $"<color=#32CD32>{newLines[newIndex - 1]}</color>\n");
                    newIndex--;
                }
                else if (oldIndex > 0 &&
                         (newIndex == 0 || lcsMatrix[oldIndex, newIndex - 1] < lcsMatrix[oldIndex - 1, newIndex]))
                {
                    // Old line is deleted
                    coloredContent.Insert(0, $"<color=#DC143C>{oldLines[oldIndex - 1]}</color>\n");
                    oldIndex--;
                }
            }

            return coloredContent.ToString();
        }

        private static int[,] BuildLCSMatrix(string[] oldLines, string[] newLines)
        {
            int[,] lcsMatrix = new int[oldLines.Length + 1, newLines.Length + 1];

            for (int i = 1; i <= oldLines.Length; i++)
            {
                for (int j = 1; j <= newLines.Length; j++)
                {
                    if (oldLines[i - 1] == newLines[j - 1])
                    {
                        lcsMatrix[i, j] = lcsMatrix[i - 1, j - 1] + 1;
                    }
                    else
                    {
                        lcsMatrix[i, j] = Mathf.Max(lcsMatrix[i - 1, j], lcsMatrix[i, j - 1]);
                    }
                }
            }

            return lcsMatrix;
        }
    }
}
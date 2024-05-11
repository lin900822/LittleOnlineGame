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
            Dictionary<string, string> insertDic = null)
        {
            //创建代码展示窗口
            var window = (CodePreviewWindow)GetWindowWithRect(typeof(CodePreviewWindow),
                new Rect(100, 50, 1000, 800), true, "Code Generate Preview");
            window._codeContent = content;
            window._filePath    = filePath;
            //处理代码新增
            if (File.Exists(filePath) && insertDic != null)
            {
                //获取原始代码
                string originScript = File.ReadAllText(filePath);
                foreach (var item in insertDic)
                {
                    //如果老代码中没有这个代码就进行插入操作
                    if (!originScript.Contains(item.Key))
                    {
                        int index                          = window.GetInsertIndex(originScript);
                        originScript = window._codeContent = originScript.Insert(index, item.Value + "\t\t");
                    }
                }
            }

            window.Show();
        }

        public void OnGUI()
        {
            //绘制ScroView
            _scroll = EditorGUILayout.BeginScrollView(_scroll, GUILayout.Height(720), GUILayout.Width(1000));
            EditorGUILayout.TextArea(_codeContent);
            EditorGUILayout.EndScrollView();
            EditorGUILayout.Space();

            //绘制脚本生成路径
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.TextArea("Code Generate Path：" + _filePath);
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.Space();

            //绘制按钮
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Generate", GUILayout.Height(30)))
            {
                //按钮事件
                ButtonClick();
            }

            EditorGUILayout.EndHorizontal();
        }

        public void ButtonClick()
        {
            if (File.Exists(_filePath))
            {
                File.Delete(_filePath);
            }

            StreamWriter writer = File.CreateText(_filePath);
            writer.Write(_codeContent);
            writer.Close();
            AssetDatabase.Refresh();
            if (EditorUtility.DisplayDialog("Code Generator", "Generate Succeed！", "Confirm"))
            {
                Close();
            }
        }

        /// <summary>
        /// 获取插入代码的下标
        /// </summary>
        /// <param name="content"></param>
        /// <returns></returns>
        private int GetInsertIndex(string content)
        {
            //找到UI事件组件下面的第一个public 所在的位置 进行插入
            Regex regex = new Regex("- UI Component Events -");
            Match match = regex.Match(content);

            Regex           regex1          = new Regex("public");
            MatchCollection matchCollection = regex1.Matches(content);

            for (int i = 0; i < matchCollection.Count; i++)
            {
                if (matchCollection[i].Index > match.Index)
                {
                    return matchCollection[i].Index;
                }
            }

            return -1;
        }
    }
}
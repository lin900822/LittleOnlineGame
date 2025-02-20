using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace Framework.UI.Editor
{
    public class UIComponentInfo
    {
        public int    InstanceId;
        public string Name;
        public string Type;
    }

    public class GenerateWindowTool : UnityEditor.Editor
    {
        private static List<UIComponentInfo> _uiComponentInfos = new List<UIComponentInfo>();

        private static Dictionary<string, string> _methodCodeByMethodName = new Dictionary<string, string>();

        [MenuItem("/Assets/UI框架/生成Window腳本(Shift+V) #V", false, 0)]
        private static void CreateFindComponentScripts()
        {
            _uiComponentInfos.Clear();
            _methodCodeByMethodName.Clear();

            var selectedGO = Selection.objects.First() as GameObject; //获取到当前选择的物体
            if (selectedGO == null)
            {
                Debug.LogError("請選擇 GameObject");
                return;
            }

            if (!Directory.Exists(GeneratorConfig.WindowGeneratePath))
            {
                Directory.CreateDirectory(GeneratorConfig.WindowGeneratePath);
            }

            ParseNodeData(selectedGO.transform);

            var prefabPath = PrefabUtility.GetPrefabAssetPathOfNearestInstanceRoot(selectedGO);

            if (string.IsNullOrEmpty(prefabPath))
            {
                Debug.LogError($"請選擇Project視窗內的Prefab生成");
                return;
            }

            var codeContent = GenCodeContent(selectedGO.name);
            var codePath    = GeneratorConfig.WindowGeneratePath + "/" + selectedGO.name + ".cs";
            CodePreviewWindow.ShowCodePreviewWindow(codeContent, codePath, _methodCodeByMethodName,
                () => { AdjustUIComponentContainer(prefabPath, selectedGO); });
        }

        private static void AdjustUIComponentContainer(string prefabPath, GameObject selectedGO)
        {
            using (var editScope = new PrefabUtility.EditPrefabContentsScope(prefabPath))
            {
                var prefabRoot = editScope.prefabContentsRoot;

                var uiComponentContainer = prefabRoot.GetComponent<UIComponentContainer>();
                if (uiComponentContainer == null)
                {
                    uiComponentContainer = selectedGO.AddComponent<UIComponentContainer>();
                }

                uiComponentContainer.UIComponents.Clear();
                foreach (var uiComponentInfo in _uiComponentInfos)
                {
                    var uiGO = EditorUtility.InstanceIDToObject(uiComponentInfo.InstanceId) as GameObject;
                    uiComponentContainer.UIComponents.Add(uiGO);
                }
            }
        }

        private static string GenCodeContent(string name)
        {
            _methodCodeByMethodName.Clear();
            var sb = new StringBuilder();

            sb.AppendLine("/*---------------------------------");
            sb.AppendLine(" - Title: UI腳本生成工具");
            sb.AppendLine(" - Created Date: " + DateTime.Now);
            sb.AppendLine(" - 注意事項:");
            sb.AppendLine(" - 1. 請不要刪除或修改 \"// Start XXX\" 和 \"// End XXX\" 等相關的註解, 自動生成器會依賴他們");
            sb.AppendLine(" - 2. 請不要在 \"Start UI Components Fields\" 和 \"End UI Components Fields\" 之間加入新的程式碼");
            sb.AppendLine(" - 3. 請不要在 \"Start Start InitUIComponent\" 和 \"End Start InitUIComponent\" 之間加入新的程式碼");
            sb.AppendLine("---------------------------------*/");
            sb.AppendLine("using UnityEngine.UI;");
            sb.AppendLine("using UnityEngine;");
            sb.AppendLine("using Framework.UI;");
            sb.AppendLine();

            sb.AppendLine($"namespace Game.UI");
            sb.AppendLine("{");

            sb.AppendLine($"    public class {name} : WindowBase");
            sb.AppendLine("    {");

            // UI Components Fields
            sb.AppendLine($"        #region - UI Components Fields -");
            sb.AppendLine($"");
            sb.AppendLine($"        // Start UI Components Fields");
            foreach (var item in _uiComponentInfos)
            {
                sb.AppendLine($"        private {item.Type} {item.Name}{item.Type};");
            }

            sb.AppendLine($"        // End UI Components Fields");
            sb.AppendLine($"");
            sb.AppendLine($"        #endregion");

            // Life Cycle
            sb.AppendLine("");
            sb.AppendLine($"        #region - Life Cycle -");
            sb.AppendLine($"");
            // OnLoaded
            sb.AppendLine("        public override void OnLoaded()");
            sb.AppendLine("        {");
            sb.AppendLine("            base.OnLoaded();");
            sb.AppendLine("            InitUIComponent();");
            sb.AppendLine("        }");
            sb.AppendLine("");

            // InitUIComponent
            sb.AppendLine("        private void InitUIComponent()");
            sb.AppendLine("        {");
            sb.AppendLine("            // Start InitUIComponent");
            for (var i = 0; i < _uiComponentInfos.Count; i++)
            {
                var node     = _uiComponentInfos[i];
                var fullName = node.Name + node.Type;

                if (string.Equals("GameObject", node.Type))
                {
                    sb.AppendLine($"            {fullName} = UIComponentContainer[{i}].gameObject;");
                }
                else if (string.Equals("Transform", node.Type))
                {
                    sb.AppendLine($"            {fullName} = UIComponentContainer[{i}].transform;");
                }
                else
                {
                    sb.AppendLine($"            {fullName} = UIComponentContainer[{i}].GetComponent<{node.Type}>();");
                }
            }

            foreach (var node in _uiComponentInfos)
            {
                var type       = node.Type;
                var methodName = node.Name;
                var suffix     = "";
                if (type.Contains("Button"))
                {
                    suffix = "Click";
                    sb.AppendLine(
                        $"            AddButtonClickListener({methodName}{type}, On{methodName}Button{suffix});");
                }

                if (type.Contains("InputField"))
                {
                    sb.AppendLine(
                        $"            AddInputFieldListener({methodName}{type}, On{methodName}InputChange, On{methodName}InputEnd);");
                }

                if (type.Contains("Toggle"))
                {
                    suffix = "Change";
                    sb.AppendLine(
                        $"            AddToggleClickListener({methodName}{type}, On{methodName}Toggle{suffix});");
                }
            }

            sb.AppendLine("            // End InitUIComponent");
            sb.AppendLine("        }");
            sb.AppendLine("");

            // OnShow
            sb.AppendLine("        public override void OnShow(UIData uiData = null)");
            sb.AppendLine("        {");
            sb.AppendLine("            base.OnShow(uiData);");
            sb.AppendLine("        }");
            sb.AppendLine("");

            // OnHide
            sb.AppendLine("        public override void OnHide()");
            sb.AppendLine("        {");
            sb.AppendLine("            base.OnHide();");
            sb.AppendLine("        }");
            sb.AppendLine("");

            // OnUnloaded
            sb.AppendLine("        public override void OnUnloaded()");
            sb.AppendLine("        {");
            sb.AppendLine("            base.OnUnloaded();");
            sb.AppendLine("        }");
            sb.AppendLine("");

            sb.AppendLine($"        #endregion");
            sb.AppendLine($"");

            // UI Component Events 
            sb.AppendLine($"        #region - UI Component Events -");
            sb.AppendLine($"        ");
            sb.AppendLine($"        // Start UI Component Events");
            sb.AppendLine($"");
            foreach (var item in _uiComponentInfos)
            {
                var type       = item.Type;
                var methodName = "On" + item.Name;
                var suffix     = "";
                if (type.Contains("Button"))
                {
                    suffix = "ButtonClick";
                    GenMethod(sb, methodName + suffix);
                }
                else if (type.Contains("InputField"))
                {
                    suffix = "InputChange";
                    GenMethod(sb, methodName + suffix, "string text");
                    suffix = "InputEnd";
                    GenMethod(sb, methodName + suffix, "string text");
                }
                else if (type.Contains("Toggle"))
                {
                    suffix = "ToggleChange";
                    GenMethod(sb, methodName + suffix, "bool state,Toggle toggle");
                }
            }

            sb.AppendLine($"        // End UI Component Events");
            sb.AppendLine($"        ");
            sb.AppendLine($"        #endregion");

            // Custom Logic
            sb.AppendLine($"");
            sb.AppendLine($"        #region - Custom Logic -");
            sb.AppendLine($"        ");
            sb.AppendLine($"        ");
            sb.AppendLine($"        ");
            sb.AppendLine($"        #endregion");

            sb.AppendLine("    }");
            sb.AppendLine("}");
            return sb.ToString();
        }

        private static void GenMethod(StringBuilder sb, string methodName, string param = "")
        {
            sb.AppendLine($"        private void {methodName}({param})");
            sb.AppendLine("        {");
            if (methodName == "OnCloseButtonClick")
            {
                sb.AppendLine("            HideWindow();");
            }
            else
            {
                sb.AppendLine("            ");
            }

            sb.AppendLine("        }");
            sb.AppendLine("");

            // 用於插入已存在的腳本使用
            var builder = new StringBuilder();
            builder.AppendLine($"        private void {methodName}({param})");
            builder.AppendLine("        {");
            builder.AppendLine("            ");
            builder.AppendLine("        }");
            builder.AppendLine("");
            _methodCodeByMethodName.Add(methodName, builder.ToString());
        }

        private static void ParseNodeData(Transform trans)
        {
            for (var i = 0; i < trans.childCount; i++)
            {
                var go     = trans.GetChild(i).gameObject;
                var goName = go.name;
                if (goName.Contains("[") && goName.Contains("]"))
                {
                    var index = goName.IndexOf("]") + 1;
                    var name  = goName.Substring(index, goName.Length - index);
                    var type  = goName.Substring(1,     index - 2);

                    name = name.Replace(" ", "");

                    _uiComponentInfos.Add(new UIComponentInfo()
                    {
                        Name       = name,
                        Type       = type,
                        InstanceId = go.GetInstanceID()
                    });
                }

                ParseNodeData(trans.GetChild(i));
            }
        }
    }
}
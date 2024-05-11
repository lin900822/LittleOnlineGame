using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace Framework.ZMUI.Editor
{
    public class EditorObjectData
    {
        public int    InstanceId;
        public string FieldName;
        public string FieldType;
    }

    public class GenerateWindowTool : UnityEditor.Editor
    {
        public static Dictionary<int, string>
            objFindPathDic = new Dictionary<int, string>(); //key 物体的insid，value 代表物体的查找路径

        public static List<EditorObjectData> objDataList = new List<EditorObjectData>(); //查找对象的数据

        static Dictionary<string, string> methodDic = new Dictionary<string, string>();

        [MenuItem("/GameObject/[UI腳本生成工具]/生成Window腳本(Shift+V) #V", false, 0)]
        static void CreateFindComponentScripts()
        {
            objFindPathDic.Clear();
            objDataList.Clear();
            methodDic.Clear();

            GameObject obj = Selection.objects.First() as GameObject; //获取到当前选择的物体
            if (obj == null)
            {
                Debug.LogError("請選擇 GameObject");
                return;
            }

            if (!Directory.Exists(GeneratorConfig.WindowGeneratePath))
            {
                Directory.CreateDirectory(GeneratorConfig.WindowGeneratePath);
            }

            PresWindowNodeData(obj.transform, obj.name);

            //生成CS脚本
            string codeContent = GenCodeContent(obj.name);
            Debug.Log(codeContent);
            string codePath    = GeneratorConfig.WindowGeneratePath + "/" + obj.name + ".cs";
            CodePreviewWindow.ShowCodePreviewWindow(codeContent, codePath, methodDic);
        }

        /// <summary>
        /// 生成Window脚本
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static string GenCodeContent(string name)
        {
            methodDic.Clear();
            StringBuilder sb = new StringBuilder();

            //添加引用
            sb.AppendLine("/*---------------------------------");
            sb.AppendLine(" - Title: UI腳本生成工具");
            sb.AppendLine(" - Date: " + System.DateTime.Now);
            sb.AppendLine(" - Description:UI 表现层，该层只负责界面的交互、表现相关的更新，不允许编写任何业务逻辑代码");
            sb.AppendLine(" - 注意:以下文件是自动生成的，再次生成不会覆盖原有的代码，会在原有的代码上进行新增，可放心使用");
            sb.AppendLine("---------------------------------*/");
            sb.AppendLine("using UnityEngine.UI;");
            sb.AppendLine("using UnityEngine;");
            sb.AppendLine("using Framework.ZMUI;");
            sb.AppendLine();

            sb.AppendLine($"namespace Game.ZMUI");
            sb.AppendLine("{");
            //生成类命
            sb.AppendLine($"\tpublic class {name} : WindowBase");
            sb.AppendLine("\t{");
            sb.AppendLine("\t");

            // UI Components Fields
            sb.AppendLine($"\t\t #region - UI Components Fields -");
            sb.AppendLine($"");
            foreach (var item in objDataList)
            {
                sb.AppendLine($"\t\t private {item.FieldType} {item.FieldName}{item.FieldType};");
            }

            sb.AppendLine($"\t");
            sb.AppendLine($"\t\t #endregion");

            // Life Cycle
            sb.AppendLine("");
            sb.AppendLine($"\t\t #region - Life Cycle -");
            sb.AppendLine($"\t");
            // OnLoaded
            sb.AppendLine("\t\t public override void OnLoaded()");
            sb.AppendLine("\t\t {");
            sb.AppendLine("\t\t\t base.OnLoaded();");
            foreach (var item in objFindPathDic)
            {
                EditorObjectData itemData  = GetEditorObjectData(item.Key);
                string           fieldName = itemData.FieldName + itemData.FieldType;

                if (string.Equals("GameObject", itemData.FieldType))
                {
                    sb.AppendLine($"\t\t\t {fieldName} = transform.Find(\"{item.Value}\").gameObject;");
                }
                else if (string.Equals("Transform", itemData.FieldType))
                {
                    sb.AppendLine($"\t\t\t {fieldName} = transform.Find(\"{item.Value}\").transform;");
                }
                else
                {
                    sb.AppendLine($"\t\t\t {fieldName} = transform.Find(\"{item.Value}\").GetComponent<{itemData.FieldType}>();");
                }
            }
            
            foreach (var item in objDataList)
            {
                string type       = item.FieldType;
                string methodName = item.FieldName;
                string suffix     = "";
                if (type.Contains("Button"))
                {
                    suffix = "Click";
                    sb.AppendLine(
                        $"\t\t\t AddButtonClickListener({methodName}{type}, On{methodName}Button{suffix});");
                }

                if (type.Contains("InputField"))
                {
                    sb.AppendLine(
                        $"\t\t\t target.AddInputFieldListener({methodName}{type}, On{methodName}InputChange,mWindow.On{methodName}InputEnd);");
                }

                if (type.Contains("Toggle"))
                {
                    suffix = "Change";
                    sb.AppendLine(
                        $"\t\t\t target.AddToggleClickListener({methodName}{type}, On{methodName}Toggle{suffix});");
                }
            }
            
            sb.AppendLine("\t\t }");
            sb.AppendLine("\t");
            
            // OnShow
            sb.AppendLine("\t\t public override void OnShow()");
            sb.AppendLine("\t\t {");
            sb.AppendLine("\t\t\t base.OnShow();");
            sb.AppendLine("\t\t }");
            sb.AppendLine("\t");
            
            // OnHide
            sb.AppendLine("\t\t public override void OnHide()");
            sb.AppendLine("\t\t {");
            sb.AppendLine("\t\t\t base.OnHide();");
            sb.AppendLine("\t\t }");
            sb.AppendLine("\t");

            // OnUnloaded
            sb.AppendLine("\t\t public override void OnUnloaded()");
            sb.AppendLine("\t\t {");
            sb.AppendLine("\t\t\t base.OnUnloaded();");
            sb.AppendLine("\t\t }");
            sb.AppendLine("\t");

            sb.AppendLine($"\t\t #endregion");

            // API Function 
            sb.AppendLine($"\t");
            sb.AppendLine($"\t\t #region API Function");
            sb.AppendLine($"\n");
            sb.AppendLine($"\t\t    ");
            sb.AppendLine($"\t\t #endregion");
            sb.AppendLine($"\t");

            // UI Component Events 
            sb.AppendLine($"\t\t #region - UI Component Events -");
            sb.AppendLine($"\t");
            foreach (var item in objDataList)
            {
                string type       = item.FieldType;
                string methodName = "On" + item.FieldName;
                string suffix     = "";
                if (type.Contains("Button"))
                {
                    suffix = "ButtonClick";
                    CreateMethod(sb, ref methodDic, methodName + suffix);
                }
                else if (type.Contains("InputField"))
                {
                    suffix = "InputChange";
                    CreateMethod(sb, ref methodDic, methodName + suffix, "string text");
                    suffix = "InputEnd";
                    CreateMethod(sb, ref methodDic, methodName + suffix, "string text");
                }
                else if (type.Contains("Toggle"))
                {
                    suffix = "ToggleChange";
                    CreateMethod(sb, ref methodDic, methodName + suffix, "bool state,Toggle toggle");
                }
            }

            sb.AppendLine($"\t\t #endregion");

            sb.AppendLine("\t}");
            sb.AppendLine("}");
            return sb.ToString();
        }

        /// <summary>
        /// 生成UI事件方法
        /// </summary>
        /// <param name="sb"></param>
        /// <param name="methodDic"></param>
        /// <param name="modthName"></param>
        /// <param name="param"></param>
        public static void CreateMethod(
            StringBuilder                  sb,
            ref Dictionary<string, string> methodDic,
            string                         methodName,
            string                         param = "")
        {
            //声明UI组件事件
            sb.AppendLine($"\t\t private void {methodName}({param})");
            sb.AppendLine("\t\t {");
            sb.AppendLine("\t\t");
            if (methodName == "OnCloseButtonClick")
            {
                sb.AppendLine("\t\t\tHideWindow();");
            }

            sb.AppendLine("\t\t }");
            sb.AppendLine("\t");

            //存储UI组件事件 提供给后续新增代码使用
            StringBuilder builder = new StringBuilder();
            builder.AppendLine($"\t\t private void {methodName}({param})");
            builder.AppendLine("\t\t {");
            builder.AppendLine("\t\t");
            builder.AppendLine("\t\t }");
            methodDic.Add(methodName, builder.ToString());
        }

        public static void PresWindowNodeData(Transform trans, string WinName)
        {
            for (int i = 0; i < trans.childCount; i++)
            {
                GameObject obj  = trans.GetChild(i).gameObject;
                string     name = obj.name;
                if (name.Contains("[") && name.Contains("]"))
                {
                    int    index     = name.IndexOf("]") + 1;
                    string fieldName = name.Substring(index, name.Length - index); //获取字段昵称
                    string fieldType = name.Substring(1, index - 2);               //获取字段类型

                    objDataList.Add(new EditorObjectData
                    {
                        FieldName  = fieldName,
                        FieldType  = fieldType,
                        InstanceId = obj.GetInstanceID()
                    });

                    //计算该节点的查找路径
                    string    objPath    = name; //UIContent/[Button]Close
                    bool      isFindOver = false;
                    Transform parent     = obj.transform;
                    for (int k = 0; k < 20; k++)
                    {
                        for (int j = 0; j <= k; j++)
                        {
                            if (k == j)
                            {
                                parent = parent.parent;
                                //如果父节点是当前窗口，说明查找已经结束
                                if (string.Equals(parent.name, WinName))
                                {
                                    isFindOver = true;
                                    break;
                                }
                                else
                                {
                                    objPath = objPath.Insert(0, parent.name + "/");
                                }
                            }
                        }

                        if (isFindOver)
                        {
                            break;
                        }
                    }

                    objFindPathDic.Add(obj.GetInstanceID(), objPath);
                }

                PresWindowNodeData(trans.GetChild(i), WinName);
            }
        }

        public static EditorObjectData GetEditorObjectData(int instanceId)
        {
            foreach (var item in objDataList)
            {
                if (item.InstanceId == instanceId)
                {
                    return item;
                }
            }

            return null;
        }
    }
}
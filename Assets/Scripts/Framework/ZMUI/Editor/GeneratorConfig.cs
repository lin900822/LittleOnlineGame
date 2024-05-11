using UnityEngine;

namespace Framework.ZMUI.Editor
{
    public enum GenerateType
    {
        Find, //组件查找
        Bind, //组件绑定
    }
    public enum ParseType
    {
        Name,
        Tag
    }
    
    public static class GeneratorConfig
    {
        public static string BindComponentGeneratorPath = Application.dataPath + "/ZMUIFrameWork/Scripts/BindCompoent";
        public static string FindComponentGeneratorPath = Application.dataPath + "/ZMUIFrameWork/Scripts/FindCompoent";
        public static string WindowGeneratePath = Application.dataPath + "/Scripts/Game/ZMUI";
        public static string OBJDATALIST_KEY = "objDataList";
        public static GenerateType GenerateType = GenerateType.Bind;
        public static ParseType ParseType = ParseType.Name;
        public static string[] TAGArr = { "Image", "RawImage", "Text", "Button", "Slider", "Dropdown", "InputField", "Canvas", "Panel", "ScrollRect" ,"Toggle"};
    }
}
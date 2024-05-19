using UnityEngine;

namespace Framework.Common
{
    public class InspectorReadOnlyAttribute : PropertyAttribute { }
    
    public class TitleAttribute : PropertyAttribute
    {
        public string Title;
        public TitleAttribute(string title)
        {
            Title = title;
        }
    }
    
    public class InfoAttribute : PropertyAttribute
    {
        public string Info;
        public InfoAttribute(string info)
        {
            Info = info;
        }
    }
}
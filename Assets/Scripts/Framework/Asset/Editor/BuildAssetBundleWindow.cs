using Sirenix.OdinInspector.Editor;
using Sirenix.Utilities;
using Sirenix.Utilities.Editor;
using UnityEditor;

namespace Framework.Editor
{
    public class BuildAssetBundleWindow : OdinMenuEditorWindow
    {
        [MenuItem("熱更工具/AssetBundleWindow")]
        public static void ShowAssetBundleWindow()
        {
            BuildAssetBundleWindow assetBundleWindow = GetWindow<BuildAssetBundleWindow>();
            assetBundleWindow.position = GUIHelper.GetEditorWindowRect().AlignCenter(985, 612);
            assetBundleWindow.ForceMenuTreeRebuild();
        }

        protected override OdinMenuTree BuildMenuTree()
        {
            OdinMenuTree menuTree = new OdinMenuTree(supportsMultiSelect: false)
            {
                { "Build",null,EditorIcons.House},
            };
            return menuTree;
        }
    }

}

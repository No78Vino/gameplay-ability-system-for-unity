using Sirenix.OdinInspector.Editor;
using Sirenix.Utilities.Editor;
using Sirenix.Utilities;
using UnityEditor;
using UnityEngine;

public class OdinMenuWindowExample : OdinMenuEditorWindow
{
    [MenuItem("Test Window/My Odin Menu Window")]
    private static void OpenWindow()
    {
        var window = GetWindow<OdinMenuWindowExample>();
        window.position = GUIHelper.GetEditorWindowRect().AlignCenter(800, 500);
    }

    protected override OdinMenuTree BuildMenuTree()
    {
        var tree = new OdinMenuTree();

        var myAssetPath = "Assets";
        
        tree.AddAllAssetsAtPath("Assets", myAssetPath, typeof(ScriptableObject), true, false)
            .AddThumbnailIcons();

        tree.SortMenuItemsByName();
        tree.Config.DrawSearchToolbar = true;
        tree.Config.SearchToolbarHeight = 30;
        tree.Config.AutoHandleKeyboardNavigation = true;

        return tree;
    }
}
using System;
using System.Reflection;
using Sirenix.OdinInspector.Editor;
using Sirenix.Utilities;
using Sirenix.Utilities.Editor;
using UnityEditor;
using UnityEngine;
using UnityEngine.Timeline;

[CustomEditor(typeof(Animator))]
public class TimelineEditor : OdinMenuEditorWindow
{
    // [MenuItem("Test/Timeline", priority = 1)]
    // private static void OpenWindow()
    // {
    //     var window = GetWindow<TimelineEditor>();
    //     window.position = GUIHelper.GetEditorWindowRect().AlignCenter(900, 600);
    // }
    
    protected override OdinMenuTree BuildMenuTree()
    {
        var tree = new OdinMenuTree();
       

        tree.AddAllAssetsAtPath("Timeline", "Assets", typeof(TimelineAsset), true)
                .AddThumbnailIcons();
            

        tree.Config.DrawSearchToolbar = true;
        tree.Config.SearchToolbarHeight = 30;
        tree.Config.AutoScrollOnSelectionChanged = true;
        tree.Config.DrawScrollView = true;
        tree.Config.AutoHandleKeyboardNavigation = true;
        tree.SortMenuItemsByName(true);
            
        return tree;
    }
    
    // [MenuItem("Test/Open Timeline", priority = 1)]
    // public static void OpenTimelineWindow()
    // {
    //     var window = UnityEditor.Timeline.TimelineEditor.GetOrCreateWindow();
    //     var inspectorWindow = GetInspectTarget();
    //     inspectorWindow.Show();
    //
    //     EditorApplication.delayCall +=
    //         () => window.DockWindow(inspectorWindow, DockUtilities.DockPosition.Right);
    // }
    //
    
}
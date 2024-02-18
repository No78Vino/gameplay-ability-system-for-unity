using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using GAS.General;
using GAS.General.AbilityTimeline;
using UnityEditor;
using UnityEngine;

public class MoreInspector : EditorWindow
{
    private static MoreInspector _window;
    private EditorWindow inspectorWindow;
    private static readonly Vector2 MIN_SIE = new Vector2(400, 300);

    [MenuItem("Test/额外Inspector面板", priority = 43)]
    private static void PopUp()
    {
        _window = GetWindow<MoreInspector>("额外Inspector面板");
        _window.minSize = MIN_SIE;
        _window.Show();
    }

    private void OnGUI()
    {
        if (GUILayout.Button("显示额外Inspector面板"))
        {
            inspectorWindow = GetInspectTarget();
            inspectorWindow.Show();
            DockUtilities.DockWindow(this, inspectorWindow, DockUtilities.DockPosition.Right);
        }
    }

    public EditorWindow GetInspectTarget()
    {
        // Get Unity Internal Objects
        Type inspectorType = typeof(Editor).Assembly.GetType("UnityEditor.InspectorWindow");
        // Create an inspector window Instance
        EditorWindow inspectorInstance = ScriptableObject.CreateInstance(inspectorType) as EditorWindow;
        // We display it - currently, it will inspect whatever gameObject is currently selected
        // So we need to find a way to let it inspect/aim at our target GO that we passed

        // 1. Cache the current selected gameObject
        UnityEngine.Object prevSelection = Selection.activeObject;
        // 2. Set the current selection to our target GO
        // 3. Get a ref to the "locked" property, which will lock the state of the inspector to the current inspected target
        var isLocked = inspectorType.GetProperty("isLocked", BindingFlags.Instance | BindingFlags.Public);
        // 4. Invoke 'isLocked' setter method passing "true" to lock the inspector
        isLocked.GetSetMethod().Invoke(inspectorInstance, new object[] {true});
        // 5. Finally revert back to the previous selection so that other inspector will continue to inspector whatever they were inspecting
        Selection.activeObject = prevSelection;

        return inspectorInstance;
    }
}
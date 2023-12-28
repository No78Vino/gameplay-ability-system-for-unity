using System.Collections.Generic;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

public struct MyStruct
{
    public string name;
    public int value;
}

public class MyStructEditor : EditorWindow
{
    private List<MyStruct> myStructList = new List<MyStruct>();
    private ReorderableList reorderableList;

    private void OnEnable()
    {
        reorderableList = new ReorderableList(myStructList, typeof(MyStruct), true, true, true, true);
        reorderableList.drawElementCallback += DrawListElement;
        reorderableList.drawHeaderCallback += DrawListHeader;
        reorderableList.elementHeightCallback += GetElementHeight;
        reorderableList.onAddCallback += AddListItem;
    }

    private void OnGUI()
    {
        reorderableList.DoLayoutList();
    }

    private void DrawListHeader(Rect rect)
    {
        EditorGUI.LabelField(rect, "MyStruct List");
    }

    private void DrawListElement(Rect rect, int index, bool isActive, bool isFocused)
    {
        var element = myStructList[index];

        rect.y += 2;
        rect.height = EditorGUIUtility.singleLineHeight;

        EditorGUI.LabelField(new Rect(rect.x, rect.y, 50, EditorGUIUtility.singleLineHeight), "Name:");
        element.name = EditorGUI.TextField(new Rect(rect.x + 60, rect.y, 120, EditorGUIUtility.singleLineHeight), element.name);

        if (isActive)
        {
            EditorGUI.LabelField(new Rect(rect.x, rect.y + EditorGUIUtility.singleLineHeight + 5, 50, EditorGUIUtility.singleLineHeight), "Value:");
            element.value = EditorGUI.IntField(new Rect(rect.x + 60, rect.y + EditorGUIUtility.singleLineHeight + 5, 120, EditorGUIUtility.singleLineHeight), element.value);
        }

        myStructList[index] = element;

        // 删除按钮
        if (GUI.Button(new Rect(rect.x + rect.width - 60, rect.y, 30, EditorGUIUtility.singleLineHeight), "X"))
        {
            myStructList.RemoveAt(index);
        }

        // 编辑按钮
        if (GUI.Button(new Rect(rect.x + rect.width - 25, rect.y, 20, EditorGUIUtility.singleLineHeight), "E"))
        {
            ShowEditWindow(index);
        }
    }

    private float GetElementHeight(int index)
    {
        return reorderableList.index == index ? EditorGUIUtility.singleLineHeight * 2 + 5 : EditorGUIUtility.singleLineHeight;
    }

    private void AddListItem(ReorderableList list)
    {
        myStructList.Add(new MyStruct());
    }

    private void ShowEditWindow(int index)
    {
        // 实现弹出窗口的编辑功能
        Debug.Log("Editing item at index: " + index);
    }

    [MenuItem("Test Window/MyStruct Editor")]
    public static void ShowWindow()
    {
        GetWindow<MyStructEditor>("MyStruct Editor");
    }
}

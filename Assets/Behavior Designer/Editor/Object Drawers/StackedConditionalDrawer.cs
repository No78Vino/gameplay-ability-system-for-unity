using UnityEngine;
using UnityEditor;
using UnityEditorInternal;
using BehaviorDesigner.Runtime.Tasks;
using System;
using System.Reflection;

namespace BehaviorDesigner.Editor.ObjectDrawers
{
    [CustomObjectDrawer(typeof(StackedConditional))]
    public class StackedConditionalDrawer : ObjectDrawer
    {
        private ReorderableList reorderableList;
        private StackedConditional lastStackedConditional;

        public override void OnGUI(GUIContent label)
        {
            var stackedConditional = task as StackedConditional;

            stackedConditional.comparisonType = (StackedConditional.ComparisonType)FieldInspector.DrawField(stackedConditional,
                new GUIContent("Comparison Type", "Specifies if the tasks should be traversed with an AND (Sequence) or an OR (Selector)."),
                stackedConditional.GetType().GetField("comparisonType", BindingFlags.Instance | BindingFlags.Public),
                stackedConditional.comparisonType);

            stackedConditional.graphLabel = (bool)FieldInspector.DrawField(stackedConditional,
                new GUIContent("Graph Label", "Should the tasks be labeled within te graph?"),
                stackedConditional.GetType().GetField("graphLabel", BindingFlags.Instance | BindingFlags.Public),
                stackedConditional.graphLabel);

            if (stackedConditional.conditionals == null) {
                stackedConditional.conditionals = new Conditional[0];
            }

            if (reorderableList == null) {
                reorderableList = new ReorderableList(stackedConditional.conditionals, typeof(Conditional), true, true, true, true);
                reorderableList.drawHeaderCallback += (Rect rect) =>
                {
                    EditorGUI.LabelField(rect, "Conditionals");
                };
                reorderableList.onAddDropdownCallback += OnAddDropdownCallback;
                reorderableList.drawElementCallback += OnDrawElementCallback;
                reorderableList.onReorderCallback += OnReorderCallback;
                reorderableList.onSelectCallback += OnSelectCallback;
                reorderableList.onCanRemoveCallback += OnCanRemoveCallback;
                reorderableList.onRemoveCallback += OnRemoveCallback;
            }
            if (stackedConditional != lastStackedConditional) {
                lastStackedConditional = stackedConditional;
                var index = EditorPrefs.GetInt("BehaviorDesigner.StackedConditional." + stackedConditional.ID, -1);
                if (index < stackedConditional.conditionals.Length) {
                    reorderableList.index = index;
                }
            }
            if (reorderableList.index == -1 && stackedConditional.conditionals.Length > 0) {
                reorderableList.index = 0;
            }
            reorderableList.DoLayoutList();

            if (reorderableList.index >= 0 && stackedConditional.conditionals != null && reorderableList.index < stackedConditional.conditionals.Length) {
                var selectedConditional = stackedConditional.conditionals[reorderableList.index];
                EditorGUILayout.LabelField(selectedConditional.GetType().Name, BehaviorDesignerUtility.BoldLabelGUIStyle);
                FieldInspector.DrawFields(selectedConditional, selectedConditional);
            }
        }

        private void OnAddDropdownCallback(Rect buttonRect, ReorderableList list)
        {
            var addMenu = new GenericMenu();
            BehaviorDesignerWindow.instance.TaskList.AddTaskTypesToMenu(2, ref addMenu, null, typeof(StackedConditional), string.Empty, false, OnAddTask);
            addMenu.ShowAsContext();
        }

        private void OnAddTask(object obj)
        {
            var stackedConditional = task as StackedConditional;
            var conditionals = stackedConditional.conditionals;
            Array.Resize(ref conditionals, conditionals.Length + 1);
            var taskType = obj as Type;
            conditionals[conditionals.Length - 1] = Activator.CreateInstance(taskType) as Conditional;
            reorderableList.list = stackedConditional.conditionals = conditionals;
            reorderableList.index = conditionals.Length - 1;
            BehaviorDesignerWindow.instance.SaveBehavior();
        }

        private void OnDrawElementCallback(Rect rect, int index, bool isActive, bool isFocused)
        {
            var stackedConditional = task as StackedConditional;
            if (stackedConditional.conditionals == null || index >= stackedConditional.conditionals.Length || stackedConditional.conditionals[index] == null) {
                if (stackedConditional.conditionals != null && index < stackedConditional.conditionals.Length) {
                    var conditionals = stackedConditional.conditionals;
                    ArrayUtility.RemoveAt(ref conditionals, index);
                    reorderableList.list = stackedConditional.conditionals = conditionals;
                    BehaviorDesignerWindow.instance.SaveBehavior();
                }
                return;
            }
            EditorGUI.LabelField(rect, stackedConditional.conditionals[index].GetType().Name);
            if (stackedConditional.conditionals[index].NodeData == null || stackedConditional.NodeData == null || !Application.isPlaying) {
                return;
            }

            if (stackedConditional.conditionals[index].NodeData.ExecutionStatus == TaskStatus.Success || stackedConditional.conditionals[index].NodeData.ExecutionStatus == TaskStatus.Failure) {
                Texture2D texture;
                if (stackedConditional.NodeData.IsReevaluating) {
                    texture = stackedConditional.conditionals[index].NodeData.ExecutionStatus == TaskStatus.Failure ? BehaviorDesignerUtility.ExecutionFailureRepeatTexture : BehaviorDesignerUtility.ExecutionSuccessRepeatTexture;
                } else {
                    texture = stackedConditional.conditionals[index].NodeData.ExecutionStatus == TaskStatus.Failure ? BehaviorDesignerUtility.ExecutionFailureTexture : BehaviorDesignerUtility.ExecutionSuccessTexture;
                }
                rect.x = rect.width + 8;
                rect.width = rect.height = 16;
                GUI.DrawTexture(rect, texture);
            }
        }

        private void OnReorderCallback(ReorderableList list)
        {
            var stackedConditionals = task as StackedConditional;
            stackedConditionals.conditionals = (Conditional[])list.list;
            BehaviorDesignerWindow.instance.SaveBehavior();
        }

        private void OnSelectCallback(ReorderableList list)
        {
            EditorPrefs.SetInt("BehaviorDesigner.StackedConditional." + task.ID, list.index);
        }

        private bool OnCanRemoveCallback(ReorderableList list)
        {
            var stackedConditionals = task as StackedConditional;
            return stackedConditionals.conditionals != null && stackedConditionals.conditionals.Length > 0;
        }

        private void OnRemoveCallback(ReorderableList list)
        {
            var stackedConditional = task as StackedConditional;
            var conditionals = stackedConditional.conditionals;
            ArrayUtility.RemoveAt(ref conditionals, list.index);
            reorderableList.list = stackedConditional.conditionals = conditionals;
            BehaviorDesignerWindow.instance.SaveBehavior();

            reorderableList.index -= 1;
        }
    }
}
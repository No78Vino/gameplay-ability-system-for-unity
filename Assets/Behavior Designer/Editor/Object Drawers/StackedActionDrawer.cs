using UnityEngine;
using UnityEditor;
using UnityEditorInternal;
using BehaviorDesigner.Runtime.Tasks;
using System;
using System.Reflection;

using Action = BehaviorDesigner.Runtime.Tasks.Action;

namespace BehaviorDesigner.Editor.ObjectDrawers
{
    [CustomObjectDrawer(typeof(StackedAction))]
    public class StackedActionDrawer : ObjectDrawer
    {
        private ReorderableList reorderableList;
        private StackedAction lastStackedAction;

        public override void OnGUI(GUIContent label)
        {
            var stackedAction = task as StackedAction;

            stackedAction.comparisonType = (StackedAction.ComparisonType)FieldInspector.DrawField(stackedAction,
                new GUIContent("Comparison Type", "Specifies if the tasks should be traversed with an AND (Sequence) or an OR (Selector)."),
                stackedAction.GetType().GetField("comparisonType", BindingFlags.Instance | BindingFlags.Public),
                stackedAction.comparisonType);

            stackedAction.graphLabel = (bool)FieldInspector.DrawField(stackedAction,
                new GUIContent("Graph Label", "Should the tasks be labeled within te graph?"),
                stackedAction.GetType().GetField("graphLabel", BindingFlags.Instance | BindingFlags.Public),
                stackedAction.graphLabel);

            if (stackedAction.actions == null) {
                stackedAction.actions = new Action[0];
            }

            if (reorderableList == null) {
                reorderableList = new ReorderableList(stackedAction.actions, typeof(Action), true, true, true, true);
                reorderableList.drawHeaderCallback += (Rect rect) =>
                {
                    EditorGUI.LabelField(rect, "Actions");
                };
                reorderableList.onAddDropdownCallback += OnAddDropdownCallback;
                reorderableList.drawElementCallback += OnDrawElementCallback;
                reorderableList.onReorderCallback += OnReorderCallback;
                reorderableList.onSelectCallback += OnSelectCallback;
                reorderableList.onCanRemoveCallback += OnCanRemoveCallback;
                reorderableList.onRemoveCallback += OnRemoveCallback;
            }
            if (stackedAction != lastStackedAction) {
                lastStackedAction = stackedAction;
                var index = EditorPrefs.GetInt("BehaviorDesigner.StackedAction." + stackedAction.ID, -1);
                if (index < stackedAction.actions.Length) {
                    reorderableList.index = index;
                }
            }
            if (reorderableList.index == -1 && stackedAction.actions.Length > 0) {
                reorderableList.index = 0;
            }
            reorderableList.DoLayoutList();

            if (reorderableList.index >= 0 && stackedAction.actions != null && reorderableList.index < stackedAction.actions.Length) {
                var selectedAction = stackedAction.actions[reorderableList.index];
                EditorGUILayout.LabelField(selectedAction.GetType().Name, BehaviorDesignerUtility.BoldLabelGUIStyle);
                FieldInspector.DrawFields(selectedAction, selectedAction);
            }
        }

        private void OnAddDropdownCallback(Rect buttonRect, ReorderableList list)
        {
            var addMenu = new GenericMenu();
            BehaviorDesignerWindow.instance.TaskList.AddTaskTypesToMenu(0, ref addMenu, null, typeof(StackedAction), string.Empty, false, OnAddTask);
            addMenu.ShowAsContext();
        }

        private void OnAddTask(object obj)
        {
            var stackedAction = task as StackedAction;
            var actions = stackedAction.actions;
            Array.Resize(ref actions, actions.Length + 1);
            var taskType = obj as Type;
            actions[actions.Length - 1] = Activator.CreateInstance(taskType) as Action;
            reorderableList.list = stackedAction.actions = actions;
            reorderableList.index = actions.Length - 1;
            BehaviorDesignerWindow.instance.SaveBehavior();
        }

        private void OnDrawElementCallback(Rect rect, int index, bool isActive, bool isFocused)
        {
            var stackedAction = task as StackedAction;
            if (stackedAction.actions == null || index >= stackedAction.actions.Length || stackedAction.actions[index] == null) {
                if (stackedAction.actions != null && index < stackedAction.actions.Length) {
                    var actions = stackedAction.actions;
                    ArrayUtility.RemoveAt(ref actions, index);
                    reorderableList.list = stackedAction.actions = actions;
                    BehaviorDesignerWindow.instance.SaveBehavior();
                }
                return;
            }
            EditorGUI.LabelField(rect, stackedAction.actions[index].GetType().Name);
            if (stackedAction.actions[index].NodeData == null || stackedAction.NodeData == null || !Application.isPlaying) {
                return;
            }

            if (stackedAction.actions[index].NodeData.ExecutionStatus == TaskStatus.Success || stackedAction.actions[index].NodeData.ExecutionStatus == TaskStatus.Failure) {
                Texture2D texture;
                if (stackedAction.NodeData.IsReevaluating) {
                    texture = stackedAction.actions[index].NodeData.ExecutionStatus == TaskStatus.Failure ? BehaviorDesignerUtility.ExecutionFailureRepeatTexture : BehaviorDesignerUtility.ExecutionSuccessRepeatTexture;
                } else {
                    texture = stackedAction.actions[index].NodeData.ExecutionStatus == TaskStatus.Failure ? BehaviorDesignerUtility.ExecutionFailureTexture : BehaviorDesignerUtility.ExecutionSuccessTexture;
                }
                rect.x = rect.width + 8;
                rect.width = rect.height = 16;
                GUI.DrawTexture(rect, texture);
            }
        }

        private void OnReorderCallback(ReorderableList list)
        {
            var stackedActions = task as StackedAction;
            stackedActions.actions = (Action[])list.list;
            BehaviorDesignerWindow.instance.SaveBehavior();
        }

        private void OnSelectCallback(ReorderableList list)
        {
            EditorPrefs.SetInt("BehaviorDesigner.StackedAction." + task.ID, list.index);
        }

        private bool OnCanRemoveCallback(ReorderableList list)
        {
            var stackedActions = task as StackedAction;
            return stackedActions.actions != null && stackedActions.actions.Length > 0;
        }

        private void OnRemoveCallback(ReorderableList list)
        {
            var stackedAction = task as StackedAction;
            var actions = stackedAction.actions;
            ArrayUtility.RemoveAt(ref actions, list.index);
            reorderableList.list = stackedAction.actions = actions;
            BehaviorDesignerWindow.instance.SaveBehavior();

            reorderableList.index -= 1;
        }
    }
}
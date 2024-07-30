using System;
using System.Collections;
using System.Collections.Generic;
using GAS.General;
using GAS.Runtime;
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using UnityEditor;
using UnityEngine;

namespace GAS.Editor
{
    public class TaskMarkEditor : OdinEditorWindow
    {
        [BoxGroup]
        [HideLabel]
        [DisplayAsString(TextAlignment.Left, true)]
        public string RunInfo;

        private TaskMark _mark;

        [Delayed]
        [BoxGroup]
        [HideReferenceObjectPicker]
        [ListDrawerSettings(ShowFoldout = true, DraggableItems = true)]
        [OnValueChanged("OnTaskListChanged", true)]
        public List<InstantTaskCellInspector> Tasks;

        public static TaskMarkEditor Create(TaskMark mark)
        {
            var window = CreateInstance<TaskMarkEditor>();
            window._mark = mark;

            window.UpdateMarkInfo();
            return window;
        }

        [BoxGroup]
        [Button]
        [GUIColor(0.9f, 0.2f, 0.2f)]
        private void Delete()
        {
            _mark.Delete();
        }

        private void UpdateMarkInfo()
        {
            RunInfo = $"<b>Trigger(f):{_mark.MarkData.startFrame}</b>";
            Tasks = new List<InstantTaskCellInspector>();
            foreach (var taskData in _mark.MarkData.InstantTasks) Tasks.Add(new InstantTaskCellInspector(taskData));
        }

        private void OnTaskListChanged()
        {
            var tasks = new List<InstantTaskData>();
            foreach (var t in Tasks)
                tasks.Add(new InstantTaskData
                {
                    TaskData = new JsonData
                    {
                        Type = t.InstantTaskType,
                        Data = t.Data()
                    }
                });

            _mark.MarkDataForSave.InstantTasks = tasks;
            AbilityTimelineEditorWindow.Instance.Save();
        }
    }

    [CustomEditor(typeof(TaskMarkEditor))]
    public class TaskMarkInspector : OdinEditorWithoutHeader
    {
    }


    public class InstantTaskCellInspector
    {
        private static IEnumerable InstantTaskSonTypes = InstantTaskData.InstantTaskSonTypeChoices;
        private static Type[] _instantTaskInspectorTypes;

        private static Dictionary<Type, Type> _instantTaskInspectorMap;

        private readonly InstantTaskData _data;

        private InstantAbilityTask _instantAbilityTask;

        [Delayed]
        [BoxGroup]
        [LabelText("Task")]
        [ValueDropdown("InstantTaskSonTypes")]
        [InfoBox("This Task has no inspector!", InfoMessageType.Warning, "InstantTaskIsNull")]
        [OnValueChanged("OnTaskTypeChanged")]
        public string InstantTaskType;

        [BoxGroup]
        [HideReferenceObjectPicker]
        [HideIf("InstantTaskIsNull")]
        [LabelText("Detail")]
        public InstantTaskInspector InstantTask;

        public InstantTaskCellInspector(InstantTaskData data)
        {
            _data = data;
            _instantAbilityTask = data.Load() as InstantAbilityTask;
            InstantTaskType = data.TaskData.Type;
            RefreshDetailInspector();
        }

        public InstantTaskCellInspector()
        {
            _data = new InstantTaskData();
            _instantAbilityTask = _data.Load() as InstantAbilityTask;
            InstantTaskType = _data.TaskData.Type;
            RefreshDetailInspector();
        }

        public static Type[] InstantTaskInspectorTypes =>
            _instantTaskInspectorTypes ??= TypeUtil.GetAllSonTypesOf(typeof(InstantTaskInspector));

        private static Dictionary<Type, Type> InstantTaskInspectorMap
        {
            get
            {
                if (_instantTaskInspectorMap != null) return _instantTaskInspectorMap;
                _instantTaskInspectorMap = new Dictionary<Type, Type>();
                foreach (var inspectorType in InstantTaskInspectorTypes)
                {
                    if (inspectorType.BaseType != null)
                    {
                        var taskType = inspectorType.BaseType.GetGenericArguments()[0];
                        _instantTaskInspectorMap.Add(taskType, inspectorType);
                    }
                }

                return _instantTaskInspectorMap;
            }
        }

        public string Data()
        {
            if (_instantAbilityTask == null) return null;

            _data.Save(_instantAbilityTask);
            return _data.TaskData.Data;
        }

        private void OnTaskTypeChanged()
        {
            _data.TaskData.Type = InstantTaskType;
            _data.TaskData.Data = null;
            RefreshDetailInspector();
        }

        private bool InstantTaskIsNull()
        {
            return InstantTask == null;
        }

        public void RefreshDetailInspector()
        {
            _instantAbilityTask = _data.Load() as InstantAbilityTask;
            if (_instantAbilityTask != null && InstantTaskInspectorMap.TryGetValue(_instantAbilityTask.GetType(), out var inspectorType))
            {
                var taskInspector = (InstantTaskInspector)Activator.CreateInstance(inspectorType);
                taskInspector.Init(_instantAbilityTask);
                InstantTask = taskInspector;
            }
            else
            {
                InstantTask = null;
            }
        }
    }
}

#if UNITY_EDITOR
namespace GAS.Editor
{
    using UnityEditor;
    using Editor;
    using UnityEngine;
    using Sirenix.OdinInspector.Editor;
    using Sirenix.OdinInspector;
    using System.Linq;
    using System.Collections;
    using GAS.General;
    using System;
    using System.Collections.Generic;
    using GAS.Runtime;

    public class TaskClipEditor:OdinEditorWindow
    {
        private static IEnumerable OngoingTaskSonTypes = OngoingTaskData.OngoingTaskSonTypeChoices;
        
        private static Type[] _ongoingTaskInspectorTypes;

        public static Type[] OngoingTaskInspectorTypes =>
            _ongoingTaskInspectorTypes ??= TypeUtil.GetAllSonTypesOf(typeof(OngoingTaskInspector));
        
        private static Dictionary<Type, Type> _ongoingTaskInspectorMap;
        private static Dictionary<Type, Type> OngoingTaskInspectorMap
        {
            get
            {
                if (_ongoingTaskInspectorMap != null) return _ongoingTaskInspectorMap;
                _ongoingTaskInspectorMap = new Dictionary<Type, Type>();
                foreach (var inspectorType in OngoingTaskInspectorTypes)
                {
                    var taskType = inspectorType.BaseType.GetGenericArguments()[0];
                    _ongoingTaskInspectorMap.Add(taskType, inspectorType);
                }

                return _ongoingTaskInspectorMap;
            }
        }
        
        private const string GRP_BOX = "GRP_BOX";
        private const string GRP_BOX_TASK = "GRP_BOX/Task";
        private TimelineAbilityAssetBase AbilityAsset => AbilityTimelineEditorWindow.Instance.AbilityAsset;
        private TaskClip _clip;
        
        public static TaskClipEditor Create(TaskClip clip)
        {
            var window = new TaskClipEditor();
            window._clip = clip;
            window.Refresh();
            return window;
        }
        
        [BoxGroup(GRP_BOX)]
        [HideLabel]
        [DisplayAsString(TextAlignment.Left,true)]
        public string RunInfo;

        [Delayed]
        [BoxGroup(GRP_BOX)]
        [LabelText("Duration(f)")]
        [OnValueChanged("OnDurationFrameChanged")]
        public int Duration;
        
        [Delayed]
        [BoxGroup(GRP_BOX_TASK)]
        [LabelText("OngoingTask")]
        [ValueDropdown("OngoingTaskSonTypes")]
        [InfoBox("This Task has no inspector!",InfoMessageType.Warning, "OngoingTaskIsNull")]
        [OnValueChanged("OnTaskTypeChanged")]
        public string OngoingTaskType;
        
        [BoxGroup(GRP_BOX_TASK)]
        [HideReferenceObjectPicker]
        [HideIf("OngoingTaskIsNull")]
        [LabelText("Task Detail")]
        public OngoingTaskInspector OngoingTask;
        
        [BoxGroup(GRP_BOX)]
        [Button]
        [GUIColor(0.9f,0.2f,0.2f)]
        void Delete()
        {
            _clip.Delete();
        }
        
        void Refresh()
        {
            RunInfo = $"<b>Run(f):{_clip.TaskClipData.startFrame} -> {_clip.TaskClipData.EndFrame}</b>";
            Duration = _clip.TaskClipData.durationFrame;
            OngoingTaskType = _clip.TaskClipData.ongoingTask.TaskData.Type;

            RefreshTaskInspector();
        }

        void RefreshTaskInspector()
        {
            // 根据选择的OngoingAbilityTask子类，显示对应的属性
            var ongoingAbilityTask = _clip.TaskClipData.Load();
            if (OngoingTaskInspectorMap.TryGetValue(ongoingAbilityTask.GetType(), out var inspectorType))
            {
                var taskInspector = (OngoingTaskInspector)Activator.CreateInstance(inspectorType);
                taskInspector.Init(ongoingAbilityTask);
                OngoingTask = taskInspector;
            }
            else
            {
                OngoingTask = null;
            }
        }

        private void OnDurationFrameChanged()
        {
            // 钳制
            var max = AbilityAsset.FrameCount - _clip.ClipDataForSave.startFrame;
            Duration = Mathf.Clamp(Duration, 1, max);
            _clip.UpdateClipDataDurationFrame(Duration);
            _clip.RefreshShow(_clip.FrameUnitWidth);
            Refresh();
        }
        
        private void OnTaskTypeChanged()
        {
            _clip.ClipDataForSave.ongoingTask.TaskData.Type = OngoingTaskType;
            _clip.ClipDataForSave.ongoingTask.TaskData.Data = null;
            AbilityTimelineEditorWindow.Instance.Save();
            AbilityTimelineEditorWindow.Instance.TimelineInspector.RefreshInspector();
            
            RefreshTaskInspector();
        }

        bool OngoingTaskIsNull()
        {
            return OngoingTask == null;
        }
    }
    
    [CustomEditor(typeof(TaskClipEditor))]
    public class TaskClipInspector:OdinEditorWithoutHeader
    {
    }
}
#endif
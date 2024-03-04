using System;
using System.Collections.Generic;
using System.Linq;
using GAS.General;
using GAS.Runtime.Ability;
using GAS.Runtime.Ability.TimelineAbility;
using UnityEngine;
using UnityEngine.UIElements;

namespace GAS.Editor.Ability.AbilityTimelineEditor
{
    public class TaskClip : TrackClip<TaskClipEventTrack>
    {
        private TimelineAbilityAsset AbilityAsset => AbilityTimelineEditorWindow.Instance.AbilityAsset;
        private TaskClipEvent TaskClipData => clipData as TaskClipEvent;

        public TaskClipEvent ClipDataForSave
        {
            get
            {
                var cueTrackDataForSave = track.TaskClipTrackDataForSave;
                for (var i = 0; i < cueTrackDataForSave.clipEvents.Count; i++)
                    if (cueTrackDataForSave.clipEvents[i] == TaskClipData)
                        return track.TaskClipTrackDataForSave.clipEvents[i];
                return null;
            }
        }

        private static Type[] _ongoingTaskInspectorTypes;

        public static Type[] OngoingTaskInspectorTypes =>
            _ongoingTaskInspectorTypes ??= TypeUtil.GetAllSonTypesOf(typeof(OngoingAbilityTaskInspector));
        
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
        
        public override void Delete()
        {
            var success = track.TaskClipTrackDataForSave.clipEvents.Remove(TaskClipData);
            AbilityTimelineEditorWindow.Instance.Save();
            if (!success) return;
            track.RemoveTrackItem(this);
            AbilityTimelineEditorWindow.Instance.SetInspector();
        }

        public override void RefreshShow(float newFrameUnitWidth)
        {
            base.RefreshShow(newFrameUnitWidth);
            var taskType = TaskClipData.ongoingTask.TaskData.Type;
            var shortName = taskType.Split('.').Last();
            ItemLabel.text = !string.IsNullOrEmpty(shortName) ? shortName : "Null!";
        }

        public override void UpdateClipDataStartFrame(int newStartFrame)
        {
            var updatedClip = ClipDataForSave;
            ClipDataForSave.startFrame = newStartFrame;
            AbilityTimelineEditorWindow.Instance.Save();
            clipData = updatedClip;
        }

        public override void UpdateClipDataDurationFrame(int newDurationFrame)
        {
            var updatedClip = ClipDataForSave;
            ClipDataForSave.durationFrame = newDurationFrame;
            AbilityTimelineEditorWindow.Instance.Save();
            clipData = updatedClip;
        }

        #region Inspector

        private Label _startFrameLabel;
        private Label _endFrameLabel;
        private IntegerField _durationField;

        public override VisualElement Inspector()
        {
            var inspector = TrackInspectorUtil.CreateTrackInspector();

            // 运行帧
            _startFrameLabel =
                TrackInspectorUtil.CreateLabel(
                    $"Run(f):{TaskClipData.startFrame}->{TaskClipData.EndFrame}");
            inspector.Add(_startFrameLabel);

            // 持续帧
            _durationField = TrackInspectorUtil.CreateIntegerField("Duration(f)", TaskClipData.durationFrame,
                OnDurationFrameChanged);
            inspector.Add(_durationField);
            
            // 选择项：所有OngoingAbilityTask子类
            var ongoingTaskSonTypes= OngoingTaskData.OngoingTaskSonTypes;
            List<string> ongoingTaskSons  = ongoingTaskSonTypes.Select(sonType => sonType.FullName).ToList();
            var catcherTypeSelector =
                TrackInspectorUtil.CreateDropdownField("OngoingTask", ongoingTaskSons,
                    TaskClipData.ongoingTask.TaskData.Type, OnTaskTypeChanged);
            inspector.Add(catcherTypeSelector);
            
            // 根据选择的OngoingAbilityTask子类，显示对应的属性
            var ongoingAbilityTask = TaskClipData.Load();
            if(OngoingTaskInspectorMap.TryGetValue(ongoingAbilityTask.GetType(), out var inspectorType))
            {
                var taskInspector = (OngoingAbilityTaskInspector)Activator.CreateInstance(inspectorType, ongoingAbilityTask);
                inspector.Add(taskInspector.Inspector());
            }
            else
            {
                Debug.LogError( $"[EX] OngoingAbilityTask's Inspector not found: {ongoingAbilityTask.GetType()}");
            }

            // 删除按钮
            var deleteButton = TrackInspectorUtil.CreateButton("删除", Delete);
            deleteButton.style.backgroundColor = new StyleColor(new Color(0.5f, 0, 0, 1f));
            inspector.Add(deleteButton);

            return inspector;
        }

        private void OnTaskTypeChanged(ChangeEvent<string> evt)
        {
            ClipDataForSave.ongoingTask.TaskData.Type = evt.newValue;
            ClipDataForSave.ongoingTask.TaskData.Data = null;
            AbilityTimelineEditorWindow.Instance.Save();
            AbilityTimelineEditorWindow.Instance.TimelineInspector.RefreshInspector();
        }

        private void OnDurationFrameChanged(ChangeEvent<int> evt)
        {
            // 钳制
            var max = AbilityAsset.MaxFrameCount - TaskClipData.startFrame;
            var newValue = Mathf.Clamp(evt.newValue, 1, max);
            // 保存数据
            UpdateClipDataDurationFrame(newValue);
            // 修改显示
            RefreshShow(FrameUnitWidth);
            _endFrameLabel.text = $"结束帧:{TaskClipData.EndFrame}";
            _durationField.value = newValue;
        }

        #endregion
    }
}
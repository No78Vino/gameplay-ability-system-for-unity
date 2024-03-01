using GAS.Runtime.Ability;
using GAS.Runtime.Ability.TimelineAbility;
using GAS.Runtime.Ability.TimelineAbility.AbilityTask;
using UnityEngine;
using UnityEngine.UIElements;

namespace GAS.Editor.Ability.AbilityTimelineEditor
{
    public class TaskClip : TrackClip<TaskClipEventTrack>
    {
        private TimelineAbilityAsset AbilityAsset => AbilityTimelineEditorWindow.Instance.AbilityAsset;
        private TaskClipEvent TaskClipData => clipData as TaskClipEvent;

        private TaskClipEvent ClipDataForSave
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
            ItemLabel.text = TaskClipData.task?TaskClipData.task.name:"Null!";
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
                    $"运行(f):{TaskClipData.startFrame}/{TaskClipData.EndFrame}");
            inspector.Add(_startFrameLabel);

            // 持续帧
            _durationField = TrackInspectorUtil.CreateIntegerField("时长(f)", TaskClipData.durationFrame,
                OnDurationFrameChanged);
            inspector.Add(_durationField);

            // 任务
            var taskField = TrackInspectorUtil.CreateObjectField("自定义事件",typeof(OngoingAbilityTask) ,TaskClipData.task,
                evt =>
                {
                    ClipDataForSave.task = evt.newValue as OngoingAbilityTask;
                    AbilityTimelineEditorWindow.Instance.Save();
                    ItemLabel.text = TaskClipData.task.name;
                });
            inspector.Add(taskField);

            // 删除按钮
            var deleteButton = TrackInspectorUtil.CreateButton("删除", Delete);
            deleteButton.style.backgroundColor = new StyleColor(new Color(0.5f, 0, 0, 1f));
            inspector.Add(deleteButton);

            return inspector;
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
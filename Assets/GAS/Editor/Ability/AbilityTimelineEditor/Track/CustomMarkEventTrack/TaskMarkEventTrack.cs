using System;
using System.Collections.Generic;
using GAS.Editor.Ability.AbilityTimelineEditor;
using GAS.Runtime.Ability.TimelineAbility;
using GAS.Runtime.Cue;
using UnityEngine;
using UnityEngine.UIElements;

namespace GAS.Editor.Ability.AbilityTimelineEditor
{
    public class TaskMarkEventTrack:FixedTrack
    {
        public static TaskMarkEventTrackData InstantTaskEventTrackData =>
            AbilityTimelineEditorWindow.Instance.AbilityAsset.InstantTasks;

        public override Type TrackDataType => typeof(TaskMarkEventTrackData);
        protected override Color TrackColor => new(0.1f, 0.6f, 0.6f, 0.2f);
        protected override Color MenuColor => new(0.1f, 0.6f, 0.6f, 0.9f);
        
        public override void Init(VisualElement trackParent, VisualElement menuParent, float frameWidth,
            TrackDataBase trackData)
        {
            base.Init(trackParent, menuParent, frameWidth, trackData);
            MenuText.text = "Instant Task";
        }

        public override void TickView(int frameIndex, params object[] param)
        {
        }

        public override void RefreshShow(float newFrameWidth)
        {
            base.RefreshShow(newFrameWidth);
            foreach (var item in _trackItems) Track.Remove(((TrackMarkBase)item).Ve);
            _trackItems.Clear();

            if (AbilityTimelineEditorWindow.Instance.AbilityAsset == null) return;

            foreach (var markEvent in InstantTaskEventTrackData.markEvents)
            {
                var item = new TaskMark();
                item.InitTrackMark(this, Track, _frameWidth, markEvent);
                _trackItems.Add(item);
            }
        }

        public override VisualElement Inspector()
        {
            var inspector = TrackInspectorUtil.CreateTrackInspector();

            var trackLabel = TrackInspectorUtil.CreateLabel("Instant Task:");
            inspector.Add(trackLabel);

            foreach (var mark in InstantTaskEventTrackData.markEvents)
            {
                var markFrame = TrackInspectorUtil.CreateLabel($"   Trigger(f):{mark.startFrame}");
                inspector.Add(markFrame);
                foreach (var task in mark.InstantTasks)
                {
                    var taskName = task != null ? task.name : "Null!";
                    var taskInfo = TrackInspectorUtil.CreateLabel($"    |-> {taskName}");
                    inspector.Add(taskInfo);
                }
            }

            return inspector;
        }

        protected override void OnAddTrackItem(DropdownMenuAction action)
        {
            // 添加Mark数据
            var markEvent = new TaskMarkEvent
            {
                startFrame = GetTrackIndexByMouse(action.eventInfo.localMousePosition.x),
            };
            InstantTaskEventTrackData.markEvents.Add(markEvent);

            // 刷新显示
            var mark = new TaskMark();
            mark.InitTrackMark(this, Track, _frameWidth, markEvent);
            _trackItems.Add(mark);

            // 选中新Clip
            mark.OnSelect();

            Debug.Log("[EX] Add Instant Cue Mark");
        }

        protected override void OnRemoveTrack(DropdownMenuAction action)
        {
            InstantTaskEventTrackData.markEvents.Clear();
            AbilityTimelineEditorWindow.Instance.Save();
        }
    }
}
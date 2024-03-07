using System;
using System.Collections.Generic;
using GAS.Editor.Ability.AbilityTimelineEditor;
using GAS.Runtime.Ability.TimelineAbility;
using GAS.Runtime.Cue;
using UnityEngine;
using UnityEngine.UIElements;

namespace GAS.Editor.Ability.AbilityTimelineEditor
{
    public class TaskMarkEventTrack:TrackBase
    {
        private static TimelineAbilityAsset AbilityAsset => AbilityTimelineEditorWindow.Instance.AbilityAsset;
        public TaskMarkEventTrackData InstantTaskEventTrackData {
            get
            {
                for (int i = 0; i < AbilityAsset.InstantTasks.Count; i++)
                {
                    if(AbilityAsset.InstantTasks[i] == _instantTasksTrackData)
                        return AbilityAsset.InstantTasks[i];
                }
                return null;
            }
        }
        public override Type TrackDataType => typeof(TaskMarkEventTrackData);
        protected override Color TrackColor => new(0.1f, 0.6f, 0.6f, 0.2f);
        protected override Color MenuColor => new(0.1f, 0.6f, 0.6f, 0.9f);
        
        private TaskMarkEventTrackData _instantTasksTrackData;
        public override void Init(VisualElement trackParent, VisualElement menuParent, float frameWidth,
            TrackDataBase trackData)
        {
            base.Init(trackParent, menuParent, frameWidth, trackData);
            MenuText.text = "Instant Task";
            _instantTasksTrackData = trackData as TaskMarkEventTrackData;
        }

        public override void TickView(int frameIndex, params object[] param)
        {
            foreach (var item in _trackItems)
                ((TrackMarkBase)item).OnTickView(frameIndex);
        }

        public override void RefreshShow(float newFrameWidth)
        {
            base.RefreshShow(newFrameWidth);
            foreach (var item in _trackItems) Track.Remove(((TrackMarkBase)item).Ve);
            _trackItems.Clear();

            if (AbilityTimelineEditorWindow.Instance.AbilityAsset == null) return;

            foreach (var markEvent in _instantTasksTrackData.markEvents)
            {
                var item = new TaskMark();
                item.InitTrackMark(this, Track, _frameWidth, markEvent);
                _trackItems.Add(item);
            }
        }

        public override VisualElement Inspector()
        {
            var inspector = TrackInspectorUtil.CreateTrackInspector();

            var trackLabel = TrackInspectorUtil.CreateLabel("[ Instant Ability Task ]");
            inspector.Add(trackLabel);

            foreach (var mark in _instantTasksTrackData.markEvents)
            {
                var markFrame = TrackInspectorUtil.CreateLabel($"Trigger(f):{mark.startFrame}");
                inspector.Add(markFrame);
                foreach (var task in mark.InstantTasks)
                {
                    var taskName = task != null ? task.TaskData.Type : "Null!";
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

            Debug.Log("[EX] Add Instant Task Mark");
        }

        protected override void OnRemoveTrack(DropdownMenuAction action)
        {
            // 删除数据
            AbilityAsset.InstantTasks.Remove(_instantTasksTrackData);
            AbilityTimelineEditorWindow.Instance.Save();
            // 删除显示
            TrackParent.Remove(TrackRoot);
            MenuParent.Remove(MenuRoot);
            Debug.Log("[EX] Remove Instant Task Track");
        }
    }
}
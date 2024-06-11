
#if UNITY_EDITOR

namespace GAS.Editor
{
    using System;
    using UnityEngine;
    using UnityEngine.UIElements;
    using GAS.Runtime;

    public class TaskMarkEventTrack : TrackBase
    {
        private TaskMarkEventTrackData _instantTasksTrackData;
        private static TimelineAbilityAssetBase AbilityAsset => AbilityTimelineEditorWindow.Instance.AbilityAsset;

        public TaskMarkEventTrackData InstantTaskEventTrackData
        {
            get
            {
                for (var i = 0; i < AbilityAsset.InstantTasks.Count; i++)
                    if (AbilityAsset.InstantTasks[i] == _instantTasksTrackData)
                        return AbilityAsset.InstantTasks[i];
                return null;
            }
        }

        public override Type TrackDataType => typeof(TaskMarkEventTrackData);
        protected override Color TrackColor => new(0.1f, 0.6f, 0.6f, 0.2f);
        protected override Color MenuColor => new(0.1f, 0.6f, 0.6f, 0.9f);

        public override void Init(VisualElement trackParent, VisualElement menuParent, float frameWidth,
            TrackDataBase trackData)
        {
            base.Init(trackParent, menuParent, frameWidth, trackData);
            _instantTasksTrackData = trackData as TaskMarkEventTrackData;
            MenuText.text = _instantTasksTrackData.trackName;
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

        public override UnityEngine.Object DataInspector => TaskMarkEventTrackEditor.Create(this);

        protected override void OnAddTrackItem(DropdownMenuAction action)
        {
            // 添加Mark数据
            var markEvent = new TaskMarkEvent
            {
                startFrame = GetTrackIndexByMouse(action.eventInfo.localMousePosition.x)
            };
            InstantTaskEventTrackData.markEvents.Add(markEvent);

            // 刷新显示
            var mark = new TaskMark();
            mark.InitTrackMark(this, Track, _frameWidth, markEvent);
            _trackItems.Add(mark);

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
#endif
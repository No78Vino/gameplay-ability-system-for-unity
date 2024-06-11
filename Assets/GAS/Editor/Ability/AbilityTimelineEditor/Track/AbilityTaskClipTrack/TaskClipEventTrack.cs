#if UNITY_EDITOR
namespace GAS.Editor
{
    using System;
    using UnityEngine;
    using UnityEngine.UIElements;
    using GAS.Runtime;

    public class TaskClipEventTrack:TrackBase
    {
        private TaskClipEventTrackData _taskClipEventTrackData;
        public override Type TrackDataType => typeof(TaskClipEventTrackData);
        protected override Color TrackColor => new Color(0.7f, 0.3f, 0.7f, 0.2f);
        protected override Color MenuColor => new Color(0.5f, 0.3f, 0.5f, 1);

        private TimelineAbilityAssetBase AbilityAsset => AbilityTimelineEditorWindow.Instance.AbilityAsset;
        public TaskClipEventTrackData TaskClipTrackDataForSave
        {
            get
            {
                for (int i = 0; i < AbilityAsset.OngoingTasks.Count; i++)
                {
                    if(AbilityAsset.OngoingTasks[i] == _taskClipEventTrackData)
                        return AbilityAsset.OngoingTasks[i];
                }
                return null;
            }
        }
        
        public override void TickView(int frameIndex, params object[] param)
        {
            foreach (var item in _trackItems)
            {
                var taskClip = item as TaskClip;
                taskClip.OnTickView(frameIndex, taskClip.StartFrameIndex, taskClip.EndFrameIndex);
            }
        }

        public override void Init(VisualElement trackParent, VisualElement menuParent, float frameWidth, TrackDataBase trackData)
        {
            base.Init(trackParent, menuParent, frameWidth, trackData);
            _taskClipEventTrackData = trackData as TaskClipEventTrackData;
            MenuText.text = _taskClipEventTrackData.trackName;
        }

        public override void RefreshShow(float newFrameWidth)
        {
            base.RefreshShow(newFrameWidth);
            foreach (var item in _trackItems) Track.Remove(((TrackClipBase)item).ClipVe);
            _trackItems.Clear();

            if (AbilityTimelineEditorWindow.Instance.AbilityAsset != null)
                foreach (var clipEvent in _taskClipEventTrackData.clipEvents)
                {
                    var item = new TaskClip();
                    item.InitTrackClip(this, Track, _frameWidth, clipEvent);
                    _trackItems.Add(item);
                }
        }
        
        protected override void OnAddTrackItem(DropdownMenuAction action)
        {
            // 添加Clip数据
            var clipEvent = new TaskClipEvent
            {
                startFrame = GetTrackIndexByMouse(action.eventInfo.localMousePosition.x),
                durationFrame = 5,
                ongoingTask = new OngoingTaskData()
            };
            TaskClipTrackDataForSave.clipEvents.Add(clipEvent);
            
            // 刷新显示
            var item = new TaskClip();
            item.InitTrackClip(this, Track, _frameWidth, clipEvent);
            _trackItems.Add(item);
            
            // 选中新Clip
            item.ClipVe.OnSelect();
            
            Debug.Log("[EX] Add a new Custom Clip Event");
        }

        protected override void OnRemoveTrack(DropdownMenuAction action)
        {
            // 删除数据
            AbilityAsset.OngoingTasks.Remove(_taskClipEventTrackData);
            AbilityTimelineEditorWindow.Instance.Save();
            // 删除显示
            TrackParent.Remove(TrackRoot);
            MenuParent.Remove(MenuRoot);
            Debug.Log("[EX] Remove Task Clip Track");
        }
        
        public override UnityEngine.Object DataInspector => TaskClipEventTrackEditor.Create(this);
    }
}
#endif
using System;
using GAS.Runtime;
using UnityEngine;
using UnityEngine.UIElements;
using Object = UnityEngine.Object;

#if UNITY_EDITOR
namespace GAS.Editor
{
    public class AbilityTimelineTrack : TrackBase
    {
        protected override Color TrackColor => new(0.7f, 0.3f, 0.7f, 0.2f);
        protected override Color MenuColor => new(0.5f, 0.3f, 0.5f, 1);

        private XParamTimeline AbilityConfig => AbilityTimelineEditorWindow.Instance.AbilityConfig;
        public Track TrackData => _trackInfo;

        public override Object DataInspector => AbilityTimelineTrackEditor.Create(this);

        public override void TickView(int frameIndex, params object[] param)
        {
            foreach (var item in _trackItems)
            {
                var taskClip = item as TaskClip;
                taskClip.OnTickView(frameIndex, taskClip.StartFrameIndex, taskClip.EndFrameIndex);
            }
        }

        public override void Init(VisualElement trackParent, VisualElement menuParent, float frameWidth,
            Track trackInfo)
        {
            base.Init(trackParent, menuParent, frameWidth, trackInfo);
            MenuText.text = trackInfo.Name;
        }

        public override void RefreshShow(float newFrameWidth)
        {
            base.RefreshShow(newFrameWidth);
            foreach (var item in _trackItems) Track.Remove(item.ClipVe);
            _trackItems.Clear();

            if (AbilityTimelineEditorWindow.Instance.AbilityConfig != null)
                foreach (var task in _trackInfo.TaskClips)
                {
                    var item = new TaskClip();
                    item.InitTrackClip(this, Track, _frameWidth, task);
                    _trackItems.Add(item);
                }
            
            MenuText.text = _trackInfo.Name;
        }

        protected override void OnAddTrackItem(DropdownMenuAction action)
        {
            // 添加Clip数据
            var startTime = GetTrackIndexByMouse(action.eventInfo.localMousePosition.x);
            var clipEvent = new TaskClipData
            {
                Name = "新任务",
                StartTime = startTime,
                EndTime = Math.Min(startTime + 5, AbilityConfig.LifeTime),
                TaskType = "TaskDoNothing"
            };
            TrackData.TaskClips.Add(clipEvent);

            // 刷新显示
            var item = new TaskClip();
            item.InitTrackClip(this, Track, _frameWidth, clipEvent);
            _trackItems.Add(item);

            // 选中新Clip
            item.ClipVe.OnSelect();

            Debug.Log("[EX] Add a new Custom Task Clip");
        }

        protected override void OnRemoveTrack(DropdownMenuAction action)
        {
            // 删除数据
            AbilityConfig.Tracks.Remove(_trackInfo);

            // 删除显示
            TrackParent.Remove(TrackRoot);
            MenuParent.Remove(MenuRoot);
            Debug.Log("[EX] Remove Task Clip Track");
        }
    }
}
#endif
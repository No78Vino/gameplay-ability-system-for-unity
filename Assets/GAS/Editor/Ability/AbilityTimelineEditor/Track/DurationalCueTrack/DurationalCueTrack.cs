#if UNITY_EDITOR
namespace GAS.Editor
{
    using System;
    using UnityEngine;
    using UnityEngine.UIElements;
    using GAS.Runtime;

    public class DurationalCueTrack : TrackBase
    {
        private DurationalCueTrackData _durationalCueTrackData;
        public override Type TrackDataType => typeof(DurationalCueTrackData);
        protected override Color TrackColor => new(0.1f, 0.6f, 0.1f, 0.2f);
        protected override Color MenuColor => new(0.1f, 0.6f, 0.1f, 1);

        private TimelineAbilityAssetBase AbilityAsset => AbilityTimelineEditorWindow.Instance.AbilityAsset;

        public DurationalCueTrackData CueTrackDataForSave
        {
            get
            {
                for (var i = 0; i < AbilityAsset.DurationalCues.Count; i++)
                    if (AbilityAsset.DurationalCues[i] == _durationalCueTrackData)
                        return AbilityAsset.DurationalCues[i];
                return null;
            }
        }

        public override void TickView(int frameIndex, params object[] param)
        {
            foreach (var item in _trackItems)
            {
                var durationalCueClip = item as DurationalCueClip;
                durationalCueClip?.OnTickView(frameIndex, durationalCueClip.StartFrameIndex,
                    durationalCueClip.EndFrameIndex);
            }
        }

        public override void Init(VisualElement trackParent, VisualElement menuParent, float frameWidth,
            TrackDataBase trackData)
        {
            base.Init(trackParent, menuParent, frameWidth, trackData);
            _durationalCueTrackData = trackData as DurationalCueTrackData;
            MenuText.text = _durationalCueTrackData.trackName;
        }

        public override void RefreshShow(float newFrameWidth)
        {
            base.RefreshShow(newFrameWidth);
            foreach (var item in _trackItems) Track.Remove(((TrackClipBase)item).ClipVe);
            _trackItems.Clear();

            if (AbilityTimelineEditorWindow.Instance.AbilityAsset != null)
                foreach (var clipEvent in _durationalCueTrackData.clipEvents)
                {
                    var item = new DurationalCueClip();
                    item.InitTrackClip(this, Track, _frameWidth, clipEvent);
                    _trackItems.Add(item);
                }
        }

        protected override void OnAddTrackItem(DropdownMenuAction action)
        {
            // 添加Clip数据
            var clipEvent = new DurationalCueClipEvent
            {
                startFrame = GetTrackIndexByMouse(action.eventInfo.localMousePosition.x),
                durationFrame = 5
            };
            CueTrackDataForSave.clipEvents.Add(clipEvent);

            // 刷新显示
            var item = new DurationalCueClip();
            item.InitTrackClip(this, Track, _frameWidth, clipEvent);
            _trackItems.Add(item);

            // 选中新Clip
            item.ClipVe.OnSelect();

            Debug.Log("[EX] Add a new Durational Cue Clip");
        }

        protected override void OnRemoveTrack(DropdownMenuAction action)
        {
            // 删除数据
            AbilityAsset.DurationalCues.Remove(_durationalCueTrackData);
            AbilityTimelineEditorWindow.Instance.Save();
            // 删除显示
            TrackParent.Remove(TrackRoot);
            MenuParent.Remove(MenuRoot);
            Debug.Log("[EX] Remove Durational Cue Track");
        }

        public override UnityEngine.Object DataInspector => DurationalCueTrackEditor.Create(this);
    }
}
#endif
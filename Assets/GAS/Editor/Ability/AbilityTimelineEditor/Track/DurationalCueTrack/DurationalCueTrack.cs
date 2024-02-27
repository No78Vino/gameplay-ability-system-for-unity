using System;
using GAS.Editor.Ability.AbilityTimelineEditor.Track;
using GAS.Runtime.Ability;
using GAS.Runtime.Ability.AbilityTimeline;
using UnityEngine;
using UnityEngine.UIElements;

namespace GAS.Editor.Ability.AbilityTimelineEditor
{
    public class DurationalCueTrack:TrackBase
    {
        private DurationalCueTrackData _durationalCueTrackData;
        public override Type TrackDataType => typeof(DurationalCueTrackData);
        protected override Color TrackColor => new Color(0.1f, 0.6f, 0.1f, 0.2f);
        protected override Color MenuColor => new Color(0.1f, 0.6f, 0.1f, 1);

        private TimelineAbilityAsset AbilityAsset => AbilityTimelineEditorWindow.Instance.AbilityAsset;
        private DurationalCueTrackData CueDataForSave
        {
            get
            {
                for (int i = 0; i < AbilityAsset.DurationalCues.Count; i++)
                {
                    if(AbilityAsset.DurationalCues[i] == _durationalCueTrackData)
                        return AbilityAsset.DurationalCues[i];
                }
                return null;
            }
        }
        
        public override void TickView(int frameIndex, params object[] param)
        {
        }

        public override void Init(VisualElement trackParent, VisualElement menuParent, float frameWidth, TrackDataBase trackData)
        {
            base.Init(trackParent, menuParent, frameWidth, trackData);
            _durationalCueTrackData = trackData as DurationalCueTrackData;
            MenuText.text = _durationalCueTrackData.trackName;
        }

        public override void RefreshShow(float newFrameWidth)
        {
            base.RefreshShow(newFrameWidth);
            foreach (var item in _trackItems) Track.Remove(item.Ve);
            _trackItems.Clear();

            if (AbilityTimelineEditorWindow.Instance.AbilityAsset != null)
                foreach (var clipEvent in _durationalCueTrackData.clipEvents)
                {
                    var item = new DurationalCueClip();
                    item.InitTrackClip(this, Track, _frameWidth, clipEvent);
                    _trackItems.Add(item);
                }
        }
        
        protected override void OnAddClip(DropdownMenuAction action)
        {
            // 添加Clip数据
            var clipEvent = new DurationalCueClipEvent
            {
                startFrame = (int)(action.eventInfo.mousePosition.x / _frameWidth),
                durationFrame = 5
            };
            CueDataForSave.clipEvents.Add(clipEvent);
            
            // 刷新显示
            var item = new DurationalCueClip();
            item.InitTrackClip(this, Track, _frameWidth, clipEvent);
            _trackItems.Add(item);
            
            // 选中新Clip
            item.Ve.OnSelect();
            
            Debug.Log("[EX] Add a new Durational Cue Clip");
        }

        #region Inspector
        public override VisualElement Inspector()
        {
            var inspector = new VisualElement();
            // track Name
            var trackName = new TextField("Track Name");
            trackName.value = _durationalCueTrackData.trackName;
            trackName.RegisterValueChangedCallback(evt =>
            {
                // TODO 修改数据
                _durationalCueTrackData.trackName = evt.newValue;
                MenuText.text = evt.newValue;
            });
            inspector.Add(trackName);
            
            return inspector;
        }
        #endregion
    }
}
using System;
using GAS.Editor.Ability.AbilityTimelineEditor.Track;
using GAS.Runtime.Ability.AbilityTimeline;
using UnityEngine;
using UnityEngine.UIElements;

namespace GAS.Editor.Ability.AbilityTimelineEditor
{
    public class DurationalCueTrack:TrackBase
    {
        private DurationalCueTrackData _durationalCueTrackData;
        public override Type TrackDataType => typeof(DurationalCueTrackData);
        protected override Color TrackColor => new Color(0.1f, 0.6f, 0.1f, 0.5f);
        protected override Color MenuColor => new Color(0.1f, 0.6f, 0.1f, 1);

        public override void TickView(int frameIndex, params object[] param)
        {
        }

        public override void Init(VisualElement trackParent, VisualElement menuParent, float frameWidth, TrackDataBase trackData)
        {
            base.Init(trackParent, menuParent, frameWidth, trackData);
            _durationalCueTrackData = trackData as DurationalCueTrackData;
            MenuText.text = _durationalCueTrackData.trackName;
        }
    }
}
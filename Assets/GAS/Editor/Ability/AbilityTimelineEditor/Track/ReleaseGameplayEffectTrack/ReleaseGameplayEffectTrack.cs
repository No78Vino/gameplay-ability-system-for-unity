using System;
using GAS.Editor.Ability.AbilityTimelineEditor.Track;
using UnityEngine;
using UnityEngine.UIElements;

namespace GAS.Editor.Ability.AbilityTimelineEditor
{
    public class ReleaseGameplayEffectTrack:TrackBase
    {
        public override Type TrackDataType { get; }
        protected override Color TrackColor { get; }
        protected override Color MenuColor { get; }
        public override void TickView(int frameIndex, params object[] param)
        {
            throw new NotImplementedException();
        }

        public override VisualElement Inspector()
        {
            throw new NotImplementedException();
        }

        protected override void OnAddTrackItem(DropdownMenuAction action)
        {
            throw new NotImplementedException();
        }

        protected override void OnRemoveTrack(DropdownMenuAction action)
        {
            throw new NotImplementedException();
        }
    }
}
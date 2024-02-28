using GAS.Editor.Ability.AbilityTimelineEditor.Track;
using UnityEngine.UIElements;

namespace GAS.Editor.Ability.AbilityTimelineEditor
{
    public class InstantCueMark:TrackMark<InstantCueTrack>
    {
        public override VisualElement Inspector()
        {
            var inspector = TrackInspectorUtil.CreateTrackInspector();
            var markFrame = TrackInspectorUtil.CreateLabel($"触发帧:{markData.startFrame}");
            inspector.Add(markFrame);
            
            // foreach (var c in ((InstantCueMarkEvent) markData).cues)
            // {
            //     var cueName = c != null ? c.name : "NULL";
            //     var cueCount = TrackInspectorUtil.CreateLabel($"    |-> Cue:{cueName}");
            //     inspector.Add(cueCount);
            // }
            
            return inspector;
        }

        public override void Delete()
        {
            //TODO
        }

        public override void UpdateMarkDataFrame(int newStartFrame)
        {
            // TODO
        }
    }
}
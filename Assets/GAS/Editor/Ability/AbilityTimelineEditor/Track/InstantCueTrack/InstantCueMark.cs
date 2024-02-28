using GAS.Editor.Ability.AbilityTimelineEditor.Track;
using GAS.Runtime.Ability.AbilityTimeline;
using GAS.Runtime.Cue;
using UnityEngine.UIElements;

namespace GAS.Editor.Ability.AbilityTimelineEditor
{
    public class InstantCueMark:TrackMark<InstantCueTrack>
    {
        private InstantCueMarkEvent MarkData => markData as InstantCueMarkEvent;

        private InstantCueMarkEvent MarkDataForSave
        {
            get
            {
                var cueTrackDataForSave = InstantCueTrack.InstantCueTrackData;
                for (var i = 0; i < cueTrackDataForSave.markEvents.Count; i++)
                    if (cueTrackDataForSave.markEvents[i] == MarkData)
                        return InstantCueTrack.InstantCueTrackData.markEvents[i];
                return null;
            }
        }

        public override void RefreshShow(float newFrameUnitWidth)
        {
            base.RefreshShow(newFrameUnitWidth);
            ItemLabel.text = MarkData.cues.Count.ToString();
        }

        public override VisualElement Inspector()
        {
            var inspector = TrackInspectorUtil.CreateTrackInspector();
            var markFrame = TrackInspectorUtil.CreateLabel($"触发帧:{markData.startFrame}");
            inspector.Add(markFrame);

            var list = TrackInspectorUtil.CreateListView<GameplayCueInstant>("Cue列表", MarkData.cues, null, null);
            inspector.Add(list);
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
            var updatedClip = MarkDataForSave;
            MarkDataForSave.startFrame = newStartFrame;
            AbilityTimelineEditorWindow.Instance.Save();
            markData = updatedClip;
        }
    }
}
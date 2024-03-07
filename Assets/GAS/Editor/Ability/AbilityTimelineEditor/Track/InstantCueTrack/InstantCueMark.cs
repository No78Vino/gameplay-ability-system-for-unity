using GAS.Editor.Ability.AbilityTimelineEditor;
using GAS.Runtime.Ability.TimelineAbility;
using GAS.Runtime.Cue;
using UnityEngine;
using UnityEngine.UIElements;

namespace GAS.Editor.Ability.AbilityTimelineEditor
{
    public class InstantCueMark:TrackMark<InstantCueTrack>
    {
        private InstantCueMarkEvent InstantCueMarkData => markData as InstantCueMarkEvent;

        private InstantCueMarkEvent MarkDataForSave
        {
            get
            {
                var cueTrackDataForSave = track.InstantCueTrackData;
                for (var i = 0; i < cueTrackDataForSave.markEvents.Count; i++)
                    if (cueTrackDataForSave.markEvents[i] == InstantCueMarkData)
                        return track.InstantCueTrackData.markEvents[i];
                return null;
            }
        }

        public override void RefreshShow(float newFrameUnitWidth)
        {
            base.RefreshShow(newFrameUnitWidth);
            ItemLabel.text = InstantCueMarkData.cues.Count.ToString();
        }

        public override VisualElement Inspector()
        {
            var inspector = TrackInspectorUtil.CreateTrackInspector();
            var markFrame = TrackInspectorUtil.CreateLabel($"触发帧:{markData.startFrame}");
            inspector.Add(markFrame);

            var list = TrackInspectorUtil.CreateObjectListView("Cue列表", InstantCueMarkData.cues, OnCueAssetChanged);
            inspector.Add(list);
            
            return inspector;
        }

        private void OnCueAssetChanged(int index, ChangeEvent<Object> evt)
        {
            var cue = evt.newValue as GameplayCueInstant;
            MarkDataForSave.cues[index] = cue;
            AbilityTimelineEditorWindow.Instance.Save();
            
            RefreshShow(FrameUnitWidth);
        }

        public override void Delete()
        {
            var success = track.InstantCueTrackData.markEvents.Remove(InstantCueMarkData);
            AbilityTimelineEditorWindow.Instance.Save();
            if (!success) return;
            track.RemoveTrackItem(this);
            AbilityTimelineEditorWindow.Instance.SetInspector();
        }

        public override void UpdateMarkDataFrame(int newStartFrame)
        {
            var updatedClip = MarkDataForSave;
            MarkDataForSave.startFrame = newStartFrame;
            AbilityTimelineEditorWindow.Instance.Save();
            markData = updatedClip;
        }

        public override void OnTickView(int frameIndex)
        {
            foreach (var cue in InstantCueMarkData.cues)
            {
                cue.OnEditorPreview(AbilityTimelineEditorWindow.Instance.PreviewObject, frameIndex,
                    InstantCueMarkData.startFrame);
            }
        }
    }
}
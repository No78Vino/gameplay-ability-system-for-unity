using GAS.Editor.Ability.AbilityTimelineEditor.Track;
using GAS.Runtime.Ability.AbilityTimeline;
using GAS.Runtime.Cue;
using UnityEngine;
using UnityEngine.UIElements;

namespace GAS.Editor.Ability.AbilityTimelineEditor
{
    public class CustomMark:TrackMark<CustomMarkEventTrack>
    {
        private CustomMarkEvent MarkData => markData as CustomMarkEvent;

        private CustomMarkEvent MarkDataForSave
        {
            get
            {
                var trackDataForSave = CustomMarkEventTrack.CustomMarkEventTrackData;
                for (var i = 0; i < trackDataForSave.markEvents.Count; i++)
                    if (trackDataForSave.markEvents[i] == MarkData)
                        return CustomMarkEventTrack.CustomMarkEventTrackData.markEvents[i];
                return null;
            }
        }

        public override void RefreshShow(float newFrameUnitWidth)
        {
            base.RefreshShow(newFrameUnitWidth);
            ItemLabel.text = "临时"; //MarkData.cues.Count.ToString();
        }

        public override VisualElement Inspector()
        {
            var inspector = TrackInspectorUtil.CreateTrackInspector();
            var markFrame = TrackInspectorUtil.CreateLabel($"触发帧:{markData.startFrame}");
            inspector.Add(markFrame);

            // var list = TrackInspectorUtil.CreateObjectListView("Cue列表", MarkData.cues, OnCueAssetChanged);
            // inspector.Add(list);
            
            return inspector;
        }

        private void OnCueAssetChanged(int index, ChangeEvent<Object> evt)
        {
            //var cue = evt.newValue as GameplayCueInstant;
            //MarkDataForSave.cues[index] = cue;
            AbilityTimelineEditorWindow.Instance.Save();
            
            RefreshShow(FrameUnitWidth);
        }

        public override void Delete()
        {
            var success = CustomMarkEventTrack.CustomMarkEventTrackData.markEvents.Remove(MarkData);
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
    }
}
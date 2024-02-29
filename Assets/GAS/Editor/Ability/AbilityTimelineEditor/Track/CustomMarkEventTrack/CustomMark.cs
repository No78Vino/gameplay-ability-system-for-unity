using GAS.Runtime.Ability.AbilityTimeline;
using UnityEngine.UIElements;

namespace GAS.Editor.Ability.AbilityTimelineEditor
{
    public class CustomMark : TrackMark<CustomMarkEventTrack>
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
            ItemLabel.text = "";
        }

        public override VisualElement Inspector()
        {
            var inspector = TrackInspectorUtil.CreateTrackInspector();
            var markFrame = TrackInspectorUtil.CreateLabel($"触发帧:{markData.startFrame}");
            inspector.Add(markFrame);

            // 自定义事件
            var customEvent = TrackInspectorUtil.CreateStringListView("自定义事件", MarkData.customEventKeys, (index, evt) =>
            {
                MarkDataForSave.customEventKeys[index] = evt.newValue;
                AbilityTimelineEditorWindow.Instance.Save();
            });
            inspector.Add(customEvent);

            return inspector;
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

#if UNITY_EDITOR
namespace GAS.Editor
{
    using System.Linq;
    using GAS.Runtime;

    
    public class TaskClip : TrackClip<TaskClipEventTrack>
    {
        private TimelineAbilityAssetBase AbilityAsset => AbilityTimelineEditorWindow.Instance.AbilityAsset;
        public TaskClipEvent TaskClipData => clipData as TaskClipEvent;

        public TaskClipEvent ClipDataForSave
        {
            get
            {
                var cueTrackDataForSave = track.TaskClipTrackDataForSave;
                for (var i = 0; i < cueTrackDataForSave.clipEvents.Count; i++)
                    if (cueTrackDataForSave.clipEvents[i] == TaskClipData)
                        return track.TaskClipTrackDataForSave.clipEvents[i];
                return null;
            }
        }
        
        
        public override void Delete()
        {
            var success = track.TaskClipTrackDataForSave.clipEvents.Remove(TaskClipData);
            AbilityTimelineEditorWindow.Instance.Save();
            if (!success) return;
            track.RemoveTrackItem(this);
            AbilityTimelineEditorWindow.Instance.SetInspector();
        }

        public override void RefreshShow(float newFrameUnitWidth)
        {
            base.RefreshShow(newFrameUnitWidth);
            var taskType = TaskClipData.ongoingTask.TaskData.Type;
            var shortName = taskType.Split('.').Last();
            ItemLabel.text = !string.IsNullOrEmpty(shortName) ? shortName : "Null!";
        }

        public override void UpdateClipDataStartFrame(int newStartFrame)
        {
            var updatedClip = ClipDataForSave;
            ClipDataForSave.startFrame = newStartFrame;
            AbilityTimelineEditorWindow.Instance.Save();
            clipData = updatedClip;
        }

        public override void UpdateClipDataDurationFrame(int newDurationFrame)
        {
            var updatedClip = ClipDataForSave;
            ClipDataForSave.durationFrame = newDurationFrame;
            AbilityTimelineEditorWindow.Instance.Save();
            clipData = updatedClip;
        }

        public override void OnTickView(int frameIndex, int startFrame, int endFrame)
        {
            if (frameIndex < startFrame || frameIndex > endFrame) return;
            var ongoingAbilityTask = TaskClipData.Load();
            ongoingAbilityTask.OnEditorPreview( frameIndex, startFrame, endFrame);
        }

        public override UnityEngine.Object DataInspector => TaskClipEditor.Create(this);
    }
}
#endif
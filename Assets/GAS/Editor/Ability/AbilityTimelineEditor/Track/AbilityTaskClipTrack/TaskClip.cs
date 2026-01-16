
#if UNITY_EDITOR
namespace GAS.Editor
{
    using System.Linq;
    using GAS.Runtime;

    
    public class TaskClip : TrackClip<TaskClipEventTrack>
    {
        private XParamTimeline AbilityConfig => AbilityTimelineEditorWindow.Instance.AbilityConfig;
        public TaskClipData TaskClipData => null;//clipData as TaskClipEvent;

        public TaskClipData ClipDataForSave
        {
            get
            {
                var cueTrackDataForSave = track.TaskClipTrackDataForSave;
                for (var i = 0; i < cueTrackDataForSave.clipDatas.Count; i++)
                    if (cueTrackDataForSave.clipDatas[i] == TaskClipData)
                        return track.TaskClipTrackDataForSave.clipDatas[i];
                return null;
            }
        }
        
        
        public override void Delete()
        {
            var success = track.TaskClipTrackDataForSave.clipDatas.Remove(TaskClipData);
            AbilityTimelineEditorWindow.Instance.Save();
            if (!success) return;
            track.RemoveTrackItem(this);
            AbilityTimelineEditorWindow.Instance.SetInspector();
        }

        public override void RefreshShow(float newFrameUnitWidth)
        {
            base.RefreshShow(newFrameUnitWidth);
            // var taskType = TaskClipData.ongoingTask.TaskData.Type;
            // var shortName = taskType.Split('.').Last();
            // ItemLabel.text = !string.IsNullOrEmpty(shortName) ? shortName : "Null!";
        }

        public override void UpdateClipDataStartFrame(int newStartFrame)
        {
            var updatedClip = ClipDataForSave;
            ClipDataForSave.StartTime = newStartFrame;
            AbilityTimelineEditorWindow.Instance.Save();
            //clipData = updatedClip;
        }

        public override void UpdateClipDataDurationFrame(int newDurationFrame)
        {
            var updatedClip = ClipDataForSave;
            ClipDataForSave.EndTime = ClipDataForSave.StartTime + newDurationFrame;
            AbilityTimelineEditorWindow.Instance.Save();
            //clipData = updatedClip;
        }

        public override void OnTickView(int frameIndex, int startFrame, int endFrame)
        {
            if (frameIndex < startFrame || frameIndex > endFrame) return;
            var ongoingAbilityTask = TaskClipData.CreateTaskInEditor();
            ongoingAbilityTask.OnEditorPreview( frameIndex, startFrame, endFrame);
        }

        public override UnityEngine.Object DataInspector => TaskClipEditor.Create(this);
    }
}
#endif
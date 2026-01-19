using GAS.Runtime;
using UnityEngine;

#if UNITY_EDITOR
namespace GAS.Editor
{
    public class TaskClip : TrackClip<TaskClipEventTrack>
    {
        private XParamTimeline AbilityConfig => AbilityTimelineEditorWindow.Instance.AbilityConfig;

        public override Object DataInspector => TaskClipEditor.Create(this);


        public override void Delete()
        {
            var success = track.TrackData.TaskClips.Remove(TaskClipData);
            AbilityTimelineEditorWindow.Instance.Save();
            if (!success) return;
            track.RemoveTrackItem(this);
            AbilityTimelineEditorWindow.Instance.SetInspector();
        }

        public override void RefreshShow(float newFrameUnitWidth)
        {
            base.RefreshShow(newFrameUnitWidth);
            ItemLabel.text = TaskClipData.Name;
        }

        public override void UpdateClipDataStartFrame(int newStartFrame)
        {
            TaskClipData.StartTime = newStartFrame;
            AbilityTimelineEditorWindow.Instance.Save();
        }

        public override void UpdateClipDataEndFrame(int endFrame)
        {
            TaskClipData.EndTime = endFrame;
            AbilityTimelineEditorWindow.Instance.Save();
        }

        public override void OnTickView(int frameIndex, int startFrame, int endFrame)
        {
            if (frameIndex < startFrame || frameIndex > endFrame) return;
            var task = EditorAbilityHelper.CreateTaskInEditor(TaskClipData.TaskType, AbilityConfig,
                TaskClipData.Parameter);
            task.OnEditorPreview(frameIndex, startFrame, endFrame);
        }
    }
}
#endif
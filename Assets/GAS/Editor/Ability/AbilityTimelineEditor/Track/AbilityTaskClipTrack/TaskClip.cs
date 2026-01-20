using GAS.Runtime;
using UnityEngine;
using UnityEngine.UIElements;

#if UNITY_EDITOR
namespace GAS.Editor
{
    public class TaskClip : TrackClipBase
    {
        private XParamTimeline AbilityConfig => AbilityTimelineEditorWindow.Instance.AbilityConfig;

        public override Object DataInspector => TaskClipEditor.Create(this);

        protected AbilityTimelineTrack Track;

        public override void InitTrackClip(
            AbilityTimelineTrack track,
            VisualElement parent,
            float frameUnitWidth,
            TaskClipData taskClipDataData)
        {
            Track = track;
            base.InitTrackClip(track, parent, frameUnitWidth, taskClipDataData);

            RefreshShow(FrameUnitWidth);
        }

        public override void Delete()
        {
            var success = Track.TrackData.TaskClips.Remove(TaskClipData);
            AbilityTimelineEditorWindow.Instance.Save();
            if (!success) return;
            Track.RemoveTrackItem(this);
            AbilityTimelineEditorWindow.Instance.SetInspector();
        }

        public override void RefreshShow(float newFrameUnitWidth)
        {
            base.RefreshShow(newFrameUnitWidth);
            ClipVe.UpdateState(TaskClipData.StartTime == TaskClipData.EndTime);
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
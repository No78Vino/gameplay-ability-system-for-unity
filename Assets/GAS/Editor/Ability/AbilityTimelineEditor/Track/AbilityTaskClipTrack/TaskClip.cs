using GAS.Runtime;
using UnityEngine;

#if UNITY_EDITOR
namespace GAS.Editor
{
    using UnityEngine.UIElements;
    
    public class TaskClip
    {
        private VisualElement _ve;
        public VisualElement Ve => _ve;
        
        public TaskClipData TaskClipData { get;private set; }

        public TrackClipVisualElement ClipVe => _ve as TrackClipVisualElement;
        public float FrameUnitWidth { get; protected set; }
        public int StartFrameIndex => TaskClipData.StartTime;
        public int EndFrameIndex => TaskClipData.EndTime;
        public int DurationFrame => EndFrameIndex - StartFrameIndex;

        public Label ItemLabel => ClipVe.ItemLabel;
        
        private XParamTimeline AbilityConfig => AbilityTimelineEditorWindow.Instance.AbilityConfig;

        public Object DataInspector => TaskClipEditor.Create(this);

        private AbilityTimelineTrack _track;

        public void InitTrackClip(
            AbilityTimelineTrack track,
            VisualElement parent,
            float frameUnitWidth,
            TaskClipData taskClipDataData)
        {
            _track = track;
            
            FrameUnitWidth = frameUnitWidth;
            TaskClipData = taskClipDataData;

            _ve = new TrackClipVisualElement();
            ClipVe.InitClipInfo(this);
            parent.Add(_ve);
            if (AbilityTimelineEditorWindow.Instance.CurrentInspectorObject is TaskClip clipBase &&
                taskClipDataData == clipBase.TaskClipData)
                ClipVe.OnSelect();
            else
                ClipVe.OnUnSelect();
  
            RefreshShow(FrameUnitWidth);
        }

        public void Delete()
        {
            var success = _track.TrackData.TaskClips.Remove(TaskClipData);
            if (!success) return;
            _track.RemoveTrackItem(this);
            AbilityTimelineEditorWindow.Instance.SetInspector();
        }

        public void RefreshShow(float newFrameUnitWidth)
        {
            FrameUnitWidth = newFrameUnitWidth;
            // clip位置，宽度
            var mainPos = _ve.transform.position;
            mainPos.x = StartFrameIndex * FrameUnitWidth;
            _ve.transform.position = mainPos;
            _ve.style.width = DurationFrame * FrameUnitWidth;
            
            ClipVe.UpdateState(TaskClipData.StartTime == TaskClipData.EndTime);
            ItemLabel.text = TaskClipData.Name;
        }

        public void UpdateClipDataStartFrame(int newStartFrame)
        {
            TaskClipData.StartTime = newStartFrame;
        }

        public void UpdateClipDataEndFrame(int endFrame)
        {
            TaskClipData.EndTime = endFrame;
        }

        public void OnTickView(int frameIndex, int startFrame, int endFrame)
        {
            if (frameIndex < startFrame || frameIndex > endFrame) return;
            var task = EditorAbilityHelper.CreateTaskInEditor(TaskClipData.TaskType, AbilityConfig,
                TaskClipData.Parameter);
            task.OnEditorPreview(AbilityTimelineEditorWindow.Instance.PreviewObject,frameIndex, startFrame, endFrame);
        }
    }
}
#endif
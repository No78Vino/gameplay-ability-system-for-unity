using GAS.Runtime;

#if UNITY_EDITOR
namespace GAS.Editor
{
    using UnityEngine.UIElements;
    
    public abstract class TrackClipBase : TrackItemBase
    {
        public TaskClipData TaskClipData { get;private set; }

        protected TrackBase trackBase;

        //protected TrackClipVisualElement ve;
        public TrackClipVisualElement ClipVe => ve as TrackClipVisualElement;
        public float FrameUnitWidth { get; protected set; }
        public int StartFrameIndex => TaskClipData.StartTime;
        public int EndFrameIndex => TaskClipData.EndTime;
        public int DurationFrame => EndFrameIndex - StartFrameIndex;

        public Label ItemLabel => ClipVe.ItemLabel;
        public VisualElement Mark => ClipVe.Mark;

        public virtual void InitTrackClip(
            TrackBase track,
            VisualElement parent,
            float frameUnitWidth,
            TaskClipData taskClipDataData)
        {
            trackBase = track;
            FrameUnitWidth = frameUnitWidth;
            TaskClipData = taskClipDataData;

            ve = new TrackClipVisualElement();
            ClipVe.InitClipInfo(this);
            parent.Add(ve);
            if (AbilityTimelineEditorWindow.Instance.CurrentInspectorObject is TrackClipBase clipBase &&
                taskClipDataData == clipBase.TaskClipData)
                ClipVe.OnSelect();
            else
                ClipVe.OnUnSelect();
        }

        //public abstract VisualElement Inspector();
        public virtual UnityEngine.Object DataInspector=>null;

        public abstract void Delete();

        public virtual void RefreshShow(float newFrameUnitWidth)
        {
            FrameUnitWidth = newFrameUnitWidth;

            // clip位置，宽度
            var mainPos = ve.transform.position;
            mainPos.x = StartFrameIndex * FrameUnitWidth;
            ve.transform.position = mainPos;
            ve.style.width = DurationFrame * FrameUnitWidth;
        }

        public abstract void UpdateClipDataStartFrame(int newStartFrame);


        public abstract void UpdateClipDataEndFrame(int endFrame);

        public abstract void OnTickView(int frameIndex, int startFrame, int endFrame);
    }

    public abstract class TrackClip<T> : TrackClipBase where T : TrackBase
    {
        protected T track;

        public override void InitTrackClip(
            TrackBase track,
            VisualElement parent,
            float frameUnitWidth,
            Runtime.TaskClipData taskClipDataData)
        {
            this.track = (T)track;
            base.InitTrackClip(track, parent, frameUnitWidth, taskClipDataData);

            RefreshShow(FrameUnitWidth);
        }
    }
}
#endif
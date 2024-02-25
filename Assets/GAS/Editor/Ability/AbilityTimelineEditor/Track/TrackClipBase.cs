using GAS.Runtime.Ability.AbilityTimeline;
using UnityEngine;
using UnityEngine.UIElements;

namespace GAS.Editor.Ability.AbilityTimelineEditor.Track
{
    public abstract class TrackClipBase
    {
        protected TrackClipVisualElement ve;
        public float FrameUnitWidth { get;protected set; }
        public int StartFrameIndex=>clipData.startFrame;
        public int EndFrameIndex=>clipData.EndFrame;
        public int DurationFrame => clipData.durationFrame;
        protected ClipEventBase clipData;
        protected TrackBase trackBase;
        
        public virtual void InitTrackClip(
            TrackBase track,
            VisualElement parent,
            float frameUnitWidth,
            ClipEventBase clipData)
        {
            trackBase = track;
            FrameUnitWidth = frameUnitWidth;
            this.clipData = clipData;
            
            ve = new TrackClipVisualElement();
            ve.InitClipInfo(this);
            parent.Add(ve);
            ve.OnUnSelect();
        }

        public virtual VisualElement Inspector()
        {
            return null;
        }

        public virtual void Delete()
        {
        }

        public virtual void RefreshShow(float newFrameUnitWidth)
        {
            FrameUnitWidth = newFrameUnitWidth;
        }

        protected void CheckStyle(VisualElement element)
        {
            element.style.fontSize = 14;
            //element.style.width = new StyleLength(1);
        }
    }
    
    public abstract class TrackClip<T>:TrackClipBase where T : TrackBase
    {
        protected T track;

        public override void InitTrackClip(
            TrackBase track,
            VisualElement parent,
            float frameUnitWidth,
            ClipEventBase clipData)
        {
            this.track = (T) track;
            base.InitTrackClip(track, parent, frameUnitWidth, clipData);
        }
       
    }

}
using GAS.Runtime.Ability.AbilityTimeline;
using UnityEngine;
using UnityEngine.UIElements;

namespace GAS.Editor.Ability.AbilityTimelineEditor.Track
{
    public abstract class TrackClipBase
    {
        protected TrackClipVisualElement ve;
        public TrackClipVisualElement Ve => ve;
        public float FrameUnitWidth { get;protected set; }
        public int StartFrameIndex=>clipData.startFrame;
        public int EndFrameIndex=>clipData.EndFrame;
        public int DurationFrame => clipData.durationFrame;
        protected ClipEventBase clipData;
        protected TrackBase trackBase;
        public TrackBase TrackBase => trackBase;
        
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

        public void UpdateClipDataStartFrame(int newStartFrame)
        {
            clipData.startFrame = newStartFrame;
        }
        
        #region Visual Element Event
        
        public void OnMainMouseDown(MouseDownEvent evt)
        {
        }

        public void OnMainMouseUp(MouseUpEvent evt)
        {
        }

        public void OnMainMouseMove(MouseMoveEvent evt)
        {
            // if (_dragging)
            // {
            //     var offset = evt.mousePosition.x - _startDragX;
            //     var offsetFrame = Mathf.RoundToInt(offset / FrameUnitWidth);
            //     var targetFrame = _startDragFrameIndex + offsetFrame;
            //     if (offsetFrame == 0 || targetFrame < 0) return;

                // var checkDrag = offsetFrame > 0
                //     ? trackBase.CheckFrameIndexOnDrag(targetFrame + DurationFrame)
                //     : trackBase.CheckFrameIndexOnDrag(targetFrame);
                //
                // if (checkDrag)
                // {
                //     StartFrameIndex = targetFrame;
                //     if (EndFrameIndex > AbilityTimelineEditorWindow.Instance.AbilityAsset.MaxFrameCount)
                //         AbilityTimelineEditorWindow.Instance.CurrentSelectFrameIndex = EndFrameIndex;
                //     RefreshShow(FrameUnitWidth);
                //     AbilityTimelineEditorWindow.Instance.SetInspector(this);
                // }
            //}
        }

        public void OnMainMouseOut(MouseOutEvent evt)
        {
        }
        
        #endregion
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
            
            RefreshShow(FrameUnitWidth);
        }
       
    }

}
#if UNITY_EDITOR
namespace GAS.Editor
{
    using Runtime;
    using UnityEngine;
    
    using UnityEngine.UIElements;
    public abstract class TrackMarkBase:TrackItemBase
    {
        protected static XParamTimeline AbilityAsset => AbilityTimelineEditorWindow.Instance.AbilityConfig;
        private static string MarkAssetGuid => "5a3b3360bcba29b4cac2875f518af19d";
        public float FrameUnitWidth { get;protected set; }
        public int StartFrameIndex=>markData.StartTime;
        
        protected TaskClipData markData;

        protected TrackBase trackBase;
        public TrackBase TrackBase => trackBase;

        private DragAreaManipulator MarkDragAreaManipulator;
        public Label ItemLabel { get;protected set; }

        //public abstract VisualElement Inspector();
        public abstract UnityEngine.Object DataInspector { get; }
        
        public abstract void Delete();
        public abstract void Duplicate();

        public virtual void RefreshShow(float newFrameUnitWidth)
        {
            FrameUnitWidth = newFrameUnitWidth;
            
            // 位置
            var mainPos = ve.transform.position;
            mainPos.x = StartFrameIndex * FrameUnitWidth - 7.5f; // - ve.worldBound.width * 0.5f;
            ve.transform.position = mainPos;
        }
        
        private void OnContextMenu(ContextualMenuPopulateEvent obj)
        {
            obj.menu.AppendAction("Delete Mark", _ => Delete());
            obj.menu.AppendAction("Duplicate", _ => Duplicate());
        }
        
        public abstract void UpdateMarkDataFrame(int newStartFrame);
        
        public abstract void OnTickView(int frameIndex);
        
        
        #region Mouse Event
        
        private TimerShaftView TimerShaftView => AbilityTimelineEditorWindow.Instance.TimerShaftView;
        private float _lastMainDragStartPos;
        private float _newStartFramePos;
        private int NewStartFrame => (int)_newStartFramePos;

        private void OnMainMouseDown(PointerDownEvent evt)
        {
            _lastMainDragStartPos = StartFrameIndex;
            OnSelect();
        }

        private void OnMainMouseUp()
        {
            if(TimerShaftView.DottedLineFrameIndex == -1) return;
            
            ApplyMarkDrag();
            TimerShaftView.DottedLineFrameIndex = -1;
        }
        
        private void OnMainMouseMove(Vector2 delta)
        {
            var offsetFrame = delta.x / FrameUnitWidth;
            _newStartFramePos = _lastMainDragStartPos + offsetFrame;
            if (offsetFrame == 0 || _newStartFramePos < 0) return;
            int minFrame =  0;
            int maxFrame =  AbilityTimelineEditorWindow.Instance.AbilityConfig.LifeTime;
            if (NewStartFrame >= minFrame && NewStartFrame <= maxFrame)
            {
                TimerShaftView.DottedLineFrameIndex = NewStartFrame;
            }
        }
        
        private void ApplyMarkDrag()
        {
            int minFrame =  0;
            int maxFrame = AbilityTimelineEditorWindow.Instance.AbilityConfig.LifeTime;
            var newStartFrame = Mathf.Clamp(NewStartFrame, minFrame, maxFrame);
            if (newStartFrame == StartFrameIndex) return;
            
            UpdateMarkDataFrame(newStartFrame);
            RefreshShow(FrameUnitWidth);
            
            AbilityTimelineEditorWindow.Instance.TimelineInspector.RefreshInspector();
        }
        
        #endregion
        
        #region Select
        public bool Selected { get; private set; }
        private static Color SelectedColor = new Color(0.8f, 0.6f, 0.3f, 1f);
        private static Color UnSelectedColor = new Color(1f, 1f, 1f, 0.9f);
        public void OnSelect()
        {
            Selected = true;
            AbilityTimelineEditorWindow.Instance.SetInspector(this);
            ve.style.unityBackgroundImageTintColor = SelectedColor;
        }

        public void OnUnSelect()
        {
            Selected = false;
            ve.style.unityBackgroundImageTintColor = UnSelectedColor;
        }

        #endregion
    }
}

#endif
#if UNITY_EDITOR
namespace GAS.Editor
{
    using Runtime;
    using UnityEditor;
    using UnityEngine;
    
    using UnityEngine.UIElements;
    public abstract class TrackMarkBase:TrackItemBase
    {
        protected static TimelineAbilityAssetBase AbilityAsset => AbilityTimelineEditorWindow.Instance.AbilityAsset;
        private static string MarkAssetGuid => "5a3b3360bcba29b4cac2875f518af19d";
        public float FrameUnitWidth { get;protected set; }
        public int StartFrameIndex=>markData.startFrame;
        
        protected MarkEventBase markData;
        public MarkEventBase MarkData => markData;
        
        protected TrackBase trackBase;
        public TrackBase TrackBase => trackBase;

        private DragAreaManipulator MarkDragAreaManipulator;
        public Label ItemLabel { get;protected set; }

        public virtual void InitTrackMark(
            TrackBase track,
            VisualElement parent,
            float frameUnitWidth,
            MarkEventBase markData)
        {
            trackBase = track;
            FrameUnitWidth = frameUnitWidth;
            this.markData = markData;
            
            string markAssetPath = AssetDatabase.GUIDToAssetPath(MarkAssetGuid);
            ve = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(markAssetPath).Instantiate().Query().ToList()[1];
            ItemLabel = ve.Q<Label>("MarkText");
            parent.Add(ve);
            MarkDragAreaManipulator = new DragAreaManipulator(MouseCursorType.None, OnMainMouseMove,
                OnMainMouseDown, OnMainMouseUp);
            ve.AddManipulator(MarkDragAreaManipulator);
            ve.AddManipulator(new ContextualMenuManipulator(OnContextMenu));
            
            
            if(AbilityTimelineEditorWindow.Instance.CurrentInspectorObject is TrackMarkBase markBase &&
               markData == markBase.markData)
                OnSelect();
            else
                OnUnSelect();
        }

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
            int maxFrame =  AbilityTimelineEditorWindow.Instance.AbilityAsset.FrameCount;
            if (NewStartFrame >= minFrame && NewStartFrame <= maxFrame)
            {
                TimerShaftView.DottedLineFrameIndex = NewStartFrame;
            }
        }
        
        private void ApplyMarkDrag()
        {
            int minFrame =  0;
            int maxFrame = AbilityTimelineEditorWindow.Instance.AbilityAsset.FrameCount;
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
    
    public abstract class TrackMark<T>:TrackMarkBase where T : TrackBase
    {
        protected T track;

        public override void InitTrackMark(
            TrackBase track,
            VisualElement parent,
            float frameUnitWidth,
            MarkEventBase markData)
        {
            this.track = (T) track;
            base.InitTrackMark(track, parent, frameUnitWidth, markData);
            
            RefreshShow(FrameUnitWidth);
        }
    }
}

#endif
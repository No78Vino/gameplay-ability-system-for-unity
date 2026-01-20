#if UNITY_EDITOR
namespace GAS.Editor
{
    using UnityEditor;
    using UnityEngine;
    using UnityEngine.UIElements;
    
    public class TrackClipVisualElement : VisualElement
    {
        private TaskClip _clip;
        private readonly DragAreaManipulator _leftDragAreaManipulator;
        private readonly PointerIMGUIContainer _leftResizeArea;
        private readonly VisualElement _mainArea;

        private readonly DragAreaManipulator _mainAreaDragAreaManipulator;
        private readonly DragAreaManipulator _markAreaDragAreaManipulator;

        private readonly VisualElement _outsideBox;
        private readonly DragAreaManipulator _rightDragAreaManipulator;
        private readonly IMGUIContainer _rightResizeArea;
        private VisualElement _selectedBottomLine;

        public TrackClipVisualElement()
        {
            var visualTree =
                AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(AssetDatabase.GUIDToAssetPath(ItemAssetGUID));
            visualTree.CloneTree(this);
            AddToClassList("clip");

            _outsideBox = this.Q<VisualElement>("OutsideBox");
            ItemLabel = this.Q<Label>("ItemLabel");
            OverLine = this.Q<VisualElement>("OverLine");
            _selectedBottomLine = this.Q<VisualElement>("SelectedBottomLine");
            Mark = this.Q<VisualElement>("Mark");
            _mainArea = this.Q<VisualElement>("Main");
            _leftResizeArea = this.Q<PointerIMGUIContainer>("LeftResizeArea");
            _rightResizeArea = this.Q<IMGUIContainer>("RightResizeArea");


            _mainAreaDragAreaManipulator = new DragAreaManipulator(MouseCursorType.None, OnMainMouseMove,
                OnMainMouseDown, OnMainMouseUp);
            _mainArea.AddManipulator(_mainAreaDragAreaManipulator);
            
            _markAreaDragAreaManipulator = new DragAreaManipulator(MouseCursorType.None, OnMainMouseMove,
                OnMainMouseDown, OnMainMouseUp);
            Mark.AddManipulator(_markAreaDragAreaManipulator);

            _leftDragAreaManipulator = new DragAreaManipulator(MouseCursorType.ResizeHorizontal, OnLeftResizeDragMove,
                OnLeftResizeDragStart, OnLeftResizeDragEnd);
            _leftResizeArea.AddManipulator(_leftDragAreaManipulator);

            _rightDragAreaManipulator = new DragAreaManipulator(MouseCursorType.ResizeHorizontal, OnRightResizeDragMove,
                OnRightResizeDragStart, OnRightResizeDragEnd);
            _rightResizeArea.AddManipulator(_rightDragAreaManipulator);

            _mainArea.AddManipulator(new ContextualMenuManipulator(OnContextMenu));
            Mark.AddManipulator(new ContextualMenuManipulator(OnContextMenu));
        }

        protected string ItemAssetGUID => "3197d239f4ce79b41b2278ecea5aaab8";

        public Label ItemLabel { get; }

        public VisualElement OverLine { get; }

        public VisualElement Mark { get; }
        
        private float FrameUnitWidth => _clip.FrameUnitWidth;
        private int StartFrameIndex => _clip.StartFrameIndex;
        private int EndFrameIndex => _clip.EndFrameIndex;
        private int DurationFrame => _clip.DurationFrame;

        public bool Selected { get; private set; }
        public bool Hovered { get; private set; }

        private TimerShaftView TimerShaftView => AbilityTimelineEditorWindow.Instance.TimerShaftView;

        private void OnContextMenu(ContextualMenuPopulateEvent obj)
        {
            obj.menu.AppendAction("删除任务", action => _clip.Delete());
        }

        public void InitClipInfo(TaskClip trackClipBase)
        {
            _clip = trackClipBase;
        }

        /// <summary>
        /// 切换形态：MARK / CLIP， 标记/片段  两种情况
        /// </summary>
        /// <param name="mark"></param>
        public void UpdateState(bool mark)
        {
            Mark.visible = mark;
            _leftResizeArea.visible = !mark;
            _rightResizeArea.visible = !mark;
            _leftDragAreaManipulator.Enable = !mark;
            _rightDragAreaManipulator.Enable = !mark;
        }
        
        public new class UxmlFactory : UxmlFactory<TrackClipVisualElement, UxmlTraits>
        {
        }

        #region Main Area Mouse Event
        
        private float _lastMainDragStartPos;
        private float _newStartFramePos;
        private int NewStartFrame => (int)_newStartFramePos;
        

        protected void OnMainMouseDown(PointerDownEvent evt)
        {
            _lastMainDragStartPos = StartFrameIndex;
            OnSelect();
        }

        protected void OnMainMouseUp()
        {
            if (TimerShaftView.ShowDragItemPreview == false) return;

            OnMainAreaApplyDrag();
            TimerShaftView.ShowDragItemPreview = false;
            TimerShaftView.DottedLineFrameIndex = -1;
        }

        private void OnMainMouseMove(Vector2 delta)
        {
            var offsetFrame = delta.x / FrameUnitWidth;
            _newStartFramePos = _lastMainDragStartPos + offsetFrame;
            if (offsetFrame == 0 || _newStartFramePos < 0) return;
            var minFrame = 0;
            var maxFrame = AbilityTimelineEditorWindow.Instance.AbilityConfig.LifeTime;
            if (NewStartFrame >= minFrame && NewStartFrame + DurationFrame <= maxFrame)
            {
                var mainContent = TimerShaftView.MainContent;
                var bound = _mainArea.worldBound;
                var rectOriginX = NewStartFrame * FrameUnitWidth - AbilityTimelineEditorWindow.Instance.CurrentFramePos;
                bound.y = bound.y - mainContent.worldBound.y + bound.height;
                bound.x = Mathf.Clamp(rectOriginX, 0, parent.worldBound.width);
                if (rectOriginX < 0)
                    bound.width += rectOriginX;
                if (bound.width + bound.x > mainContent.worldBound.width - 8)
                    bound.width = mainContent.worldBound.width - bound.x - 8; // 8 = 滑动条宽度
                TimerShaftView.ShowDragItemPreview = true;
                TimerShaftView.DragItemPreviewRect = bound;
                TimerShaftView.DottedLineFrameIndex = NewStartFrame;
            }
        }

        private void OnMainAreaApplyDrag()
        {
            var newStartFrame = Mathf.Clamp(NewStartFrame, 0, 
                AbilityTimelineEditorWindow.Instance.AbilityConfig.LifeTime - DurationFrame);
            var newEndFrame = newStartFrame + DurationFrame;
            _clip.UpdateClipDataStartFrame(newStartFrame);
            _clip.UpdateClipDataEndFrame(newEndFrame);
            _clip.RefreshShow(FrameUnitWidth);

            AbilityTimelineEditorWindow.Instance.TimelineInspector.RefreshInspector();
        }

        #endregion

        #region Hover And Select

        protected static readonly Color NormalColor = new(0.3f, 0.3f, 0.5f, 0.75f);
        protected static readonly Color SelectColor = new(0.6f, 0.1f, 0.1f, 0.75f);
        protected static readonly Color OutsideBoxSelectColor = new(0.8f, 0.5f, 0.1f, 1f);
        protected static readonly Color OutsideBoxHoverColor = new(0.8f, 0.8f, 0.7f, 0.9f);
        protected static readonly Color MarkNormalColor = new(1f, 1f, 1f, 1f);
        protected static readonly Color MarkSelectColor = new(0.7f, 1f, 0.7f, 1f);
        protected static readonly Color MarkHoverColor = new(0.7f, 1f, 1f, 1f);

        private const int TipBoundingSize = 4;

        public bool InClipRect(Vector2 position)
        {
            var rect = _mainArea.worldBound;
            return rect.Contains(position);
        }

        public void SwitchBounding()
        {
            var isShow = Hovered || Selected;
            if (isShow)
            {
                var color = Selected ? OutsideBoxSelectColor : OutsideBoxHoverColor;
                _outsideBox.style.backgroundColor = new StyleColor(color);
                _outsideBox.style.width = new StyleLength(_mainArea.worldBound.width + TipBoundingSize);
                _outsideBox.style.height = new StyleLength(_mainArea.worldBound.height + TipBoundingSize);
                _outsideBox.style.display = DisplayStyle.Flex;
                _outsideBox.MarkDirtyRepaint();
                
                var markColor = Selected ? MarkSelectColor : MarkHoverColor;
                Mark.style.unityBackgroundImageTintColor = markColor;
                Mark.MarkDirtyRepaint();
            }
            else
            {
                _outsideBox.style.display = DisplayStyle.None;
                _outsideBox.MarkDirtyRepaint();
            }
        }

        public void OnHover(bool value)
        {
            Hovered = value switch
            {
                true when !Hovered && !Selected => true,
                false when Hovered => false,
                _ => Hovered
            };
            SwitchBounding();
        }

        public virtual void OnSelect()
        {
            AbilityTimelineEditorWindow.Instance.SetInspector(_clip);
            Selected = true;
            ItemLabel.style.backgroundColor = SelectColor;
            Mark.style.unityBackgroundImageTintColor = MarkSelectColor;
            SwitchBounding();
        }

        public virtual void OnUnSelect()
        {
            Selected = false;
            ItemLabel.style.backgroundColor = NormalColor;
            Mark.style.unityBackgroundImageTintColor = MarkNormalColor;
            SwitchBounding();
        }

        #endregion

        #region Clip Resize Area

        private float _lastResizeDragStartPos;
        private float _newResizeStartFramePos;
        private int NewResizeStartFrame => (int)_newResizeStartFramePos;

        private float _lastResizeDragEndPos;
        private float _newResizeEndFramePos;
        private int NewResizeEndFrame => (int)_newResizeEndFramePos;

        private void OnLeftResizeDragMove(Vector2 delta)
        {
            var offsetFrame = delta.x / FrameUnitWidth;
            _newResizeStartFramePos = _lastResizeDragStartPos + offsetFrame;

            if (offsetFrame == 0 || _newResizeStartFramePos < 0 || NewResizeStartFrame + 1 >= EndFrameIndex) return;
            
            if (NewResizeStartFrame >= 0 && NewResizeStartFrame <= EndFrameIndex - 1)
                TimerShaftView.DottedLineFrameIndex = NewResizeStartFrame;
        }

        private void OnLeftResizeDragStart(PointerDownEvent evt)
        {
            _lastResizeDragStartPos = StartFrameIndex;
        }

        private void OnLeftResizeDragEnd()
        {
            var newStartFrame = Mathf.Clamp(NewResizeStartFrame, 0, EndFrameIndex);

            _clip.UpdateClipDataStartFrame(newStartFrame);
            _clip.UpdateClipDataEndFrame(EndFrameIndex);
            if (EndFrameIndex > AbilityTimelineEditorWindow.Instance.AbilityConfig.LifeTime)
                AbilityTimelineEditorWindow.Instance.CurrentSelectFrameIndex = EndFrameIndex;
            _clip.RefreshShow(FrameUnitWidth);
            AbilityTimelineEditorWindow.Instance.SetInspector(_clip);
            TimerShaftView.DottedLineFrameIndex = -1;

            AbilityTimelineEditorWindow.Instance.TimelineInspector.RefreshInspector();
        }

        private void OnRightResizeDragMove(Vector2 delta)
        {
            var offsetFrame = delta.x / FrameUnitWidth;
            _newResizeEndFramePos = _lastResizeDragEndPos + offsetFrame;
            if (offsetFrame == 0 || _newResizeEndFramePos < 0 || NewResizeEndFrame - 1 <= StartFrameIndex) return;

            var maxFrame = AbilityTimelineEditorWindow.Instance.AbilityConfig.LifeTime;
            var minFrame = _clip.StartFrameIndex + 1;
            if (NewResizeEndFrame >= minFrame && NewResizeEndFrame <= maxFrame)
                TimerShaftView.DottedLineFrameIndex = NewResizeEndFrame;
        }

        private void OnRightResizeDragStart(PointerDownEvent evt)
        {
            _lastResizeDragEndPos = EndFrameIndex;
        }

        private void OnRightResizeDragEnd()
        {
            var maxFrame = AbilityTimelineEditorWindow.Instance.AbilityConfig.LifeTime;
            var newEndFrame = Mathf.Clamp(NewResizeEndFrame, StartFrameIndex, maxFrame);
            _clip.UpdateClipDataEndFrame(newEndFrame);
            if (EndFrameIndex > AbilityTimelineEditorWindow.Instance.AbilityConfig.LifeTime)
                AbilityTimelineEditorWindow.Instance.CurrentSelectFrameIndex = EndFrameIndex;
            _clip.RefreshShow(FrameUnitWidth);
            AbilityTimelineEditorWindow.Instance.SetInspector(_clip);
            TimerShaftView.DottedLineFrameIndex = -1;

            AbilityTimelineEditorWindow.Instance.TimelineInspector.RefreshInspector();
        }

        #endregion
    }
}
#endif
using System;
using GAS.Runtime.Ability.AbilityTimeline;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UIElements;

#if UNITY_EDITOR
namespace GAS.Editor.Ability.AbilityTimelineEditor.Track
{
    public class TrackClipVisualElement:VisualElement
    {
        public new class UxmlFactory : UxmlFactory<TrackClipVisualElement, UxmlTraits> { }
        protected string ItemAssetGUID => "3197d239f4ce79b41b2278ecea5aaab8";

        private VisualElement _outsideBox;
        private Label _itemLabel;
        private VisualElement _overLine;
        private VisualElement _selectedBottomLine;
        private VisualElement _mainArea;
        private PointerIMGUIContainer _leftResizeArea;
        private IMGUIContainer _rightResizeArea;

        private ResizeBoundingManipulator _leftResizeBoundingManipulator;
        private ResizeBoundingManipulator _rightResizeBoundingManipulator;
            
        public Label ItemLabel => _itemLabel;
        public VisualElement OverLine => _overLine;

        private TrackClipBase _clip;
        private float FrameUnitWidth=>_clip.FrameUnitWidth;
        private int StartFrameIndex=>_clip.StartFrameIndex;
        private int EndFrameIndex => _clip.EndFrameIndex;
        private int DurationFrame => _clip.DurationFrame;
        
        private int _startDragFrameIndex;
        private float _startDragX;
        private bool _dragging;

        public bool Selected { get; private set; }
        public bool Hovered { get; private set; }
        
        #region Visual Element Event

        public EventCallback<MouseDownEvent> onMainMouseDown;
        public EventCallback<MouseUpEvent> onMainMouseUp;
        public EventCallback<MouseMoveEvent> onMainMouseMove;
        public EventCallback<MouseOutEvent> onMainMouseOut;

        #endregion
        public TrackClipVisualElement()
        {
            var visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(AssetDatabase.GUIDToAssetPath(ItemAssetGUID));
            visualTree.CloneTree(this);
            AddToClassList("clip");
            
            _outsideBox = this.Q<VisualElement>("OutsideBox");
            _itemLabel = this.Q<Label>("ItemLabel");
            _overLine = this.Q<VisualElement>("OverLine");
            _selectedBottomLine = this.Q<VisualElement>("SelectedBottomLine");
            _mainArea = this.Q<VisualElement>("Main");
            _leftResizeArea = this.Q<PointerIMGUIContainer>("LeftResizeArea");
            _rightResizeArea = this.Q<IMGUIContainer>("RightResizeArea");
            
            
            _mainArea.RegisterCallback<MouseDownEvent>(OnMainMouseDown);
            _mainArea.RegisterCallback<MouseUpEvent>(OnMainMouseUp);
            _mainArea.RegisterCallback<MouseMoveEvent>(OnMainMouseMove);
            _mainArea.RegisterCallback<MouseOutEvent>(OnMainMouseOut);
            
            _leftResizeBoundingManipulator = new ResizeBoundingManipulator(OnLeftResizeDragMove, OnLeftResizeDragStart, OnLeftResizeDragEnd);
            _leftResizeArea.AddManipulator(_leftResizeBoundingManipulator);
         
            _rightResizeBoundingManipulator = new ResizeBoundingManipulator(OnRightResizeDragMove, OnRightResizeDragStart, OnRightResizeDragEnd);
            _rightResizeArea.AddManipulator(_rightResizeBoundingManipulator);
            
            
            EditorGUIUtility.AddCursorRect(new Rect(0,0,600,600), MouseCursor.ResizeHorizontal);
        }

        public void InitClipInfo(TrackClipBase trackClipBase)
        {
            _clip = trackClipBase;
            onMainMouseDown=_clip.OnMainMouseDown;
            onMainMouseUp=_clip.OnMainMouseUp;
            onMainMouseMove=_clip.OnMainMouseMove;
            onMainMouseOut=_clip.OnMainMouseOut;
        }
        
        #region Mouse Event

        protected void OnMainMouseDown(MouseDownEvent evt)
        {
            _dragging = true;
            _startDragFrameIndex = StartFrameIndex;
            _startDragX = evt.mousePosition.x;
            OnSelect();
            
            Hovered = true;
        }
        
        protected void OnMainMouseUp(MouseUpEvent evt)
        {
            _dragging = false;
            OnMainAreaApplyDrag();
            
            Hovered = false;
        }
        
        protected void OnMainMouseMove(MouseMoveEvent evt)
        {
            if (_dragging)
            {
                var offset = evt.mousePosition.x - _startDragX;
                var offsetFrame = Mathf.RoundToInt(offset / FrameUnitWidth);
                var targetFrame = _startDragFrameIndex + offsetFrame;
                if (offsetFrame == 0 || targetFrame < 0) return;
                
                onMainMouseMove?.Invoke(evt);
                
                var checkDrag = offsetFrame > 0
                    ? _clip.TrackBase.CheckFrameIndexOnDrag(targetFrame + DurationFrame)
                    : _clip.TrackBase.CheckFrameIndexOnDrag(targetFrame);

                if (checkDrag)
                {
                    _clip.UpdateClipDataStartFrame(targetFrame);
                    if (EndFrameIndex > AbilityTimelineEditorWindow.Instance.AbilityAsset.MaxFrameCount)
                        AbilityTimelineEditorWindow.Instance.CurrentSelectFrameIndex = EndFrameIndex;
                    _clip.RefreshShow(FrameUnitWidth);
                    AbilityTimelineEditorWindow.Instance.SetInspector(_clip);
                }
            }
        }
        
        protected void OnMainMouseOut(MouseOutEvent evt)
        {
            if (_dragging) OnMainAreaApplyDrag();
            _dragging = false;
        }
        
        protected void OnMainAreaApplyDrag()
        {
            if (StartFrameIndex != _startDragFrameIndex)
            {
                _clip.TrackBase.SetFrameIndex(_startDragFrameIndex, StartFrameIndex);
            }
        }

        #endregion
        
        #region Hover And Select

        protected static readonly Color NormalColor = new(0, 0.6f, 0.7f, 0.75f);
        protected static readonly Color SelectColor = new(0.6f, 0.1f, 0.1f, 0.75f);
        private const int TipBoundingSize = 4;
        public bool InClipRect(Vector2 position)
        {
            var rect = _mainArea.worldBound;
            return rect.Contains(position);
        }

        public void SwitchBounding()
        {
            bool isShow = Hovered || Selected;
            if(isShow)
            {
                var color =Selected?new Color(0.8f, 0.5f, 0.1f, 1f):new Color(0.5f, 0.6f, 0.7f, 0.9f);
                _outsideBox.style.backgroundColor = new StyleColor(color);
                _outsideBox.style.width = new StyleLength(_mainArea.worldBound.width+TipBoundingSize);
                _outsideBox.style.height = new StyleLength(_mainArea.worldBound.height+TipBoundingSize);
                _outsideBox.style.display = DisplayStyle.Flex;
                _outsideBox.MarkDirtyRepaint();
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
            Selected = true;
            AbilityTimelineEditorWindow.Instance.SetInspector(_clip);
            _itemLabel.style.backgroundColor = SelectColor;
            SwitchBounding();
        }

        public virtual void OnUnSelect()
        {
            Selected = false;
            _itemLabel.style.backgroundColor = NormalColor;
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
            Debug.Log($"_newResizeStartFrame={_newResizeStartFramePos}");
            if (offsetFrame == 0 || _newResizeStartFramePos < 0 || NewResizeStartFrame + 1 >= EndFrameIndex) return;

            var checkDrag = true; 
            foreach (var clipEvent in AbilityTimelineEditorWindow.Instance.AbilityAsset.AnimationData.animationClipData)
            {
                if (clipEvent != _clip.ClipData && clipEvent.startFrame<_lastResizeDragStartPos && 
                    NewResizeStartFrame < clipEvent.EndFrame && NewResizeStartFrame > clipEvent.startFrame)
                {
                    checkDrag = false;
                    break;
                }
            }
            if (checkDrag)
            {
                AbilityTimelineEditorWindow.Instance.DottedLineFrameIndex = NewResizeStartFrame;
            }
        }

        private void OnLeftResizeDragStart(PointerDownEvent evt)
        {
            _lastResizeDragStartPos = StartFrameIndex;
        }
        
        private void OnLeftResizeDragEnd()
        { 
            int lastEndFrame = EndFrameIndex;
            int minFrame = 0;
            foreach (var clipEvent in AbilityTimelineEditorWindow.Instance.AbilityAsset.AnimationData.animationClipData)
            {
                if (clipEvent != _clip.ClipData && clipEvent.EndFrame<_lastResizeDragStartPos)
                {
                    minFrame = Mathf.Max(minFrame,clipEvent.EndFrame);
                    break;
                }
            }

            var newStartFrame = Mathf.Clamp(NewResizeStartFrame, minFrame, lastEndFrame - 1);
            
            _clip.UpdateClipDataStartFrame(newStartFrame);
            _clip.UpdateClipDataDurationFrame(lastEndFrame - _clip.StartFrameIndex);
            if (EndFrameIndex > AbilityTimelineEditorWindow.Instance.AbilityAsset.MaxFrameCount)
                AbilityTimelineEditorWindow.Instance.CurrentSelectFrameIndex = EndFrameIndex;
            _clip.RefreshShow(FrameUnitWidth);
            AbilityTimelineEditorWindow.Instance.SetInspector(_clip);
            AbilityTimelineEditorWindow.Instance.DottedLineFrameIndex = -1;
        }
        
        void OnRightResizeDragMove  (Vector2 delta)
        {
            var offsetFrame = delta.x / FrameUnitWidth;
            _newResizeEndFramePos = _lastResizeDragEndPos + offsetFrame;
            if (offsetFrame == 0 || _newResizeEndFramePos < 0 || NewResizeEndFrame - 1 <= StartFrameIndex) return;

            var checkDrag = true; 
            foreach (var clipEvent in AbilityTimelineEditorWindow.Instance.AbilityAsset.AnimationData.animationClipData)
            {
                if (clipEvent != _clip.ClipData && clipEvent.startFrame>_lastResizeDragEndPos && 
                    NewResizeEndFrame > clipEvent.startFrame && NewResizeEndFrame < clipEvent.EndFrame)
                {
                    checkDrag = false;
                    break;
                }
            }
            if (checkDrag)
            {
                AbilityTimelineEditorWindow.Instance.DottedLineFrameIndex = NewResizeEndFrame;
            }
        }
        
        void OnRightResizeDragStart(PointerDownEvent evt)
        {
            _lastResizeDragEndPos = EndFrameIndex;
        }
        
        void OnRightResizeDragEnd()
        {
            int lastStartFrame = StartFrameIndex;
            int maxFrame = AbilityTimelineEditorWindow.Instance.AbilityAsset.MaxFrameCount;
            foreach (var clipEvent in AbilityTimelineEditorWindow.Instance.AbilityAsset.AnimationData.animationClipData)
            {
                if (clipEvent != _clip.ClipData && clipEvent.startFrame>_lastResizeDragEndPos)
                {
                    maxFrame = Mathf.Min(maxFrame,clipEvent.startFrame);
                    break;
                }
            }
            var newEndFrame = Mathf.Clamp(NewResizeEndFrame, lastStartFrame + 1, maxFrame);
            _clip.UpdateClipDataDurationFrame(newEndFrame - _clip.StartFrameIndex);
            if (EndFrameIndex > AbilityTimelineEditorWindow.Instance.AbilityAsset.MaxFrameCount)
                AbilityTimelineEditorWindow.Instance.CurrentSelectFrameIndex = EndFrameIndex;
            _clip.RefreshShow(FrameUnitWidth);
            AbilityTimelineEditorWindow.Instance.SetInspector(_clip);
            AbilityTimelineEditorWindow.Instance.DottedLineFrameIndex = -1;
        }
        #endregion
    }
}
#endif
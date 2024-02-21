using GAS.Runtime.Ability.AbilityTimeline;
using UnityEngine;
using UnityEngine.UIElements;

namespace GAS.Editor.Ability.AbilityTimelineEditor.Track.AnimationTrack
{
    public class AnimationTrackItem : TrackItemBase
    {
        private static readonly Color normalColor = new(0, 0.8f, 0.1f, 0.5f);
        private static readonly Color selectColor = new(0.7f, 0.1f, 0f, 0.5f);

        private AnimationFrameEvent _animationFrameEvent;
        private VisualElement _animOverLine;
        private float _frameUnitWidth;
        private VisualElement _mainDragArea;
        private int _startFrameIndex;

        private bool _dragging;
        private AnimationTrack _track;
        public Label ItemLabel { get; private set; }

        protected override string ItemAssetPath =>
            "Assets/GAS/Editor/Ability/AbilityTimelineEditor/Track/AnimationTrack/AnimationTrackItem.uxml";

        public void Init(AnimationTrack track, VisualElement parent, int frameIndex, float frameUnitWidth,
            AnimationFrameEvent animationFrameEvent)
        {
            base.Init();
            ItemLabel = Item as Label;
            _track = track;
            parent.Add(Item);
            _startFrameIndex = frameIndex;
            _frameUnitWidth = frameUnitWidth;
            _animationFrameEvent = animationFrameEvent;
            _mainDragArea = Item.Q<VisualElement>("Main");
            _animOverLine = Item.Q<VisualElement>("OverLine");

            _mainDragArea.RegisterCallback<MouseDownEvent>(OnMouseDown);
            _mainDragArea.RegisterCallback<MouseUpEvent>(OnMouseUp);
            _mainDragArea.RegisterCallback<MouseMoveEvent>(OnMouseMove);
            _mainDragArea.RegisterCallback<MouseOutEvent>(OnMouseOut);

            Item.style.backgroundColor = normalColor;
            
            RefreshShow(_frameUnitWidth);
        }


        public void RefreshShow(float newFrameUnitWidth)
        {
            _frameUnitWidth = newFrameUnitWidth;

            ItemLabel.text = _animationFrameEvent.Clip.name;
            var mainPos = ItemLabel.transform.position;
            mainPos.x = _startFrameIndex * _frameUnitWidth;
            ItemLabel.transform.position = mainPos;
            ItemLabel.style.width = _animationFrameEvent.DurationFrame * _frameUnitWidth;

            var clipFrameCount = (int)(_animationFrameEvent.Clip.length * _animationFrameEvent.Clip.frameRate);
            if (clipFrameCount > _animationFrameEvent.DurationFrame)
            {
                _animOverLine.style.display = DisplayStyle.None;
            }
            else
            {
                _animOverLine.style.display = DisplayStyle.Flex;

                var overLinePos = _animOverLine.transform.position;
                overLinePos.x = clipFrameCount * _frameUnitWidth - 1;
                _animOverLine.transform.position = overLinePos;
            }
        }

        #region Mouse Event

        private void OnMouseDown(MouseDownEvent evt)
        {
            Item.style.backgroundColor = selectColor;
            _dragging = true;
        }

        private void OnMouseUp(MouseUpEvent evt)
        {
            if (_dragging)
            {
                _dragging = false;
                Item.style.backgroundColor = normalColor;
            }
        }

        private void OnMouseMove(MouseMoveEvent evt)
        {
            if (_dragging)
            {
                // var delta = evt.mouseDelta.x;
                // var frameDelta = Mathf.RoundToInt(delta / _frameUnitWidth);
                // var newStartFrame = _startFrameIndex + frameDelta;
                // if (newStartFrame < 0)
                // {
                //     newStartFrame = 0;
                // }
                //
                // var newEndFrame = newStartFrame + _animationFrameEvent.DurationFrame;
                // if (newEndFrame > _track.TrackLength)
                // {
                //     newStartFrame = _track.TrackLength - _animationFrameEvent.DurationFrame;
                // }
                //
                // _startFrameIndex = newStartFrame;
                // var mainPos = ItemLabel.transform.position;
                // mainPos.x = _startFrameIndex * _frameUnitWidth;
                // ItemLabel.transform.position = mainPos;
            }
        }

        private void OnMouseOut(MouseOutEvent evt)
        {
            Item.style.backgroundColor = normalColor;
        }

        #endregion
    }
}
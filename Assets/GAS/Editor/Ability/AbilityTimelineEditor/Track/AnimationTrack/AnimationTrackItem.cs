using System.Collections.Generic;
using GAS.Runtime.Ability.AbilityTimeline;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace GAS.Editor.Ability.AbilityTimelineEditor.Track.AnimationTrack
{
    public class AnimationTrackItem : TrackItemBase
    {
        private static readonly Color normalColor = new(0, 0.8f, 0.1f, 0.5f);
        private static readonly Color selectColor = new(0.7f, 0.1f, 0f, 0.5f);

        private AnimationClipEvent _animationClipEvent;
        private VisualElement _animOverLine;

        private bool _dragging;
        private float _frameUnitWidth;
        private VisualElement _mainDragArea;
        private int _startDragFrameIndex;
        private float _startDragX;
        private int _startFrameIndex;


        private AnimationTrack _track;
        public Label ItemLabel { get; private set; }

        protected override string ItemAssetPath =>
            "Assets/GAS/Editor/Ability/AbilityTimelineEditor/Track/AnimationTrack/AnimationTrackItem.uxml";

        public void Init(
            AnimationTrack track,
            VisualElement parent,
            float frameUnitWidth,
            AnimationClipEvent animationClipEvent)
        {
            base.Init(animationClipEvent);
            ItemLabel = Item as Label;
            _track = track;
            parent.Add(Item);
            _startFrameIndex = animationClipEvent.startFrame;
            _frameUnitWidth = frameUnitWidth;
            _animationClipEvent = animationClipEvent;
            _mainDragArea = Item.Q<VisualElement>("Main");
            _animOverLine = Item.Q<VisualElement>("OverLine");

            _mainDragArea.RegisterCallback<MouseDownEvent>(OnMouseDown);
            _mainDragArea.RegisterCallback<MouseUpEvent>(OnMouseUp);
            _mainDragArea.RegisterCallback<MouseMoveEvent>(OnMouseMove);
            _mainDragArea.RegisterCallback<MouseOutEvent>(OnMouseOut);

            Item.style.backgroundColor = normalColor;

            RefreshShow(_frameUnitWidth);
        }

        #region Inspector

        private ObjectField clip;
        private Label startFrame;
        private Label duration;
        private FloatField transition;
        public override VisualElement Inspector()
        {
            var inspector = new VisualElement();
            // 动画Clip
            clip = new ObjectField("动画资源");
            clip.style.display = DisplayStyle.Flex;
            clip.objectType = typeof(AnimationClip);
            clip.value = _animationClipEvent.Clip;
            clip.RegisterValueChangedCallback(OnClipChanged);
            inspector.Add(clip);
            
            // 起始
            startFrame = new Label($"起始帧:{_startFrameIndex}f/{_startFrameIndex * Time.fixedDeltaTime}s");
            startFrame.style.display = DisplayStyle.Flex;
            inspector.Add(startFrame);
            
            // 时长
            duration = new Label($"时长:{_animationClipEvent.durationFrame}f/" +
                                       $"{_animationClipEvent.durationFrame * Time.fixedDeltaTime}s");
            duration.style.display = DisplayStyle.Flex;
            inspector.Add(duration);
            
            // 过渡
            transition = new FloatField("过渡时间");
            transition.style.display = DisplayStyle.Flex;
            transition.value = _animationClipEvent.TransitionTime;
            transition.RegisterValueChangedCallback(OnTransitionChanged);
            inspector.Add(transition);
            
            // 删除按钮
           Button delete = new Button(Delete);
           delete.text = "删除动画";
           delete.style.backgroundColor = new Color(0.8f, 0.1f, 0.1f, 0.5f);
           inspector.Add(delete);
           
           return inspector;
        }

        public override void Delete()
        {
            var keyFrameIndex = _animationClipEvent.startFrame;
            var success = AbilityTimelineEditorWindow.Instance.AbilityAsset.AnimationData.animationClipData.Remove(
                _animationClipEvent);

            if (!success) return;
            _track.RemoveTrackItem(this);
            AbilityTimelineEditorWindow.Instance.SetInspector();
        }

        private void OnTransitionChanged(ChangeEvent<float> evt)
        {
            var newTransition = Mathf.Max(0, evt.newValue);
            _animationClipEvent.TransitionTime = newTransition;
            AbilityTimelineEditorWindow.Instance.Save();
            transition.value = newTransition;
            //RefreshShow(_frameUnitWidth);
        }

        private void OnClipChanged(ChangeEvent<Object> evt)
        {
            var animClip = evt.newValue as AnimationClip;
            if (animClip != null)
            {
                _animationClipEvent.Clip = animClip;
                _animationClipEvent.durationFrame = (int)(animClip.length * animClip.frameRate);
                AbilityTimelineEditorWindow.Instance.Save();
                RefreshShow(_frameUnitWidth);
            }
            else
            {
                EditorUtility.DisplayDialog("ERROR", "动画资源不可为空！", "确定");
                RefreshShow(_frameUnitWidth);
            }
            
        }
        #endregion
        public void RefreshShow(float newFrameUnitWidth)
        {
            _frameUnitWidth = newFrameUnitWidth;

            ItemLabel.text = _animationClipEvent.Clip.name;
            var mainPos = ItemLabel.transform.position;
            mainPos.x = _startFrameIndex * _frameUnitWidth;
            ItemLabel.transform.position = mainPos;
            ItemLabel.style.width = _animationClipEvent.durationFrame * _frameUnitWidth;

            var clipFrameCount = (int)(_animationClipEvent.Clip.length * _animationClipEvent.Clip.frameRate);
            if (clipFrameCount > _animationClipEvent.durationFrame)
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

            // 刷新面板显示
            if (AbilityTimelineEditorWindow.Instance.CurrentInspectorObject == this)
            {
                AbilityTimelineEditorWindow.Instance.SetInspector(this);
            }
        }

        #region Mouse Event

        private void OnMouseDown(MouseDownEvent evt)
        {
            Item.style.backgroundColor = selectColor;
            _dragging = true;
            _startDragFrameIndex = _startFrameIndex;
            _startDragX = evt.mousePosition.x;
            
            // 更新小面板
            AbilityTimelineEditorWindow.Instance.SetInspector(this);
        }

        private void OnMouseUp(MouseUpEvent evt)
        {
            _dragging = false;
            Item.style.backgroundColor = normalColor;
            ApplyDrag();
        }

        private void OnMouseMove(MouseMoveEvent evt)
        {
            if (_dragging)
            {
                var offset = evt.mousePosition.x - _startDragX;
                var offsetFrame = Mathf.RoundToInt(offset / _frameUnitWidth);
                int targetFrame = _startDragFrameIndex + offsetFrame;
                if(offsetFrame==0 || targetFrame<0) return;
                
                var checkDrag = offsetFrame > 0 ? _track.CheckFrameIndexOnDrag(targetFrame + _animationClipEvent.durationFrame) : _track.CheckFrameIndexOnDrag(targetFrame);

                if (checkDrag)
                {
                    _startFrameIndex = targetFrame;
                    if (_startFrameIndex + _animationClipEvent.durationFrame >
                        AbilityTimelineEditorWindow.Instance.AbilityAsset.MaxFrameCount)
                    {
                        AbilityTimelineEditorWindow.Instance.CurrentSelectFrameIndex =
                            _startFrameIndex + _animationClipEvent.durationFrame;
                    }
                    RefreshShow(_frameUnitWidth);
                }
            }
        }

        private void OnMouseOut(MouseOutEvent evt)
        {
            if (_dragging) ApplyDrag();
            Item.style.backgroundColor = normalColor;
            _dragging = false;
        }

        void ApplyDrag()
        {
            if (_startFrameIndex != _startDragFrameIndex)
            {
                _track.SetFrameIndex( _startDragFrameIndex, _startFrameIndex);
            }
        }

        #endregion
    }
}
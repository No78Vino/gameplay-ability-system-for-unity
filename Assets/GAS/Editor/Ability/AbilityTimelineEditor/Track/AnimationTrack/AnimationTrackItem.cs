using GAS.Runtime.Ability.AbilityTimeline;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace GAS.Editor.Ability.AbilityTimelineEditor.Track.AnimationTrack
{
    public class AnimationTrackItem : TrackItem<AnimationTrack>
    {
        private AnimationClipEvent AnimationClipEvent => trackEvent as AnimationClipEvent;
        private VisualElement _animOverLine;
        private VisualElement _areaChangeSize;
        private VisualElement _mainDragArea;
        private int _startDragFrameIndex;
        private float _startDragX;
  
        private bool _dragging;
        public Label ItemLabel { get; private set; }
        protected override string ItemAssetGUID => "3197d239f4ce79b41b2278ecea5aaab8";

        public override void InitTrackItem(
            TrackBase track,
            VisualElement parent,
            float frameUnitWidth,
            TrackEventBase animationClipEvent)
        {
            base.InitTrackItem(track, parent, frameUnitWidth, animationClipEvent);
    
            ItemLabel = Item as Label;
            _mainDragArea = Item.Q<VisualElement>("Main");
            _animOverLine = Item.Q<VisualElement>("OverLine");
            _areaChangeSize = Item.Q<VisualElement>("AreaSizeChange");
            
            _mainDragArea.RegisterCallback<MouseDownEvent>(OnMouseDown);
            _mainDragArea.RegisterCallback<MouseUpEvent>(OnMouseUp);
            _mainDragArea.RegisterCallback<MouseMoveEvent>(OnMouseMove);
            _mainDragArea.RegisterCallback<MouseOutEvent>(OnMouseOut);
            
            _areaChangeSize.RegisterCallback<MouseDownEvent>(OnBtnSizeMouseDown);
            _areaChangeSize.RegisterCallback<MouseUpEvent>(OnBtnSizeMouseUp);
            _areaChangeSize.RegisterCallback<MouseMoveEvent>(OnBtnSizeMouseMove);
            
            _mainDragArea.generateVisualContent += OnDrawBoxGenerateVisualContent;
            RefreshShow(this.frameUnitWidth);
        }

        public override void RefreshShow(float newFrameUnitWidth)
        {
            base.RefreshShow(newFrameUnitWidth);
            ItemLabel.text = AnimationClipEvent.Clip.name;
            var mainPos = ItemLabel.transform.position;
            mainPos.x = startFrameIndex * frameUnitWidth;
            ItemLabel.transform.position = mainPos;
            ItemLabel.style.width = AnimationClipEvent.durationFrame * frameUnitWidth;

            var clipFrameCount = (int)(AnimationClipEvent.Clip.length * AnimationClipEvent.Clip.frameRate);
            if (clipFrameCount > AnimationClipEvent.durationFrame)
            {
                _animOverLine.style.display = DisplayStyle.None;
            }
            else
            {
                _animOverLine.style.display = DisplayStyle.Flex;

                var overLinePos = _animOverLine.transform.position;
                overLinePos.x = clipFrameCount * frameUnitWidth - 1;
                _animOverLine.transform.position = overLinePos;
            }

            // 刷新面板显示
            if (AbilityTimelineEditorWindow.Instance.CurrentInspectorObject == this)
                AbilityTimelineEditorWindow.Instance.SetInspector(this);
        }

        #region Inspector

        private ObjectField clip;
        private Label startFrame;
        private Label endFrame;
        private IntegerField duration;
        private FloatField transition;

        public override VisualElement Inspector()
        {
            var inspector = new VisualElement();
            // 动画Clip
            clip = new ObjectField("动画资源");
            clip.style.display = DisplayStyle.Flex;
            clip.objectType = typeof(AnimationClip);
            clip.value = AnimationClipEvent.Clip;
            clip.RegisterValueChangedCallback(OnClipChanged);
            inspector.Add(clip);

            // 起始
            startFrame = new Label($"起始帧:{startFrameIndex}f/{startFrameIndex * Time.fixedDeltaTime}s");
            startFrame.style.display = DisplayStyle.Flex;
            inspector.Add(startFrame);
            // 结束
            endFrame = new Label(
                $"结束帧:{AnimationClipEvent.EndFrame}f/{AnimationClipEvent.EndFrame * Time.fixedDeltaTime}s");
            endFrame.style.display = DisplayStyle.Flex;
            inspector.Add(endFrame);

            // 时长
            duration = new IntegerField("时长(f)");
            duration.value = AnimationClipEvent.durationFrame;
            duration.isDelayed = true;
            duration.RegisterValueChangedCallback(OnDurationChanged);
            duration.style.display = DisplayStyle.Flex;
            inspector.Add(duration);

            // 过渡
            transition = new FloatField("过渡时间");
            transition.style.display = DisplayStyle.Flex;
            transition.value = AnimationClipEvent.TransitionTime;
            transition.isDelayed = true;
            transition.RegisterValueChangedCallback(OnTransitionChanged);
            inspector.Add(transition);

            // 删除按钮
            var delete = new Button(Delete);
            delete.text = "删除动画";
            delete.style.backgroundColor = new Color(0.8f, 0.1f, 0.1f, 0.5f);
            inspector.Add(delete);

            return inspector;
        }

        private void OnDurationChanged(ChangeEvent<int> evt)
        {
            var max = AbilityTimelineEditorWindow.Instance.AbilityAsset.MaxFrameCount - startFrameIndex;
            foreach (var clipEvent in track.AbilityAnimationData.animationClipData)
            {
                if (startFrameIndex >= clipEvent.startFrame) continue;
                var length = Mathf.Max(1,clipEvent.startFrame - startFrameIndex);
                max = Mathf.Min(max, length);
            }
            var newDuration = Mathf.Clamp(evt.newValue,1,max);
            AnimationClipEvent.durationFrame = newDuration;
            AbilityTimelineEditorWindow.Instance.Save();
            duration.value = newDuration;
            RefreshShow(frameUnitWidth);
        }

        public override void Delete()
        {
            var success = AbilityTimelineEditorWindow.Instance.AbilityAsset.AnimationData.animationClipData.Remove(
                AnimationClipEvent);
            AbilityTimelineEditorWindow.Instance.Save();
            if (!success) return;
            track.RemoveTrackItem(this);
            AbilityTimelineEditorWindow.Instance.SetInspector();
        }

        private void OnTransitionChanged(ChangeEvent<float> evt)
        {
            var newTransition = Mathf.Max(0, evt.newValue);
            AnimationClipEvent.TransitionTime = newTransition;
            AbilityTimelineEditorWindow.Instance.Save();
            transition.value = newTransition;
            //RefreshShow(_frameUnitWidth);
        }

        private void OnClipChanged(ChangeEvent<Object> evt)
        {
            var animClip = evt.newValue as AnimationClip;
            if (animClip != null)
            {
                AnimationClipEvent.Clip = animClip;
                AnimationClipEvent.durationFrame = (int)(animClip.length * animClip.frameRate);
                AbilityTimelineEditorWindow.Instance.Save();
                RefreshShow(frameUnitWidth);
            }
            else
            {
                EditorUtility.DisplayDialog("ERROR", "动画资源不可为空！", "确定");
                RefreshShow(frameUnitWidth);
            }
        }

        #endregion

        #region Mouse Event

        private void OnMouseDown(MouseDownEvent evt)
        {
            _dragging = true;
            _startDragFrameIndex = startFrameIndex;
            _startDragX = evt.mousePosition.x;
            Select();
        }

        private void OnMouseUp(MouseUpEvent evt)
        {
            _dragging = false;
            ApplyDrag();
        }

        private void OnMouseMove(MouseMoveEvent evt)
        {
            if (_dragging)
            {
                var offset = evt.mousePosition.x - _startDragX;
                var offsetFrame = Mathf.RoundToInt(offset / frameUnitWidth);
                var targetFrame = _startDragFrameIndex + offsetFrame;
                if (offsetFrame == 0 || targetFrame < 0) return;

                var checkDrag = offsetFrame > 0
                    ? track.CheckFrameIndexOnDrag(targetFrame + AnimationClipEvent.durationFrame)
                    : track.CheckFrameIndexOnDrag(targetFrame);

                if (checkDrag)
                {
                    startFrameIndex = targetFrame;
                    if (startFrameIndex + AnimationClipEvent.durationFrame >
                        AbilityTimelineEditorWindow.Instance.AbilityAsset.MaxFrameCount)
                        AbilityTimelineEditorWindow.Instance.CurrentSelectFrameIndex =
                            startFrameIndex + AnimationClipEvent.durationFrame;
                    RefreshShow(frameUnitWidth);
                    AbilityTimelineEditorWindow.Instance.SetInspector(this);
                }
            }
        }

        private void OnMouseOut(MouseOutEvent evt)
        {
            if (_dragging) ApplyDrag();
            _dragging = false;
        }

        private void ApplyDrag()
        {
            if (startFrameIndex != _startDragFrameIndex) track.SetFrameIndex(_startDragFrameIndex, startFrameIndex);
        }

        #endregion

        #region SizeLine Mouse Event
        
        bool _sizeDragging;
        private float _lastSizeMousePosX;
        private void OnBtnSizeMouseDown(MouseDownEvent evt)
        {
            _sizeDragging = true;
            _lastSizeMousePosX = evt.mousePosition.x;
            Debug.Log($"OnBtnSizeMouseDown");
        }
        
        private void OnBtnSizeMouseUp(MouseUpEvent evt)
        {
            _sizeDragging = false;
            // float offset = evt.mousePosition.x - _lastSizeMousePosX;
            // int offsetFrame = Mathf.RoundToInt(offset / frameUnitWidth);
            // int durationFrame = AnimationClipEvent.durationFrame + offsetFrame;
            // ApplySizeDrag(durationFrame);
            Debug.Log($"OnBtnSizeMouseUp");
        }

        private void OnBtnSizeMouseMove(MouseMoveEvent evt)
        {
            if (_sizeDragging)
            {
                float offset = evt.mousePosition.x - _lastSizeMousePosX;
                int offsetFrame = Mathf.RoundToInt(offset / frameUnitWidth);
                int durationFrame = AnimationClipEvent.durationFrame + offsetFrame;
                if (offsetFrame == 0 || durationFrame < 1) return;
                Debug.Log($"DurationFrame:{durationFrame}");
                //ApplySizeDrag(durationFrame);
            }
        }
        
        private void ApplySizeDrag(int durationFrame)
        {            
            var max = AbilityTimelineEditorWindow.Instance.AbilityAsset.MaxFrameCount - startFrameIndex;
            foreach (var clipEvent in track.AbilityAnimationData.animationClipData)
            {
                if (startFrameIndex >= clipEvent.startFrame) continue;
                var length = Mathf.Max(1,clipEvent.startFrame - startFrameIndex);
                max = Mathf.Min(max, length);
            }
            
            durationFrame = Mathf.Clamp(durationFrame, 1, max);
            AnimationClipEvent.durationFrame = durationFrame;
            AbilityTimelineEditorWindow.Instance.Save();
            RefreshShow(frameUnitWidth);
            AbilityTimelineEditorWindow.Instance.SetInspector(this);
        }
        #endregion
        
        public override void OnUnSelect()
        {
            base.OnUnSelect();
            if(_areaChangeSize!=null) _areaChangeSize.style.display = DisplayStyle.None;
        }
        
        public override void OnSelect()
        {
            base.OnUnSelect();
            if(_areaChangeSize!=null) _areaChangeSize.style.display = DisplayStyle.Flex;
        }
        
        
        void OnDrawBoxGenerateVisualContent(MeshGenerationContext mgc)
        {
            bool Hovered = true;
            if (Hovered)
            {
                var paint2D = mgc.painter2D;
                paint2D.strokeColor = new Color(68, 192, 255, 255);
                paint2D.BeginPath();
                paint2D.MoveTo(new Vector2(0, 0));
                paint2D.LineTo(new Vector2(_mainDragArea.worldBound.width, 0));
                paint2D.LineTo(new Vector2(_mainDragArea.worldBound.width, _mainDragArea.worldBound.height));
                paint2D.LineTo(new Vector2(0, _mainDragArea.worldBound.height));
                paint2D.LineTo(new Vector2(0, 0));
                paint2D.Stroke();
            }
        }
    }
}
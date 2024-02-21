using GAS.Runtime.Ability.AbilityTimeline;
using UnityEngine.UIElements;

namespace GAS.Editor.Ability.AbilityTimelineEditor.Track.AnimationTrack
{
    public class AnimationTrackItem : TrackItemBase
    {
        private AnimationFrameEvent _animationFrameEvent;
        private int _startFrameIndex;
        private float _frameUnitWidth;

        private AnimationTrack _track;
        public Label ItemLabel { get;private set;  }

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
        }
    }
}
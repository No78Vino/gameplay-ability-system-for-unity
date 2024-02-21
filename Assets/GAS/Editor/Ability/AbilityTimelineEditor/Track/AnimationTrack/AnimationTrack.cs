using System.Collections.Generic;
using GAS.Runtime.Ability;
using GAS.Runtime.Ability.AbilityTimeline;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

#if UNITY_EDITOR
namespace GAS.Editor.Ability.AbilityTimelineEditor.Track.AnimationTrack
{
    public class AnimationTrack : TrackBase
    {
        public override void Init(VisualElement trackParent, VisualElement menuParent, float frameWidth)
        {
            base.Init(trackParent, menuParent, frameWidth);
            Track.RegisterCallback<DragUpdatedEvent>(OnDragUpdated);
            Track.RegisterCallback<DragExitedEvent>(OnDragExited);
            RefreshShow();
        }

        protected override string TrackAssetPath =>
            "Assets/GAS/Editor/Ability/AbilityTimelineEditor/Track/AnimationTrack/AnimationTrack.uxml";

        protected override string MenuAssetPath =>
            "Assets/GAS/Editor/Ability/AbilityTimelineEditor/Track/AnimationTrack/AnimationTrackMenu.uxml";

        private Dictionary<int, AnimationTrackItem> _trackItems = new Dictionary<int, AnimationTrackItem>();

        private AbilityAnimationData AbilityAnimationData =>
            AbilityTimelineEditorWindow.Instance.AbilityAsset.AnimationData;
        protected override void RefreshShow(float newFrameWidth)
        {
            base.RefreshShow(newFrameWidth);
            foreach (var kv in _trackItems)
            {
                Track.Remove(kv.Value.ItemLabel);
            }
            _trackItems.Clear();

            if (AbilityTimelineEditorWindow.Instance.AbilityAsset != null)
            {
                foreach (var frameEvent in AbilityAnimationData.frameData)
                {
                    var animationFrameEvent = frameEvent.Event;
                    var item = new AnimationTrackItem();
                    item.Init(this, Track, frameEvent.Frame, FrameWidth, animationFrameEvent);
                    _trackItems.Add(frameEvent.Frame, item);
                }
            }
        }

        #region DragEvent

        private void OnDragUpdated(DragUpdatedEvent evt)
        {
            var objects = DragAndDrop.objectReferences;
            if (objects.Length > 0)
            {
                var clip = objects[0] as AnimationClip;
                DragAndDrop.visualMode = clip != null ? DragAndDropVisualMode.Copy : DragAndDropVisualMode.Rejected;
            }
            else
            {
                DragAndDrop.visualMode = DragAndDropVisualMode.Rejected;
            }
        }

        private void OnDragExited(DragExitedEvent evt)
        {
            var objects = DragAndDrop.objectReferences;
            if (objects.Length > 0)
            {
                var clip = objects[0] as AnimationClip;
                if (clip != null)
                {
                    var selectedFrame =
                        AbilityTimelineEditorWindow.Instance.GetFrameIndexByPosition(evt.localMousePosition.x);
                    // TODO:判断是否在无效帧上（已存在AnimationClip时不可添加）
                    var valid = true;
                    var durationFrame = -1;
                    var nextTrackItemFrame = -1;
                    var currentOffset = int.MaxValue;
                    var clipFrameCount = (int)(clip.frameRate * clip.length);

                    foreach (var animFrameEvent in AbilityAnimationData.frameData)
                    {
                        if (selectedFrame > animFrameEvent.Frame && selectedFrame < animFrameEvent.Frame + (animFrameEvent.Event).DurationFrame)
                        {
                            valid = false;
                            break;
                        }

                        if (animFrameEvent.Frame > selectedFrame)
                        {
                            var tempOffset = animFrameEvent.Frame - selectedFrame;
                            if (tempOffset < currentOffset)
                            {
                                currentOffset = tempOffset;
                                nextTrackItemFrame = animFrameEvent.Frame;
                            }
                        }
                    }

                    if (valid)
                    {
                        if (nextTrackItemFrame >= 0)
                        {
                            var offset = clipFrameCount - currentOffset;
                            durationFrame = offset < 0 ? clipFrameCount : currentOffset;
                        }
                        else
                        {
                            durationFrame = clipFrameCount;
                        }

                        var animationFrameEvent = new AnimationFrameEvent
                        {
                            Clip = clip,
                            DurationFrame = durationFrame,
                            TransitionTime = 0.25f
                        };

                        AbilityAnimationData.frameData.Add(new AnimationFrameEventInfo
                        {
                            Frame = selectedFrame,
                            Event = animationFrameEvent
                        });
                        AbilityTimelineEditorWindow.Instance.AbilityAsset.Save();
                        RefreshShow();
                        Debug.Log($"Add Animation Clip {clip.name} to Frame {selectedFrame}");
                    }
                }
            }
        }

        #endregion
        
        public bool CheckFrameIndexOnDrag(int targetIndex)
        {
            foreach (var item in AbilityAnimationData.frameData)
            {
                if(targetIndex>item.Frame && targetIndex<item.Frame+item.Event.DurationFrame)
                    return false;
            }

            return true;
        }
        
        public void SetFrameIndex(int oldIndex, int newIndex)
        {
            var index = -1;
            var list = AbilityAnimationData.frameData;
            for (var i = 0; i < list.Count; i++)
            {
                if (list[i].Frame != oldIndex) continue;
                index = i;
                break;
            }

            if (index < 0) return;
            AbilityAnimationData.frameData[index].Frame= newIndex;
            AbilityTimelineEditorWindow.Instance.AbilityAsset.Save();
        }
    }
}
#endif
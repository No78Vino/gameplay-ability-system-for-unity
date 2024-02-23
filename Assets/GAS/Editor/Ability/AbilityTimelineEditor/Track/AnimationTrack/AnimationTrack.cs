using System.Collections.Generic;
using System.Linq;
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

        private List<AnimationTrackItem> _trackItems = new List<AnimationTrackItem>();

        public AbilityAnimationData AbilityAnimationData =>
            AbilityTimelineEditorWindow.Instance.AbilityAsset.AnimationData;
        public override void RefreshShow(float newFrameWidth)
        {
            base.RefreshShow(newFrameWidth);
            foreach (var item in _trackItems)
            {
                Track.Remove(item.ItemLabel);
            }
            _trackItems.Clear();

            if (AbilityTimelineEditorWindow.Instance.AbilityAsset != null)
            {
                foreach (var clipEvent in AbilityAnimationData.animationClipData)
                {
                    var item = new AnimationTrackItem();
                    item.InitTrackItem(this, Track, FrameWidth, clipEvent);
                    _trackItems.Add(item);
                }
            }
        }

        public void RemoveTrackItem(AnimationTrackItem item)
        {
            Track.Remove(item.ItemLabel);
            _trackItems.Remove(item);
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
                    var valid = true;
                    var durationFrame = -1;
                    var nextTrackItemFrame = -1;
                    var currentOffset = int.MaxValue;
                    var clipFrameCount = (int)(clip.frameRate * clip.length);

                    foreach (var clipEvent in AbilityAnimationData.animationClipData)
                    {
                        if (selectedFrame > clipEvent.startFrame && selectedFrame < clipEvent.EndFrame)
                        {
                            valid = false;
                            break;
                        }

                        if (clipEvent.startFrame > selectedFrame)
                        {
                            var tempOffset = clipEvent.startFrame - selectedFrame;
                            if (tempOffset < currentOffset)
                            {
                                currentOffset = tempOffset;
                                nextTrackItemFrame = clipEvent.startFrame;
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

                        AbilityAnimationData.animationClipData.Add(new AnimationClipEvent()
                        {
                            startFrame = selectedFrame,
                            durationFrame = durationFrame,
                            Clip = clip,
                            TransitionTime = 0
                        });
                        AbilityTimelineEditorWindow.Instance.AbilityAsset.Save();
                        RefreshShow();
                        
                        // 面板选中新增的Item
                        AbilityTimelineEditorWindow.Instance.SetInspector(_trackItems.Last());
                        
                        Debug.Log($"Add Animation Clip {clip.name} to Frame {selectedFrame}");
                    }
                }
            }
        }

        #endregion
        
        public bool CheckFrameIndexOnDrag(int targetIndex)
        {
            return AbilityAnimationData.animationClipData.All(clipEvent => targetIndex <= clipEvent.startFrame || targetIndex >= clipEvent.EndFrame);
        }
        
        public void SetFrameIndex(int oldIndex, int newIndex)
        {
            var index = -1;
            var list = AbilityAnimationData.animationClipData;
            for (var i = 0; i < list.Count; i++)
            {
                if (list[i].startFrame != oldIndex) continue;
                index = i;
                break;
            }

            if (index < 0) return;
            AbilityAnimationData.animationClipData[index].startFrame= newIndex;
            AbilityTimelineEditorWindow.Instance.AbilityAsset.Save();
        }
        
        public override void TickView(int frameIndex, params object[] param )
        {
            GameObject previewObject = param[0] as GameObject;
            foreach (var clipEvent in AbilityAnimationData.animationClipData)
            {
                if (clipEvent.startFrame <= frameIndex && frameIndex < clipEvent.EndFrame)
                {
                    float clipFrameCount = (int)(clipEvent.Clip.frameRate * clipEvent.Clip.length);
                    float progress = (frameIndex - clipEvent.startFrame) / clipFrameCount;
                    if (progress > 1 && clipEvent.Clip.isLooping) progress -= (int)progress;
                    clipEvent.Clip.SampleAnimation(previewObject, progress * clipEvent.Clip.length);
                }
            }
        }
    }
}
#endif
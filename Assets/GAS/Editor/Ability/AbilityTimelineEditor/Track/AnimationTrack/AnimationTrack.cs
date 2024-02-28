using System;
using System.Linq;
using GAS.Runtime.Ability.AbilityTimeline;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

#if UNITY_EDITOR
namespace GAS.Editor.Ability.AbilityTimelineEditor.Track.AnimationTrack
{
    public class AnimationTrack : TrackBase
    {
        public AbilityAnimationData AbilityAnimationData => null;
            //AbilityTimelineEditorWindow.Instance.AbilityAsset.AnimationData;

        public override VisualElement Inspector()
        {
            var inspector = new VisualElement();
            return inspector;
        }

        public override void Init(VisualElement trackParent, VisualElement menuParent, float frameWidth,TrackDataBase trackData)
        {
            base.Init(trackParent, menuParent, frameWidth,trackData);
            Track.RegisterCallback<DragUpdatedEvent>(OnDragUpdated);
            Track.RegisterCallback<DragExitedEvent>(OnDragExited);
            RefreshShow();
        }

        public override void RefreshShow(float newFrameWidth)
        {
            base.RefreshShow(newFrameWidth);
            foreach (var item in _trackItems) Track.Remove(((TrackClipBase)item).ClipVe);
            _trackItems.Clear();

            if (AbilityTimelineEditorWindow.Instance.AbilityAsset != null)
                foreach (var clipEvent in AbilityAnimationData.animationClipData)
                {
                    var item = new AnimationTrackClip();
                    item.InitTrackClip(this, Track, _frameWidth, clipEvent);
                    _trackItems.Add(item);
                }
        }

        protected override void OnAddTrackItem(DropdownMenuAction action)
        {
            Debug.Log("Add Animation Clip");
        }

        protected override void OnRemoveTrack(DropdownMenuAction action)
        {
                // 删除数据
                Debug.Log($"[EX]Menu Item Clicked: {action.name}");
        }

        public override Type TrackDataType => typeof(AbilityAnimationData);
        protected override Color TrackColor => new Color(0.5f, 0.5f, 0.9f, 0.5f);
        protected override Color MenuColor => new Color(0.5f, 0.5f, 0.9f, 1);

        public override void TickView(int frameIndex, params object[] param)
        {
            var previewObject = param[0] as GameObject;
            foreach (var clipEvent in AbilityAnimationData.animationClipData)
                if (clipEvent.startFrame <= frameIndex && frameIndex < clipEvent.EndFrame)
                {
                    float clipFrameCount = (int)(clipEvent.Clip.frameRate * clipEvent.Clip.length);
                    var progress = (frameIndex - clipEvent.startFrame) / clipFrameCount;
                    if (progress > 1 && clipEvent.Clip.isLooping) progress -= (int)progress;
                    clipEvent.Clip.SampleAnimation(previewObject, progress * clipEvent.Clip.length);
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
                    var valid = true;
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
                        var durationFrame = -1;
                        if (nextTrackItemFrame >= 0)
                        {
                            var offset = clipFrameCount - currentOffset;
                            durationFrame = offset < 0 ? clipFrameCount : currentOffset;
                        }
                        else
                        {
                            durationFrame = clipFrameCount;
                        }

                        AbilityAnimationData.animationClipData.Add(new AnimationClipEvent
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
    }
}
#endif
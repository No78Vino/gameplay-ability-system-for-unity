using GAS.Runtime.Ability;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

#if UNITY_EDITOR
namespace GAS.Editor.Ability.AbilityTimelineEditor.Track.AnimationTrack
{
    public class AnimationTrack:TrackBase
    {
        protected override string TrackAssetPath => "Assets/GAS/Editor/Ability/AbilityTimelineEditor/Track/AnimationTrack/AnimationTrack.uxml";
        protected override string MenuAssetPath => "Assets/GAS/Editor/Ability/AbilityTimelineEditor/Track/AnimationTrack/AnimationTrackMenu.uxml";

        public AnimationTrack(VisualElement trackParent, VisualElement menuParent) : base(trackParent, menuParent)
        {
            Track.RegisterCallback<DragUpdatedEvent>(OnDragUpdated);
            Track.RegisterCallback<DragExitedEvent >(OnDragExited);
        }

        private void OnDragUpdated(DragUpdatedEvent evt)
        {
            Object[] objects = DragAndDrop.objectReferences;
            if (objects.Length > 0)
            {
                AnimationClip clip = objects[0] as AnimationClip;
                DragAndDrop.visualMode = clip != null ? DragAndDropVisualMode.Copy : DragAndDropVisualMode.Rejected;
            }
            else
            {
                DragAndDrop.visualMode = DragAndDropVisualMode.Rejected;
            }
        }
        
        private void OnDragExited(DragExitedEvent evt)
        {
            Object[] objects = DragAndDrop.objectReferences;
            if (objects.Length > 0)
            {
                AnimationClip clip = objects[0] as AnimationClip;
                if (clip != null)
                {
                    int selectedFrame = AbilityTimelineEditorWindow.Instance.GetFrameIndexByPosition(evt.localMousePosition.x);
                    // TODO:判断是否在无效帧上（已存在AnimationClip时不可添加）
                    bool valid = true;
                    int durationFrame = -1;
                    int nextTrackItemFrame = -1;
                    int currentOffset = int.MaxValue;
                    int clipFrameCount = (int)(clip.frameRate * clip.length);
                    
                    foreach (var kv in AbilityTimelineEditorWindow.Instance.AbilityAsset.AnimationData.FrameData)
                    {
                        if (selectedFrame > kv.Key && selectedFrame < kv.Key + kv.Value.DurationFrame)
                        {
                            valid = false;
                            break;
                        }

                        if (kv.Key > selectedFrame)
                        {
                            int tempOffset = kv.Key - selectedFrame;
                            if(tempOffset < currentOffset)
                            {
                                currentOffset = tempOffset;
                                nextTrackItemFrame = kv.Key;
                            }
                        }
                    }
                    
                    if (valid)
                    {
                        if (nextTrackItemFrame >= 0)
                        {
                            int offset = clipFrameCount - currentOffset;
                            durationFrame = offset<0 ? clipFrameCount : currentOffset;
                        }
                        else
                        {
                            durationFrame = clipFrameCount;
                        }
                        
                        AnimationFrameEvent animationFrameEvent = new AnimationFrameEvent
                        {
                            Clip = clip,
                            DurationFrame = durationFrame,
                            TransitionTime = 0.25f
                        };
                        
                        AbilityTimelineEditorWindow.Instance.AbilityAsset.AnimationData.FrameData.Add(selectedFrame, animationFrameEvent);
                        AbilityTimelineEditorWindow.Instance.AbilityAsset.Save();
                        Debug.Log( $"Add Animation Clip {clip.name} to Frame {selectedFrame}");
                    }
                }
            }
        }
    }
}
#endif
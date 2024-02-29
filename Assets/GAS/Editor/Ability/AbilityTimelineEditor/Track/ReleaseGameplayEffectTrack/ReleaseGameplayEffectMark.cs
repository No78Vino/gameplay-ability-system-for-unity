using GAS.Editor.Ability.AbilityTimelineEditor.Track;
using GAS.Runtime.Ability.AbilityTimeline;
using GAS.Runtime.Cue;
using GAS.Runtime.Effects;
using UnityEngine;
using UnityEngine.UIElements;

namespace GAS.Editor.Ability.AbilityTimelineEditor
{
    public class ReleaseGameplayEffectMark:TrackMark<ReleaseGameplayEffectTrack>
    {
        private ReleaseGameplayEffectMarkEvent MarkData => markData as ReleaseGameplayEffectMarkEvent;

        private ReleaseGameplayEffectMarkEvent MarkDataForSave
        {
            get
            {
                var trackDataForSave = ReleaseGameplayEffectTrack.ReleaseGameplayEffectTrackData;
                for (var i = 0; i < trackDataForSave.markEvents.Count; i++)
                    if (trackDataForSave.markEvents[i] == MarkData)
                        return ReleaseGameplayEffectTrack.ReleaseGameplayEffectTrackData.markEvents[i];
                return null;
            }
        }

        public override void RefreshShow(float newFrameUnitWidth)
        {
            base.RefreshShow(newFrameUnitWidth);
            ItemLabel.text = MarkData.gameplayEffectAssets.Count.ToString();
        }

        public override VisualElement Inspector()
        {
            var inspector = TrackInspectorUtil.CreateTrackInspector();
            var markFrame = TrackInspectorUtil.CreateLabel($"触发帧:{markData.startFrame}");
            inspector.Add(markFrame);

            // TODO 释放单位方法选取
            
            
            var list = TrackInspectorUtil.CreateObjectListView("GameplayEffect列表", MarkData.gameplayEffectAssets, OnGameplayEffectAssetChanged);
            inspector.Add(list);
            
            return inspector;
        }

        private void OnGameplayEffectAssetChanged(int index, ChangeEvent<Object> evt)
        {
            var gameplayEffectAsset = evt.newValue as GameplayEffectAsset;
            MarkDataForSave.gameplayEffectAssets[index] = gameplayEffectAsset;
            AbilityTimelineEditorWindow.Instance.Save();
            
            RefreshShow(FrameUnitWidth);
        }

        public override void Delete()
        {
            var success = ReleaseGameplayEffectTrack.ReleaseGameplayEffectTrackData.markEvents.Remove(MarkData);
            AbilityTimelineEditorWindow.Instance.Save();
            if (!success) return;
            track.RemoveTrackItem(this);
            AbilityTimelineEditorWindow.Instance.SetInspector();
        }

        public override void UpdateMarkDataFrame(int newStartFrame)
        {
            var updatedClip = MarkDataForSave;
            MarkDataForSave.startFrame = newStartFrame;
            AbilityTimelineEditorWindow.Instance.Save();
            markData = updatedClip;
        }
    }
}

#if UNITY_EDITOR
namespace GAS.Editor
{
    using Runtime;
    using UnityEngine;
    using UnityEngine.UIElements;
    
    public class BuffGameplayEffectClip : TrackClip<BuffGameplayEffectTrack>
    {
        public BuffGameplayEffectClipEvent BuffGameplayEffectClipData => clipData as BuffGameplayEffectClipEvent;

        private BuffGameplayEffectClipEvent ClipDataForSave
        {
            get
            {
                var cueTrackDataForSave = track.BuffTrackDataForSave;
                for (var i = 0; i < cueTrackDataForSave.clipEvents.Count; i++)
                    if (cueTrackDataForSave.clipEvents[i] == BuffGameplayEffectClipData)
                        return track.BuffTrackDataForSave.clipEvents[i];
                return null;
            }
        }

        public override void Delete()
        {
            var success = track.BuffTrackDataForSave.clipEvents.Remove(BuffGameplayEffectClipData);
            AbilityTimelineEditorWindow.Instance.Save();
            if (!success) return;
            track.RemoveTrackItem(this);
            AbilityTimelineEditorWindow.Instance.SetInspector();
        }

        public override void RefreshShow(float newFrameUnitWidth)
        {
            base.RefreshShow(newFrameUnitWidth);
            // clip 文本
            ItemLabel.text = BuffGameplayEffectClipData.gameplayEffect
                ? BuffGameplayEffectClipData.gameplayEffect.name
                : "【NULL】";

            // 刷新面板显示
            if (AbilityTimelineEditorWindow.Instance.CurrentInspectorObject == this)
                AbilityTimelineEditorWindow.Instance.SetInspector(this);
        }

        public override void UpdateClipDataStartFrame(int newStartFrame)
        {
            var updatedClip = ClipDataForSave;
            ClipDataForSave.startFrame = newStartFrame;
            AbilityTimelineEditorWindow.Instance.Save();
            clipData = updatedClip;
        }

        public override void UpdateClipDataDurationFrame(int newDurationFrame)
        {
            var updatedClip = ClipDataForSave;
            ClipDataForSave.durationFrame = newDurationFrame;
            AbilityTimelineEditorWindow.Instance.Save();
            clipData = updatedClip;
        }

        public void UpdateClipDataBuff(GameplayEffectAsset newBuff)
        {
            var updatedClip = ClipDataForSave;
            ClipDataForSave.gameplayEffect = newBuff;
            AbilityTimelineEditorWindow.Instance.Save();
            clipData = updatedClip;
        }
        
        public override void OnTickView(int frameIndex, int startFrame, int endFrame)
        {
        }

        public override Object DataInspector=> BuffGameplayEffectClipEditor.Create(this);
    }
}
#endif
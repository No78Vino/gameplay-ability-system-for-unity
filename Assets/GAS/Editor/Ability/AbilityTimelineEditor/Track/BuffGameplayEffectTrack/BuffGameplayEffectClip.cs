using GAS.Runtime.Ability;
using GAS.Runtime.Ability.AbilityTimeline;
using GAS.Runtime.Cue;
using GAS.Runtime.Effects;
using UnityEngine;
using UnityEngine.UIElements;

namespace GAS.Editor.Ability.AbilityTimelineEditor.Track
{
    public class BuffGameplayEffectClip: TrackClip<BuffGameplayEffectTrack>
    {
        private TimelineAbilityAsset AbilityAsset => AbilityTimelineEditorWindow.Instance.AbilityAsset;
        private BuffGameplayEffectClipEvent BuffGameplayEffectClipData => clipData as BuffGameplayEffectClipEvent;

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
            ItemLabel.text = BuffGameplayEffectClipData.gameplayEffect ? BuffGameplayEffectClipData.gameplayEffect.name : "【NULL】";

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

        #region Inspector

        private Label _startFrameLabel;
        private Label _endFrameLabel;
        private IntegerField _durationField;

        public override VisualElement Inspector()
        {
            var inspector = TrackInspectorUtil.CreateTrackInspector();

            // 运行帧
            _startFrameLabel =
                TrackInspectorUtil.CreateLabel(
                    $"运行(f):{BuffGameplayEffectClipData.startFrame}/{BuffGameplayEffectClipData.EndFrame}");
            inspector.Add(_startFrameLabel);

            // 持续帧
            _durationField = TrackInspectorUtil.CreateIntegerField("时长(f)", BuffGameplayEffectClipData.durationFrame,
                OnDurationFrameChanged);
            inspector.Add(_durationField);

            // GameplayEffect Asset
            var buff = TrackInspectorUtil.CreateObjectField("Buff GameplayEffect", typeof(GameplayEffectAsset),
                BuffGameplayEffectClipData.gameplayEffect,
                evt =>
                {
                    // 修改数据
                    ClipDataForSave.gameplayEffect = evt.newValue as GameplayEffectAsset;
                    AbilityAsset.Save();
                    clipData = ClipDataForSave;
                    // 修改显示
                    RefreshShow(FrameUnitWidth);
                });
            inspector.Add(buff);
            
            // 删除按钮
            var deleteButton = TrackInspectorUtil.CreateButton("删除", Delete);
            deleteButton.style.backgroundColor = new StyleColor(new Color(0.5f, 0, 0, 1f));
            inspector.Add(deleteButton);

            return inspector;
        }

        private void OnDurationFrameChanged(ChangeEvent<int> evt)
        {
            // 钳制
            var max = AbilityAsset.MaxFrameCount - BuffGameplayEffectClipData.startFrame;
            var newValue = Mathf.Clamp(evt.newValue, 1, max);
            // 保存数据
            UpdateClipDataDurationFrame(newValue);
            // 修改显示
            RefreshShow(FrameUnitWidth);
            _endFrameLabel.text = $"结束帧:{BuffGameplayEffectClipData.EndFrame}";
            _durationField.value = newValue;
        }

        #endregion
    }
}
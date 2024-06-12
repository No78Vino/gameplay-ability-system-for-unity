#if UNITY_EDITOR
namespace GAS.Editor
{
    using System;
    using UnityEngine;
    using UnityEngine.UIElements;
    using GAS.Runtime;

    public class BuffGameplayEffectTrack : TrackBase
    {
        private BuffGameplayEffectTrackData _buffGameplayEffectTrackData;
        public override Type TrackDataType => typeof(BuffGameplayEffectTrackData);
        protected override Color TrackColor => new(0.9f, 0.6f, 0.6f, 0.2f);
        protected override Color MenuColor => new(0.9f, 0.6f, 0.6f, 1);

        private TimelineAbilityAssetBase AbilityAsset => AbilityTimelineEditorWindow.Instance.AbilityAsset;

        public BuffGameplayEffectTrackData BuffTrackDataForSave
        {
            get
            {
                for (var i = 0; i < AbilityAsset.BuffGameplayEffects.Count; i++)
                    if (AbilityAsset.BuffGameplayEffects[i] == _buffGameplayEffectTrackData)
                        return AbilityAsset.BuffGameplayEffects[i];
                return null;
            }
        }

        public override void TickView(int frameIndex, params object[] param)
        {
        }

        public override void Init(VisualElement trackParent, VisualElement menuParent, float frameWidth,
            TrackDataBase trackData)
        {
            base.Init(trackParent, menuParent, frameWidth, trackData);
            _buffGameplayEffectTrackData = trackData as BuffGameplayEffectTrackData;
            MenuText.text = _buffGameplayEffectTrackData.trackName;
        }

        public override void RefreshShow(float newFrameWidth)
        {
            base.RefreshShow(newFrameWidth);
            foreach (var item in _trackItems) Track.Remove(((TrackClipBase)item).ClipVe);
            _trackItems.Clear();

            if (AbilityTimelineEditorWindow.Instance.AbilityAsset != null)
                foreach (var clipEvent in _buffGameplayEffectTrackData.clipEvents)
                {
                    var item = new BuffGameplayEffectClip();
                    item.InitTrackClip(this, Track, _frameWidth, clipEvent);
                    _trackItems.Add(item);
                }
        }

        protected override void OnAddTrackItem(DropdownMenuAction action)
        {
            // 添加Clip数据
            var clipEvent = new BuffGameplayEffectClipEvent
            {
                startFrame = GetTrackIndexByMouse(action.eventInfo.localMousePosition.x),
                durationFrame = 5
            };
            BuffTrackDataForSave.clipEvents.Add(clipEvent);

            // 刷新显示
            var item = new BuffGameplayEffectClip();
            item.InitTrackClip(this, Track, _frameWidth, clipEvent);
            _trackItems.Add(item);

            // 选中新Clip
            item.ClipVe.OnSelect();

            Debug.Log("[EX] Add a new Buff GameplayEffect Clip");
        }

        protected override void OnRemoveTrack(DropdownMenuAction action)
        {
            // 删除数据
            AbilityAsset.BuffGameplayEffects.Remove(_buffGameplayEffectTrackData);
            AbilityTimelineEditorWindow.Instance.Save();
            // 删除显示
            TrackParent.Remove(TrackRoot);
            MenuParent.Remove(MenuRoot);
            Debug.Log("[EX] Remove Durational Cue Track");
        }


        public override UnityEngine.Object DataInspector => BuffGameplayEffectTrackEditor.Create(this);
    }
}
#endif
using System;
using System.Collections.Generic;
using GAS.Editor.Ability.AbilityTimelineEditor;
using GAS.Runtime.Ability.AbilityTimeline;
using GAS.Runtime.Cue;
using GAS.Runtime.Effects;
using UnityEngine;
using UnityEngine.UIElements;

namespace GAS.Editor.Ability.AbilityTimelineEditor
{
    public class ReleaseGameplayEffectTrack:FixedTrack
    {
        public static ReleaseGameplayEffectTrackData ReleaseGameplayEffectTrackData =>
            AbilityTimelineEditorWindow.Instance.AbilityAsset.ReleaseGameplayEffect;

        public override Type TrackDataType => typeof(ReleaseGameplayEffectTrackData);
        protected override Color TrackColor => new(0.9f, 0.3f, 0.35f, 0.2f);
        protected override Color MenuColor => new(0.9f, 0.3f, 0.35f, 0.9f);
        
        public override void Init(VisualElement trackParent, VisualElement menuParent, float frameWidth,
            TrackDataBase trackData)
        {
            base.Init(trackParent, menuParent, frameWidth, trackData);
            MenuText.text = "施放型GameplayEffect";
        }

        public override void TickView(int frameIndex, params object[] param)
        {
        }

        public override void RefreshShow(float newFrameWidth)
        {
            base.RefreshShow(newFrameWidth);
            foreach (var item in _trackItems) Track.Remove(item.Ve);
            _trackItems.Clear();

            if (AbilityTimelineEditorWindow.Instance.AbilityAsset == null) return;

            foreach (var markEvent in ReleaseGameplayEffectTrackData.markEvents)
            {
                var item = new ReleaseGameplayEffectMark();
                item.InitTrackMark(this, Track, _frameWidth, markEvent);
                _trackItems.Add(item);
            }
        }

        public override VisualElement Inspector()
        {
            var inspector = TrackInspectorUtil.CreateTrackInspector();

            var trackLabel = TrackInspectorUtil.CreateLabel("施放型GameplayEffect:");
            trackLabel.style.fontSize = 14;
            inspector.Add(trackLabel);

            foreach (var mark in ReleaseGameplayEffectTrackData.markEvents)
            {
                var markFrame = TrackInspectorUtil.CreateLabel($"||标记帧:{mark.startFrame}  GameplayEffect数量{mark.gameplayEffectAssets.Count}");
                inspector.Add(markFrame);
                foreach (var ge in mark.gameplayEffectAssets)
                {
                    var geName = ge != null ? ge.name : "NULL";
                    var geNameLabel = TrackInspectorUtil.CreateLabel($"    |-> GameplayEffect:{geName}");
                    inspector.Add(geNameLabel);
                }
            }

            return inspector;
        }

        protected override void OnAddTrackItem(DropdownMenuAction action)
        {
            // 添加Mark数据
            var markEvent = new ReleaseGameplayEffectMarkEvent
            {
                startFrame = GetTrackIndexByMouse(action.eventInfo.localMousePosition.x),
                gameplayEffectAssets = new List<GameplayEffectAsset>()
            };
            ReleaseGameplayEffectTrackData.markEvents.Add(markEvent);

            // 刷新显示
            var mark = new ReleaseGameplayEffectMark();
            mark.InitTrackMark(this, Track, _frameWidth, markEvent);
            _trackItems.Add(mark);

            // 选中新Clip
            mark.OnSelect();

            Debug.Log("[EX] Add ReleaseGameplayEffect Mark");
        }

        protected override void OnRemoveTrack(DropdownMenuAction action)
        {
            ReleaseGameplayEffectTrackData.markEvents.Clear();
            AbilityTimelineEditorWindow.Instance.Save();
        }
    }
}
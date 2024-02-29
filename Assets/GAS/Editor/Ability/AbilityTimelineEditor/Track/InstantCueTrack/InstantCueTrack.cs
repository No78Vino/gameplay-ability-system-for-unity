using System;
using System.Collections.Generic;
using GAS.Editor.Ability.AbilityTimelineEditor.Track;
using GAS.Runtime.Ability.AbilityTimeline;
using GAS.Runtime.Cue;
using UnityEngine;
using UnityEngine.UIElements;

namespace GAS.Editor.Ability.AbilityTimelineEditor
{
    public class InstantCueTrack : TrackBase
    {
        public static InstantCueTrackData InstantCueTrackData =>
            AbilityTimelineEditorWindow.Instance.AbilityAsset.InstantCues;

        public override Type TrackDataType => typeof(InstantCueTrackData);
        protected override Color TrackColor => new(0.1f, 0.2f, 0.6f, 0.2f);
        protected override Color MenuColor => new(0.1f, 0.6f, 0.9f, 0.9f);

        public override bool IsFixedTrack()
        {
            return true;
        }

        public override void Init(VisualElement trackParent, VisualElement menuParent, float frameWidth,
            TrackDataBase trackData)
        {
            base.Init(trackParent, menuParent, frameWidth, trackData);
            MenuText.text = "即时Cue轨道";
        }

        public override void TickView(int frameIndex, params object[] param)
        {
        }

        public override void RefreshShow(float newFrameWidth)
        {
            base.RefreshShow(newFrameWidth);
            foreach (var item in _trackItems) Track.Remove(((TrackMarkBase)item).Ve);
            _trackItems.Clear();

            if (AbilityTimelineEditorWindow.Instance.AbilityAsset == null) return;

            foreach (var markEvent in InstantCueTrackData.markEvents)
            {
                var item = new InstantCueMark();
                item.InitTrackMark(this, Track, _frameWidth, markEvent);
                _trackItems.Add(item);
            }
        }

        public override VisualElement Inspector()
        {
            var inspector = TrackInspectorUtil.CreateTrackInspector();

            var trackLabel = TrackInspectorUtil.CreateLabel("即时Cue轨道：");
            trackLabel.style.fontSize = 16;
            inspector.Add(trackLabel);

            foreach (var mark in InstantCueTrackData.markEvents)
            {
                var markFrame = TrackInspectorUtil.CreateLabel($"||标记帧:{mark.startFrame}/ cue数量{mark.cues.Count}");
                inspector.Add(markFrame);
                foreach (var c in mark.cues)
                {
                    var cueName = c != null ? c.name : "NULL";
                    var cueCount = TrackInspectorUtil.CreateLabel($"    |-> Cue:{cueName}");
                    inspector.Add(cueCount);
                }
            }

            return inspector;
        }

        protected override void OnAddTrackItem(DropdownMenuAction action)
        {
            // 添加Mark数据
            var markEvent = new InstantCueMarkEvent
            {
                startFrame = GetTrackIndexByMouse(action.eventInfo.localMousePosition.x),
                cues = new List<GameplayCueInstant>()
            };
            InstantCueTrackData.markEvents.Add(markEvent);

            // 刷新显示
            var mark = new InstantCueMark();
            mark.InitTrackMark(this, Track, _frameWidth, markEvent);
            _trackItems.Add(mark);

            // 选中新Clip
            mark.OnSelect();

            Debug.Log("[EX] Add Instant Cue Mark");
        }

        protected override void OnRemoveTrack(DropdownMenuAction action)
        {
            InstantCueTrackData.markEvents.Clear();
            AbilityTimelineEditorWindow.Instance.Save();
        }
    }
}
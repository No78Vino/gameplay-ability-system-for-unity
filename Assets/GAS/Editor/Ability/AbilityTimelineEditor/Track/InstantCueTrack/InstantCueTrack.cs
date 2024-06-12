

#if UNITY_EDITOR
namespace GAS.Editor
{
    using System;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.UIElements;
    using GAS.Runtime;

    public class InstantCueTrack : TrackBase
    {
        private InstantCueTrackData _instantCuesTrackData;
        private static TimelineAbilityAssetBase AbilityAsset => AbilityTimelineEditorWindow.Instance.AbilityAsset;

        public InstantCueTrackData InstantCueTrackData
        {
            get
            {
                for (var i = 0; i < AbilityAsset.InstantCues.Count; i++)
                    if (AbilityAsset.InstantCues[i] == _instantCuesTrackData)
                        return AbilityAsset.InstantCues[i];
                return null;
            }
        }

        public override Type TrackDataType => typeof(InstantCueTrackData);
        protected override Color TrackColor => new(0.1f, 0.2f, 0.6f, 0.2f);
        protected override Color MenuColor => new(0.1f, 0.6f, 0.9f, 0.9f);

        public override void Init(VisualElement trackParent, VisualElement menuParent, float frameWidth,
            TrackDataBase trackData)
        {
            base.Init(trackParent, menuParent, frameWidth, trackData);
            _instantCuesTrackData = trackData as InstantCueTrackData;
            MenuText.text = _instantCuesTrackData.trackName;
        }

        public override void TickView(int frameIndex, params object[] param)
        {
            foreach (var item in _trackItems)
                ((TrackMarkBase)item).OnTickView(frameIndex);
        }

        public override void RefreshShow(float newFrameWidth)
        {
            base.RefreshShow(newFrameWidth);
            foreach (var item in _trackItems) Track.Remove(((TrackMarkBase)item).Ve);
            _trackItems.Clear();

            if (AbilityTimelineEditorWindow.Instance.AbilityAsset == null) return;

            foreach (var markEvent in _instantCuesTrackData.markEvents)
            {
                var item = new InstantCueMark();
                item.InitTrackMark(this, Track, _frameWidth, markEvent);
                _trackItems.Add(item);
            }
        }

        // public override VisualElement Inspector()
        // {
        //     var inspector = TrackInspectorUtil.CreateTrackInspector();
        //
        //     var trackName =
        //         TrackInspectorUtil.CreateTextField("Name", _instantCuesTrackData.trackName,
        //             e =>
        //             {
        //                 _instantCuesTrackData.trackName = e.newValue;
        //                 MenuText.text = _instantCuesTrackData.trackName;
        //                 AbilityTimelineEditorWindow.Instance.Save();
        //             });
        //     inspector.Add(trackName);
        //
        //     foreach (var mark in _instantCuesTrackData.markEvents)
        //     {
        //         var markFrame = TrackInspectorUtil.CreateLabel($"Trigger(f):{mark.startFrame}");
        //         inspector.Add(markFrame);
        //         foreach (var c in mark.cues)
        //         {
        //             var cueName = c != null ? c.name : "NULL";
        //             var cueCount = TrackInspectorUtil.CreateLabel($"    |-> Cue:{cueName}");
        //             inspector.Add(cueCount);
        //         }
        //     }
        //
        //     return inspector;
        // }

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

            mark.OnSelect();
            Debug.Log("[EX] Add Instant Cue Mark");
        }

        protected override void OnRemoveTrack(DropdownMenuAction action)
        {
            // 删除数据
            AbilityAsset.InstantCues.Remove(_instantCuesTrackData);
            AbilityTimelineEditorWindow.Instance.Save();
            // 删除显示
            TrackParent.Remove(TrackRoot);
            MenuParent.Remove(MenuRoot);
            Debug.Log("[EX] Remove Instant Cue Track");
        }

        public override UnityEngine.Object DataInspector => InstantCueTrackEditor.Create(this);
    }
}
#endif
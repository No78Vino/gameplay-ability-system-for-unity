
#if UNITY_EDITOR
namespace GAS.Editor
{
    using System;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.UIElements;
    using GAS.Runtime;

    public class TimelineTrackView
    {
        private static List<Type> _trackTypeList;
        private static readonly Dictionary<string, Type> _trackTypeMap = new();
        private readonly VisualElement _root;
        private Button _btnAddTrack;
        private VisualElement _contentTrackListParent;
        private MenuTrack _menuBuffGameplayEffect;
        private MenuTrack _menuDurationalCue;
        private MenuTrack _menuInstantCue;
        private MenuTrack _menuInstantTask;
        private MenuTrack _menuOngoingTask;
        private MenuTrack _menuReleaseGameplayEffect;
        private VisualElement _trackMenuParent;

        public TimelineTrackView(VisualElement root)
        {
            _root = root;
            InitTracks();
        }

        public List<TrackBase> TrackList { get; } = new();

        private static AbilityTimelineEditorConfig Config => AbilityTimelineEditorWindow.Instance.Config;
        private static TimelineAbilityAssetBase AbilityAsset => AbilityTimelineEditorWindow.Instance.AbilityAsset;

        private void InitTracks()
        {
            _contentTrackListParent = _root.Q<VisualElement>("ContentTrackList");
            _trackMenuParent = _root.Q<VisualElement>("TrackMenu");

            RefreshTrackDraw();
            UpdateContentSize();
        }

        public void RefreshTrackDraw()
        {
            TrackList.Clear();
            _contentTrackListParent.Clear();
            _trackMenuParent.Clear();
            if (AbilityAsset == null) return;


            // Instant Task
            _menuInstantTask = new MenuTrack();
            _menuInstantTask.Init(_contentTrackListParent, _trackMenuParent, Config.FrameUnitWidth,
                typeof(TaskMarkEventTrack), typeof(TaskMarkEventTrackData), "Instant Task",
                new Color(0.1f, 0.6f, 0.6f, 0.2f), new Color(0.1f, 0.6f, 0.6f, 0.9f));
            foreach (var instantTaskEventTrackData in AbilityAsset.InstantTasks)
            {
                var instantTaskEventTrack = new TaskMarkEventTrack();
                instantTaskEventTrack.Init(_contentTrackListParent, _trackMenuParent, Config.FrameUnitWidth,
                    instantTaskEventTrackData);
                TrackList.Add(instantTaskEventTrack);
            }
            

            // Ongoing Task
            _menuOngoingTask = new MenuTrack();
            // _menuOngoingTask.Init(_contentTrackListParent, _trackMenuParent, Config.FrameUnitWidth,
            //     typeof(TaskClipEventTrack), typeof(TaskClipEventTrackData), "Ongoing Task",
            //     new Color(0.7f, 0.3f, 0.7f, 0.2f), new Color(0.5f, 0.3f, 0.5f, 1));
            // foreach (var customClipEventTrackData in AbilityAsset.OngoingTasks)
            // {
            //     var customClipTrack = new TaskClipEventTrack();
            //     customClipTrack.Init(_contentTrackListParent, _trackMenuParent, Config.FrameUnitWidth,
            //         customClipEventTrackData);
            //     TrackList.Add(customClipTrack);
            // }

            UpdateContentSize();
        }

        public void UpdateContentSize()
        {
            _contentTrackListParent.style.width =
                AbilityTimelineEditorWindow.Instance.CurrentMaxFrame * Config.FrameUnitWidth;
            foreach (var track in TrackList) track.RefreshShow(Config.FrameUnitWidth);
        }
    }
}
#endif
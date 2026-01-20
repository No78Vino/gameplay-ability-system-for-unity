
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
        private readonly VisualElement _root;
        private Button _btnAddTrack;
        private VisualElement _contentTrackListParent;
        private VisualElement _trackMenuParent;
        private VisualElement _trackMain;

        public TimelineTrackView(VisualElement root)
        {
            _root = root;
            InitTracks();
        }

        public List<AbilityTimelineTrack> TrackList { get; } = new();

        private static AbilityTimelineEditorConfig Config => AbilityTimelineEditorWindow.Instance.Config;
        private static XParamTimeline AbilityConfig => AbilityTimelineEditorWindow.Instance.AbilityConfig;

        private void InitTracks()
        {
            _contentTrackListParent = _root.Q<VisualElement>("ContentTrackList");
            _trackMenuParent = _root.Q<VisualElement>("TrackMenu");
            //_trackMain = _root.Q<VisualElement>("");
            _trackMenuParent.AddManipulator(new ContextualMenuManipulator(OnContextMenu));
            
            RefreshTrackDraw();
            UpdateContentSize();
        }

        public void RefreshTrackDraw()
        {
            TrackList.Clear();
            _contentTrackListParent.Clear();
            _trackMenuParent.Clear();
            if (AbilityConfig == null) return;


            // Tracks
            foreach (var trackInfo in AbilityConfig.Tracks)
            {
                var track = new AbilityTimelineTrack();
                track.Init(_contentTrackListParent, _trackMenuParent, Config.FrameUnitWidth,trackInfo);
                TrackList.Add(track);
            }

            UpdateContentSize();
        }

        public void UpdateContentSize()
        {
            _contentTrackListParent.style.width =
                AbilityTimelineEditorWindow.Instance.CurrentMaxFrame * Config.FrameUnitWidth;
            foreach (var track in TrackList) track.RefreshShow(Config.FrameUnitWidth);
        }
        
        private void OnContextMenu(ContextualMenuPopulateEvent obj)
        {
            obj.menu.AppendAction("添加轨道", action =>
            {
                var newTrack = new Track();
                AbilityConfig.Tracks.Add(newTrack);
                var track = new AbilityTimelineTrack();
                track.Init(_contentTrackListParent, _trackMenuParent, Config.FrameUnitWidth,newTrack);
                TrackList.Add(track);
                UpdateContentSize();
            });
        }
    }
}
#endif
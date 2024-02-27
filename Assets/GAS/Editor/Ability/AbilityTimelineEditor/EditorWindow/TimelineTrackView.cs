using System;
using System.Collections.Generic;
using GAS.Editor.Ability.AbilityTimelineEditor.Track;
using GAS.Editor.Ability.AbilityTimelineEditor.Track.AnimationTrack;
using GAS.Runtime.Ability.AbilityTimeline;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace GAS.Editor.Ability.AbilityTimelineEditor
{
    public class TimelineTrackView
    {
        private static List<Type> _trackTypeList;
        private static readonly Dictionary<string, Type> _trackTypeMap = new();
        private readonly VisualElement _root;
        private readonly List<TrackBase> _trackList = new();
        private Button _btnAddTrack;
        private VisualElement _contentTrackListParent;
        private VisualElement _trackMenuParent;
        public AnimationTrack animationTrack;

        public TimelineTrackView(VisualElement root)
        {
            _root = root;
            InitTracks();
        }

        private static AbilityTimelineEditorConfig Config => AbilityTimelineEditorWindow.Instance.Config;

        private void InitTracks()
        {
            _trackTypeList = EditorUtil.FindAllTypesInheritingFrom(typeof(TrackBase));
            _trackTypeMap.Clear();
            foreach (var t in _trackTypeList) _trackTypeMap.Add(t.Name, t);


            _btnAddTrack = _root.Q<Button>("BtnAddTrack");
            _contentTrackListParent = _root.Q<VisualElement>("ContentTrackList");
            _trackMenuParent = _root.Q<VisualElement>("TrackMenu");

            _trackMenuParent.AddManipulator(new ContextualMenuManipulator(OnContextMenu));
            _btnAddTrack.clicked += OnClickAddTrack;

            RefreshTrackDraw();
            UpdateContentSize();
        }

        private void OnClickAddTrack()
        {
            var menu = new GenericMenu();
            foreach (var t in _trackTypeList) menu.AddItem(new GUIContent(t.Name), false, () => CreateTrack(t));
            menu.ShowAsContext();
        }

        public void RefreshTrackDraw()
        {
            _trackList.Clear();
            _contentTrackListParent.Clear();
            _trackMenuParent.Clear();
            animationTrack = new AnimationTrack();
            animationTrack.Init(_contentTrackListParent, _trackMenuParent, Config.FrameUnitWidth,
                AbilityTimelineEditorWindow.Instance.AbilityAsset.AnimationData);
            _trackList.Add(animationTrack);
        }

        public void UpdateContentSize()
        {
            _contentTrackListParent.style.width =
                AbilityTimelineEditorWindow.Instance.CurrentMaxFrame * Config.FrameUnitWidth;
            foreach (var track in _trackList) track.RefreshShow(Config.FrameUnitWidth);
        }

        private void OnContextMenu(ContextualMenuPopulateEvent evt)
        {
            foreach (var t in _trackTypeList)
                evt.menu.AppendAction(t.Name, OnMenuItemClicked, DropdownMenuAction.AlwaysEnabled);
        }

        // 右键菜单项点击回调函数
        private void OnMenuItemClicked(DropdownMenuAction action)
        {
            if (_trackTypeMap.TryGetValue(action.name, out var trackType))
                CreateTrack(trackType);
        }

        /// <summary>
        ///     创建新的轨道
        /// </summary>
        /// <param name="trackType"></param>
        private void CreateTrack(Type trackType)
        {
            // TODO:创建Data
            var data = new CueTrackData();
            // 创建View
            var track = (TrackBase)Activator.CreateInstance(trackType);
            track.Init(_contentTrackListParent, _trackMenuParent, Config.FrameUnitWidth, data);
            _trackList.Add(track);

            Debug.Log("[EX] Add a new track:" + trackType.Name);
        }
    }
}
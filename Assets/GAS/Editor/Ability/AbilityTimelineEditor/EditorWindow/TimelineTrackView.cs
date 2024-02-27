using System;
using System.Collections.Generic;
using GAS.Editor.Ability.AbilityTimelineEditor.Track;
using GAS.Editor.Ability.AbilityTimelineEditor.Track.AnimationTrack;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace GAS.Editor.Ability.AbilityTimelineEditor
{
    public class TimelineTrackView
    {
        private readonly VisualElement _root;
        private readonly List<TrackBase> _trackList = new();
        private VisualElement _contentTrackListParent;
        private VisualElement _trackMenuParent;
        private Button _btnAddTrack;
        public AnimationTrack animationTrack;

        public TimelineTrackView(VisualElement root)
        {
            _root = root;
            InitTracks();
        }

        private static AbilityTimelineEditorConfig Config => AbilityTimelineEditorWindow.Instance.Config;

        private void InitTracks()
        {
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
            List<Type> trackTypeList = EditorUtil.FindAllTypesInheritingFrom(typeof(TrackBase));
            
            // 显示添加轨道的菜单
            var menu = new GenericMenu();

            foreach (var t in trackTypeList)
            {
                menu.AddItem(new GUIContent(t.Name), false, () => CreateTrack(t));
            }
            // menu.AddItem(new GUIContent("Animation Track"), false, () => Debug.Log("Add Animation Track"));
            // menu.AddItem(new GUIContent("Cue Track"), false, () => Debug.Log("Add Cue Track"));
            
            menu.ShowAsContext();
        }

        public void RefreshTrackDraw()
        {
            _trackList.Clear();
            _contentTrackListParent.Clear();
            _trackMenuParent.Clear();
            animationTrack = new AnimationTrack();
            animationTrack.Init(_contentTrackListParent, _trackMenuParent, Config.FrameUnitWidth);
            _trackList.Add(animationTrack);
        }

        public void UpdateContentSize()
        {
            _contentTrackListParent.style.width =
                AbilityTimelineEditorWindow.Instance.CurrentMaxFrame * Config.FrameUnitWidth;
            foreach (var track in _trackList) track.RefreshShow(Config.FrameUnitWidth);
        }

        // TODO: 右键菜单
        private void OnContextMenu(ContextualMenuPopulateEvent evt)
        {
            // 添加右键菜单项
            string[] menuName = { "item1", "item2" };
            foreach (var n in menuName) evt.menu.AppendAction(n, OnMenuItemClicked, DropdownMenuAction.AlwaysEnabled);
        }

        // 右键菜单项点击回调函数
        private void OnMenuItemClicked(DropdownMenuAction action)
        {
            Debug.Log($"Menu Item Clicked: {action.name}");
        }
        
        private void CreateTrack(Type trackType)
        {
            var track = (TrackBase)Activator.CreateInstance(trackType);
            track.Init(_contentTrackListParent, _trackMenuParent, Config.FrameUnitWidth);
            _trackList.Add(track);
            Debug.Log("Add " + trackType.Name);
        }
    }
}
using System;
using System.Collections.Generic;
using GAS.Runtime.Ability;
using GAS.Runtime.Ability.TimelineAbility;
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
        public AnimationTrack.AnimationTrack animationTrack;

        public TimelineTrackView(VisualElement root)
        {
            _root = root;
            InitTracks();
        }

        private static AbilityTimelineEditorConfig Config => AbilityTimelineEditorWindow.Instance.Config;
        private static TimelineAbilityAsset AbilityAsset => AbilityTimelineEditorWindow.Instance.AbilityAsset;

        private void InitTracks()
        {
            _trackTypeList = EditorUtil.FindAllTypesInheritingFrom(typeof(TrackBase),typeof(FixedTrack));
            _trackTypeMap.Clear();
            foreach (var t in _trackTypeList) _trackTypeMap.Add(t.Name, t);


            _btnAddTrack = _root.Q<Button>("BtnAddTrack");
            _contentTrackListParent = _root.Q<VisualElement>("ContentTrackList");
            _trackMenuParent = _root.Q<VisualElement>("TrackMenu");

            // _trackMenuParent.AddManipulator(new ContextualMenuManipulator(OnContextMenu));
            _btnAddTrack.clicked += OnClickAddTrack;

            RefreshTrackDraw();
            UpdateContentSize();
        }

        private void OnClickAddTrack()
        {
            if(AbilityAsset==null) return;
            var menu = new GenericMenu();
            foreach (var t in _trackTypeList) menu.AddItem(new GUIContent($"Add {t.Name}"), false, () => CreateTrack(t));
            menu.ShowAsContext();
        }

        public void RefreshTrackDraw()
        {
            _trackList.Clear();
            _contentTrackListParent.Clear();
            _trackMenuParent.Clear();
            if (AbilityAsset == null) return;

            // 绘制轨道
            // 即时Cue轨道（唯一）
            var instantCueTrack = new InstantCueTrack();
            instantCueTrack.Init(_contentTrackListParent, _trackMenuParent, Config.FrameUnitWidth, AbilityAsset.InstantCues);
            _trackList.Add(instantCueTrack);
            
            // 施放型GameplayEffect轨道（唯一）
            var releaseGameplayEffectTrack = new ReleaseGameplayEffectTrack();
            releaseGameplayEffectTrack.Init(_contentTrackListParent, _trackMenuParent, Config.FrameUnitWidth, AbilityAsset.ReleaseGameplayEffect);
            _trackList.Add(releaseGameplayEffectTrack);
            
            // 自定义即时型事件轨道（唯一）
            var customMarkEventTrack = new TaskMarkEventTrack();
            customMarkEventTrack.Init(_contentTrackListParent, _trackMenuParent, Config.FrameUnitWidth, AbilityAsset.InstantTasks);
            _trackList.Add(customMarkEventTrack);
            
            // 持续Cue轨道
            foreach (var durationalCueTrackData in AbilityAsset.DurationalCues)
            {
                var cueTrack = new DurationalCueTrack();
                cueTrack.Init(_contentTrackListParent, _trackMenuParent, Config.FrameUnitWidth, durationalCueTrackData);
                _trackList.Add(cueTrack);
            }
            
            // 短暂Buff型GameplayEffect轨道
            foreach (var buffGameplayEffectTrackData in AbilityAsset.BuffGameplayEffects)
            {
                var buffTrack = new BuffGameplayEffectTrack();
                buffTrack.Init(_contentTrackListParent, _trackMenuParent, Config.FrameUnitWidth, buffGameplayEffectTrackData);
                _trackList.Add(buffTrack);
            }
            
            // 自定义持续型事件轨道
            foreach (var customClipEventTrackData in AbilityAsset.OngoingTasks)
            {
                var customClipTrack = new TaskClipEventTrack();
                customClipTrack.Init(_contentTrackListParent, _trackMenuParent, Config.FrameUnitWidth, customClipEventTrackData);
                _trackList.Add(customClipTrack);
            }
            
            UpdateContentSize();
        }

        public void UpdateContentSize()
        {
            _contentTrackListParent.style.width =
                AbilityTimelineEditorWindow.Instance.CurrentMaxFrame * Config.FrameUnitWidth;
            foreach (var track in _trackList) track.RefreshShow(Config.FrameUnitWidth);
        }

        /// <summary>
        ///     创建新的轨道
        /// </summary>
        /// <param name="trackType"></param>
        private void CreateTrack(Type trackType)
        {
            // 创建View
            var track = (TrackBase)Activator.CreateInstance(trackType);
            if (track.IsFixedTrack()) return;
            
            // 创建Data
            var dataType = track.TrackDataType;
            var data = (TrackDataBase)Activator.CreateInstance(dataType);
            data.DefaultInit(_trackList.Count);
            data.AddToAbilityAsset(AbilityAsset);

            // 初始化View
            track.Init(_contentTrackListParent, _trackMenuParent, Config.FrameUnitWidth, data);
            _trackList.Add(track);

            Debug.Log("[EX] Add a new track:" + trackType.Name);

            AbilityAsset.Save();
        }

        public void TrackMenusUnSelect()
        {
            foreach (var t in _trackList) t.OnUnSelect();
        }
    }
}
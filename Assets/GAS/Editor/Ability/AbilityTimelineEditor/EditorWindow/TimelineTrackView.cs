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
        private readonly List<TrackBase> _trackList = new List<TrackBase>();
        private Button _btnAddTrack;
        private VisualElement _contentTrackListParent;
        private VisualElement _trackMenuParent;

        public List<TrackBase> TrackList => _trackList;
        private MenuTrack _menuInstantCue;
        private MenuTrack _menuReleaseGameplayEffect;
        private MenuTrack _menuInstantTask;
        private MenuTrack _menuDurationalCue;
        private MenuTrack _menuBuffGameplayEffect;
        private MenuTrack _menuOngoingTask;
        
        public TimelineTrackView(VisualElement root)
        {
            _root = root;
            InitTracks();
        }

        private static AbilityTimelineEditorConfig Config => AbilityTimelineEditorWindow.Instance.Config;
        private static TimelineAbilityAsset AbilityAsset => AbilityTimelineEditorWindow.Instance.AbilityAsset;

        private void InitTracks()
        {
            // _trackTypeList = EditorUtil.FindAllTypesInheritingFrom(typeof(TrackBase),typeof(FixedTrack));
            // _trackTypeMap.Clear();
            // foreach (var t in _trackTypeList) _trackTypeMap.Add(t.Name, t);
            //_btnAddTrack = _root.Q<Button>("BtnAddTrack");
            
            _contentTrackListParent = _root.Q<VisualElement>("ContentTrackList");
            _trackMenuParent = _root.Q<VisualElement>("TrackMenu");

            // _trackMenuParent.AddManipulator(new ContextualMenuManipulator(OnContextMenu));
            // _btnAddTrack.clicked += OnClickAddTrack;

            RefreshTrackDraw();
            UpdateContentSize();
        }

        // private void OnClickAddTrack()
        // {
        //     if(AbilityAsset==null) return;
        //     var menu = new GenericMenu();
        //     foreach (var t in _trackTypeList) menu.AddItem(new GUIContent($"Add {t.Name}"), false, () => CreateTrack(t));
        //     menu.ShowAsContext();
        // }

        public void RefreshTrackDraw()
        {
            _trackList.Clear();
            _contentTrackListParent.Clear();
            _trackMenuParent.Clear();
            if (AbilityAsset == null) return;
            
            // Instant Cue
            _menuInstantCue = new MenuTrack();
            _menuInstantCue.Init(_contentTrackListParent, _trackMenuParent, Config.FrameUnitWidth,
                typeof(InstantCueTrack), typeof(InstantCueTrackData), "Instant Cue",
                new(0.1f, 0.2f, 0.6f, 0.2f), new(0.1f, 0.6f, 0.9f, 0.9f));
            foreach (var durationalCueTrackData in AbilityAsset.InstantCues)
            {
                var instantCueTrack = new InstantCueTrack();
                instantCueTrack.Init(_contentTrackListParent, _trackMenuParent, Config.FrameUnitWidth, durationalCueTrackData);
                _trackList.Add(instantCueTrack);
            }
            
            
            // Release GameplayEffect
            _menuReleaseGameplayEffect = new MenuTrack();
            _menuReleaseGameplayEffect.Init(_contentTrackListParent, _trackMenuParent, Config.FrameUnitWidth,
                typeof(ReleaseGameplayEffectTrack), typeof(ReleaseGameplayEffectTrackData), "Release Effect",
                new(0.9f, 0.3f, 0.35f, 0.2f), new(0.9f, 0.3f, 0.35f, 0.9f));
            foreach (var releaseGameplayEffectTrackData in AbilityAsset.ReleaseGameplayEffect)
            {
                var releaseGameplayEffectTrack = new ReleaseGameplayEffectTrack();
                releaseGameplayEffectTrack.Init(_contentTrackListParent, _trackMenuParent, Config.FrameUnitWidth, releaseGameplayEffectTrackData);
                _trackList.Add(releaseGameplayEffectTrack);
            }
            
            
            // Instant Task
            _menuInstantTask = new MenuTrack();
            _menuInstantTask.Init(_contentTrackListParent, _trackMenuParent, Config.FrameUnitWidth,
                typeof(TaskMarkEventTrack), typeof(TaskMarkEventTrackData), "Instant Task",
                new(0.1f, 0.6f, 0.6f, 0.2f), new(0.1f, 0.6f, 0.6f, 0.9f));
            foreach (var instantTaskEventTrackData in AbilityAsset.InstantTasks)
            {
                var instantTaskEventTrack = new TaskMarkEventTrack();
                instantTaskEventTrack.Init(_contentTrackListParent, _trackMenuParent, Config.FrameUnitWidth, instantTaskEventTrackData);
                _trackList.Add(instantTaskEventTrack);
            }
            
            
            // Durational Cue
            _menuDurationalCue = new MenuTrack();
            _menuDurationalCue.Init(_contentTrackListParent, _trackMenuParent, Config.FrameUnitWidth,
                typeof(DurationalCueTrack), typeof(DurationalCueTrackData), "Durational Cue",
                new Color(0.1f, 0.6f, 0.1f, 0.2f), new Color(0.1f, 0.6f, 0.1f, 1));
            foreach (var durationalCueTrackData in AbilityAsset.DurationalCues)
            {
                var cueTrack = new DurationalCueTrack();
                cueTrack.Init(_contentTrackListParent, _trackMenuParent, Config.FrameUnitWidth, durationalCueTrackData);
                _trackList.Add(cueTrack);
            }
            
            // Buff GameplayEffect
            _menuBuffGameplayEffect = new MenuTrack();
            _menuBuffGameplayEffect.Init(_contentTrackListParent, _trackMenuParent, Config.FrameUnitWidth,
                typeof(BuffGameplayEffectTrack), typeof(BuffGameplayEffectTrackData), "Buff",
                new Color(0.9f, 0.6f, 0.6f, 0.2f), new Color(0.9f, 0.6f, 0.6f, 1));
            foreach (var buffGameplayEffectTrackData in AbilityAsset.BuffGameplayEffects)
            {
                var buffTrack = new BuffGameplayEffectTrack();
                buffTrack.Init(_contentTrackListParent, _trackMenuParent, Config.FrameUnitWidth, buffGameplayEffectTrackData);
                _trackList.Add(buffTrack);
            }
            
            // Ongoing Task
            _menuOngoingTask = new MenuTrack();
            _menuOngoingTask.Init(_contentTrackListParent, _trackMenuParent, Config.FrameUnitWidth,
                typeof(TaskClipEventTrack), typeof(TaskClipEventTrackData), "Ongoing Task",
                new Color(0.7f, 0.3f, 0.7f, 0.2f), new Color(0.5f, 0.3f, 0.5f, 1));
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
        // private void CreateTrack(Type trackType)
        // {
        //     // 创建View
        //     var track = (TrackBase)Activator.CreateInstance(trackType);
        //     if (track.IsFixedTrack()) return;
        //     
        //     // 创建Data
        //     var dataType = track.TrackDataType;
        //     var data = (TrackDataBase)Activator.CreateInstance(dataType);
        //     data.DefaultInit();
        //     data.AddToAbilityAsset(AbilityAsset);
        //
        //     // 初始化View
        //     track.Init(_contentTrackListParent, _trackMenuParent, Config.FrameUnitWidth, data);
        //     _trackList.Add(track);
        //
        //     Debug.Log("[EX] Add a new track:" + trackType.Name);
        //
        //     AbilityAsset.Save();
        // }
    }
}
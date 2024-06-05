
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

            // Instant Cue
            _menuInstantCue = new MenuTrack();
            _menuInstantCue.Init(_contentTrackListParent, _trackMenuParent, Config.FrameUnitWidth,
                typeof(InstantCueTrack), typeof(InstantCueTrackData), "Instant Cue",
                new Color(0.1f, 0.2f, 0.6f, 0.2f), new Color(0.1f, 0.6f, 0.9f, 0.9f));
            foreach (var durationalCueTrackData in AbilityAsset.InstantCues)
            {
                var instantCueTrack = new InstantCueTrack();
                instantCueTrack.Init(_contentTrackListParent, _trackMenuParent, Config.FrameUnitWidth,
                    durationalCueTrackData);
                TrackList.Add(instantCueTrack);
            }


            // Release GameplayEffect
            _menuReleaseGameplayEffect = new MenuTrack();
            _menuReleaseGameplayEffect.Init(_contentTrackListParent, _trackMenuParent, Config.FrameUnitWidth,
                typeof(ReleaseGameplayEffectTrack), typeof(ReleaseGameplayEffectTrackData), "Release Effect",
                new Color(0.9f, 0.3f, 0.35f, 0.2f), new Color(0.9f, 0.3f, 0.35f, 0.9f));
            foreach (var releaseGameplayEffectTrackData in AbilityAsset.ReleaseGameplayEffect)
            {
                var releaseGameplayEffectTrack = new ReleaseGameplayEffectTrack();
                releaseGameplayEffectTrack.Init(_contentTrackListParent, _trackMenuParent, Config.FrameUnitWidth,
                    releaseGameplayEffectTrackData);
                TrackList.Add(releaseGameplayEffectTrack);
            }


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


            // Durational Cue
            _menuDurationalCue = new MenuTrack();
            _menuDurationalCue.Init(_contentTrackListParent, _trackMenuParent, Config.FrameUnitWidth,
                typeof(DurationalCueTrack), typeof(DurationalCueTrackData), "Durational Cue",
                new Color(0.1f, 0.6f, 0.1f, 0.2f), new Color(0.1f, 0.6f, 0.1f, 1));
            foreach (var durationalCueTrackData in AbilityAsset.DurationalCues)
            {
                var cueTrack = new DurationalCueTrack();
                cueTrack.Init(_contentTrackListParent, _trackMenuParent, Config.FrameUnitWidth, durationalCueTrackData);
                TrackList.Add(cueTrack);
            }

            // Buff GameplayEffect
            _menuBuffGameplayEffect = new MenuTrack();
            _menuBuffGameplayEffect.Init(_contentTrackListParent, _trackMenuParent, Config.FrameUnitWidth,
                typeof(BuffGameplayEffectTrack), typeof(BuffGameplayEffectTrackData), "Buff",
                new Color(0.9f, 0.6f, 0.6f, 0.2f), new Color(0.9f, 0.6f, 0.6f, 1));
            foreach (var buffGameplayEffectTrackData in AbilityAsset.BuffGameplayEffects)
            {
                var buffTrack = new BuffGameplayEffectTrack();
                buffTrack.Init(_contentTrackListParent, _trackMenuParent, Config.FrameUnitWidth,
                    buffGameplayEffectTrackData);
                TrackList.Add(buffTrack);
            }

            // Ongoing Task
            _menuOngoingTask = new MenuTrack();
            _menuOngoingTask.Init(_contentTrackListParent, _trackMenuParent, Config.FrameUnitWidth,
                typeof(TaskClipEventTrack), typeof(TaskClipEventTrackData), "Ongoing Task",
                new Color(0.7f, 0.3f, 0.7f, 0.2f), new Color(0.5f, 0.3f, 0.5f, 1));
            foreach (var customClipEventTrackData in AbilityAsset.OngoingTasks)
            {
                var customClipTrack = new TaskClipEventTrack();
                customClipTrack.Init(_contentTrackListParent, _trackMenuParent, Config.FrameUnitWidth,
                    customClipEventTrackData);
                TrackList.Add(customClipTrack);
            }

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
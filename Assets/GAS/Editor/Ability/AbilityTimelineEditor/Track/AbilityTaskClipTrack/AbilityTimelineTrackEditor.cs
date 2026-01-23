using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using UnityEditor;
using UnityEngine;

#if UNITY_EDITOR
namespace GAS.Editor
{
    public class AbilityTimelineTrackEditor : OdinEditorWindow
    {
        [Delayed] [BoxGroup] [LabelText("轨道名[展示用]")] [OnValueChanged(nameof(OnTrackNameChanged))]
        public string trackName;

        [BoxGroup] [HideLabel] [DisplayAsString(TextAlignment.Left, true)]
        public string trackInfo;

        private AbilityTimelineTrack _track;

        public static AbilityTimelineTrackEditor Create(AbilityTimelineTrack track)
        {
            var window = CreateInstance<AbilityTimelineTrackEditor>();
            window._track = track;
            window.trackName = track.TrackData.Name;
            window.UpdateTrackInfo();
            return window;
        }

        private void UpdateTrackInfo()
        {
            trackInfo = "";
            foreach (var clip in _track.TrackData.TaskClips)
                trackInfo += $"[{clip.TaskType}:{clip.Name}]\n   Run(f):{clip.StartTime} -> {clip.EndTime} \n";

            trackInfo = $"<b>{trackInfo}</b>";
        }

        private void OnTrackNameChanged()
        {
            _track.TrackData.Name = trackName;
            _track.RefreshShow();
        }
    }

    [CustomEditor(typeof(AbilityTimelineTrackEditor))]
    public class TaskClipEventTrackInspector : OdinEditorWithoutHeader
    {
    }
}
#endif
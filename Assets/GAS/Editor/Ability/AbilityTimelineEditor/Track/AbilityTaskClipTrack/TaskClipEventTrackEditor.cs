using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using UnityEditor;
using UnityEngine;

#if UNITY_EDITOR
namespace GAS.Editor
{
    public class TaskClipEventTrackEditor : OdinEditorWindow
    {
        [Delayed] [BoxGroup] [LabelText("Name")] [OnValueChanged(nameof(OnTrackNameChanged))]
        public string trackName;

        [BoxGroup] [HideLabel] [DisplayAsString(TextAlignment.Left, true)]
        public string trackInfo;

        private TaskClipEventTrack _track;

        public static TaskClipEventTrackEditor Create(TaskClipEventTrack track)
        {
            var window = CreateInstance<TaskClipEventTrackEditor>();
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
        }
    }

    [CustomEditor(typeof(TaskClipEventTrackEditor))]
    public class TaskClipEventTrackInspector : OdinEditorWithoutHeader
    {
    }
}
#endif
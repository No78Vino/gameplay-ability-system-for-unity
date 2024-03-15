#if UNITY_EDITOR
namespace GAS.Editor
{
    using UnityEditor;
    using Editor;
    using UnityEngine;
    using Sirenix.OdinInspector.Editor;
    using Sirenix.OdinInspector;

    public class TaskClipEventTrackEditor:OdinEditorWindow
    {
        private TaskClipEventTrack _track;
        
        public static TaskClipEventTrackEditor Create(TaskClipEventTrack track)
        {
            var window = new TaskClipEventTrackEditor();
            window._track = track;
            window.TrackName = track.TaskClipTrackDataForSave.trackName;
            window.UpdateTrackInfo();
            return window;
        }
        
        [Delayed]
        [BoxGroup]
        [LabelText("Name")]
        [OnValueChanged("OnTrackNameChanged")]
        public string TrackName;

        [BoxGroup]
        [HideLabel]
        [DisplayAsString(TextAlignment.Left,true)]
        public string TrackInfo;
        
        void UpdateTrackInfo()
        {
            string info = "";
            foreach (var clip in _track.TaskClipTrackDataForSave.clipEvents)
            {
                var taskName = clip.ongoingTask.TaskData.Type;
                var shortName = taskName.Substring(taskName.LastIndexOf('.') + 1);
                info += $"[{shortName}]  Run(f):{clip.startFrame} -> {clip.EndFrame} \n";
            }
            TrackInfo = $"<b>{info}</b>";
        }
        

        void OnTrackNameChanged()
        {
            _track.TaskClipTrackDataForSave.trackName = TrackName;
            _track.MenuText.text = TrackName;
            AbilityTimelineEditorWindow.Instance.Save();
        }
    }
    
    [CustomEditor(typeof(TaskClipEventTrackEditor))]
    public class TaskClipEventTrackInspector:OdinEditorWithoutHeader
    {
    }
}
#endif
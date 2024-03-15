#if UNITY_EDITOR
namespace GAS.Editor
{
    using UnityEditor;
    using Editor;
    using UnityEngine;
    using Sirenix.OdinInspector.Editor;
    using Sirenix.OdinInspector;
    
    public class TaskMarkEventTrackEditor:OdinEditorWindow
    {
        private TaskMarkEventTrack _track;
        
        public static TaskMarkEventTrackEditor Create(TaskMarkEventTrack track)
        {
            var window = new TaskMarkEventTrackEditor();
            window._track = track;
            window.TrackName = track.InstantTaskEventTrackData.trackName;
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
            foreach (var mark in _track.InstantTaskEventTrackData.markEvents)
            {
                info += $"Trigger(f):{mark.startFrame} \n";
                foreach (var task in mark.InstantTasks)
                {
                    var taskName = task.TaskData.Type;
                    var shortName = taskName.Substring(taskName.LastIndexOf('.') + 1);
                    info += $"    |-> {shortName}\n";
                }
                info += "\n";
            }
            TrackInfo = $"<b>{info}</b>";
        }
        

        void OnTrackNameChanged()
        {
            _track.InstantTaskEventTrackData.trackName = TrackName;
            _track.MenuText.text = TrackName;
            AbilityTimelineEditorWindow.Instance.Save();
        }
    }
    
    [CustomEditor(typeof(TaskMarkEventTrackEditor))]
    public class TaskMarkEventTrackInspector:OdinEditorWithoutHeader
    {
    }
}
#endif
#if UNITY_EDITOR
namespace GAS.Editor
{
    using UnityEditor;
    using Editor;
    using UnityEngine;
    using Sirenix.OdinInspector.Editor;
    using Sirenix.OdinInspector;
    
    public class InstantCueTrackEditor:OdinEditorWindow
    {
        private InstantCueTrack _track;
        public static InstantCueTrackEditor Create(InstantCueTrack track)
        {
            var window = new InstantCueTrackEditor();
            window._track = track;
            window.TrackName = track.InstantCueTrackData.trackName;
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
            foreach (var mark in _track.InstantCueTrackData.markEvents)
            {
                info += $"Trigger(f):{mark.startFrame} \n";
                foreach (var c in mark.cues)
                {
                    var cueName = c != null ? c.name : "NULL";
                    info += $"    |-> {cueName}\n";
                }
                info += "\n";
            }

            TrackInfo = $"<b>{info}</b>";
        }
        

        void OnTrackNameChanged()
        {
            Debug.Log("TrackName Changed");
            _track.InstantCueTrackData.trackName = TrackName;
            _track.MenuText.text = TrackName;
            AbilityTimelineEditorWindow.Instance.Save();
        }
    }
    
    [CustomEditor(typeof(InstantCueTrackEditor))]
    public class InstantCueTrackInspector:OdinEditorWithoutHeader
    {
    }
}
#endif
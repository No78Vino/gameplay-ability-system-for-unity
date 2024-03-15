#if UNITY_EDITOR
namespace GAS.Editor
{
    using UnityEditor;
    using Editor;
    using UnityEngine;
    using Sirenix.OdinInspector.Editor;
    using Sirenix.OdinInspector;
    
    public class DurationalCueTrackEditor:OdinEditorWindow
    {
        private DurationalCueTrack _track;
        
        public static DurationalCueTrackEditor Create(DurationalCueTrack track)
        {
            var window = new DurationalCueTrackEditor();
            window._track = track;
            window.TrackName = track.CueTrackDataForSave.trackName;
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
            foreach (var clip in _track.CueTrackDataForSave.clipEvents)
            {
                var clipName = clip.cue != null ? clip.cue.name : "NULL";
                info += $"[{clipName}]  Run(f):{clip.startFrame} -> {clip.EndFrame} \n";
            }
            TrackInfo = $"<b>{info}</b>";
        }
        

        void OnTrackNameChanged()
        {
            _track.CueTrackDataForSave.trackName = TrackName;
            _track.MenuText.text = TrackName;
            AbilityTimelineEditorWindow.Instance.Save();
        }
    }
    
    [CustomEditor(typeof(DurationalCueTrackEditor))]
    public class DurationalCueTrackInspector:OdinEditorWithoutHeader
    {
    }
}
#endif
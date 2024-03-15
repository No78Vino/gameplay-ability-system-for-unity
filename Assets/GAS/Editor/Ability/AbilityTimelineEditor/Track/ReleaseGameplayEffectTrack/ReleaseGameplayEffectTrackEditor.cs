#if UNITY_EDITOR
namespace GAS.Editor
{
    using UnityEditor;
    using Editor;
    using UnityEngine;
    using Sirenix.OdinInspector.Editor;
    using Sirenix.OdinInspector;
    
    public class ReleaseGameplayEffectTrackEditor:OdinEditorWindow
    {
        private ReleaseGameplayEffectTrack _track;
        
        public static ReleaseGameplayEffectTrackEditor Create(ReleaseGameplayEffectTrack track)
        {
            var window = new ReleaseGameplayEffectTrackEditor();
            window._track = track;
            window.TrackName = track.ReleaseGameplayEffectTrackData.trackName;
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
            foreach (var mark in _track.ReleaseGameplayEffectTrackData.markEvents)
            {
                info += $"Trigger(f):{mark.startFrame} \n";
                foreach (var ge in mark.gameplayEffectAssets)
                {
                    var geName = ge != null ? ge.name : "NULL";
                    info += $"    |-> {geName}\n";
                }

                info += "\n";
            }
            TrackInfo = $"<b>{info}</b>";
        }
        

        void OnTrackNameChanged()
        {
            _track.ReleaseGameplayEffectTrackData.trackName = TrackName;
            _track.MenuText.text = TrackName;
            AbilityTimelineEditorWindow.Instance.Save();
        }
    }
    
    [CustomEditor(typeof(ReleaseGameplayEffectTrackEditor))]
    public class ReleaseGameplayEffectTrackInspector:OdinEditorWithoutHeader
    {
    }
}
#endif
#if UNITY_EDITOR
namespace GAS.Editor
{
    using UnityEditor;
    using Editor;
    using UnityEngine;
    using Sirenix.OdinInspector.Editor;
    using Sirenix.OdinInspector;
    
    public class BuffGameplayEffectTrackEditor:OdinEditorWindow
    {
        private BuffGameplayEffectTrack  _track;
        
        public static BuffGameplayEffectTrackEditor Create(BuffGameplayEffectTrack track)
        {
            var window = new BuffGameplayEffectTrackEditor();
            window._track = track;
            window.TrackName = track.BuffTrackDataForSave.trackName;
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
            foreach (var clip in _track.BuffTrackDataForSave.clipEvents)
            {
                var clipName = clip.gameplayEffect != null ? clip.gameplayEffect.name : "NULL";
                info += $"[{clipName}]  Run(f):{clip.startFrame} -> {clip.EndFrame} \n";
            }
            TrackInfo = $"<b>{info}</b>";
        }
        

        void OnTrackNameChanged()
        {
            _track.BuffTrackDataForSave.trackName = TrackName;
            _track.MenuText.text = TrackName;
            AbilityTimelineEditorWindow.Instance.Save();
        }
    }
    
    [CustomEditor(typeof(BuffGameplayEffectTrackEditor))]
    public class BuffGameplayEffectTrackInspector:OdinEditorWithoutHeader
    {
    }
}
#endif
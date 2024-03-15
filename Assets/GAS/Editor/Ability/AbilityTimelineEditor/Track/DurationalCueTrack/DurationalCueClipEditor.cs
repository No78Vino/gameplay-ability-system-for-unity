
#if UNITY_EDITOR
namespace GAS.Editor
{
    using GAS.Runtime.Cue;
    using UnityEditor;
    using Editor;
    using UnityEngine;
    using Sirenix.OdinInspector.Editor;
    using Sirenix.OdinInspector;
    using GAS.Runtime;

    public class DurationalCueClipEditor:OdinEditorWindow
    {
        private TimelineAbilityAsset AbilityAsset => AbilityTimelineEditorWindow.Instance.AbilityAsset;
        
        private DurationalCueClip _clip;
        
        public static DurationalCueClipEditor Create(DurationalCueClip clip)
        {
            var window = new DurationalCueClipEditor();
            window._clip = clip;
            window.Refresh();
            return window;
        }
        
        [BoxGroup]
        [HideLabel]
        [DisplayAsString(TextAlignment.Left,true)]
        public string RunInfo;

        [Delayed]
        [BoxGroup]
        [LabelText("Duration(f)")]
        [OnValueChanged("OnDurationFrameChanged")]
        public int Duration;

        [Delayed]
        [BoxGroup]
        [AssetSelector]
        [OnValueChanged("OnCueChanged")]
        public GameplayCueDurational Cue;
        
        [BoxGroup]
        [Button]
        [GUIColor(0.9f,0.2f,0.2f)]
        void Delete()
        {
            _clip.Delete();
        }
        
        void Refresh()
        {
            RunInfo = $"<b>Run(f):{_clip.DurationalCueClipData.startFrame} -> {_clip.DurationalCueClipData.EndFrame}</b>";
            Duration = _clip.DurationalCueClipData.durationFrame;
            Cue = _clip.DurationalCueClipData.cue;
        }
        
        private void OnDurationFrameChanged()
        {
            // 钳制
            var max = AbilityAsset.FrameCount - _clip.DurationalCueClipData.startFrame;
            Duration = Mathf.Clamp(Duration, 1, max);
            _clip.UpdateClipDataDurationFrame(Duration);
            _clip.RefreshShow(_clip.FrameUnitWidth);
            Refresh();
        }

        private void OnCueChanged()
        {
            _clip.UpdateClipDataCue(Cue);
        }
    }
    
    [CustomEditor(typeof(DurationalCueClipEditor))]
    public class DurationalCueClipInspector:OdinEditorWithoutHeader
    {
    }
}
#endif
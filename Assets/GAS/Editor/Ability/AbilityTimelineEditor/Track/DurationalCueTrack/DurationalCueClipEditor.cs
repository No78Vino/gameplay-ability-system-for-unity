#if UNITY_EDITOR
namespace GAS.Editor
{
    using UnityEditor;
    using Editor;
    using UnityEngine;
    using Sirenix.OdinInspector.Editor;
    using Sirenix.OdinInspector;
    using GAS.Runtime;

    public class DurationalCueClipEditor : OdinEditorWindow
    {
        private TimelineAbilityAssetBase AbilityAsset => AbilityTimelineEditorWindow.Instance.AbilityAsset;

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
        [GUIColor(0.9f, 0.2f, 0.2f)]
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
    public class DurationalCueClipInspector : OdinEditorWithoutHeader
    {
        public override void OnInspectorGUI()
        {
            EditorGUILayout.HelpBox("注意！！ TimelineAbility下的持续性Cue， 只会执行OnAdd（Cue播放的第一帧），OnRemove（Cue播放的最后一帧）及OnTick， 和GameplayEffect相关的方法不会被执行", MessageType.Info);
            
            GUILayout.Space(20);
            
            base.OnInspectorGUI();
        }
    }
}
#endif
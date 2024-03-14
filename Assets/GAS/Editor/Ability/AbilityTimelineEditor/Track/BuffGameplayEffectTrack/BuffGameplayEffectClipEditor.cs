#if UNITY_EDITOR
namespace GAS.Editor
{
    using GAS.Runtime.Effects;
    using UnityEditor;
    using Ability.AbilityTimelineEditor;
    using UnityEngine;
    using Sirenix.OdinInspector.Editor;
    using Sirenix.OdinInspector;
    using GAS.Runtime.Ability.TimelineAbility;
    
    public class BuffGameplayEffectClipEditor:OdinEditorWindow
    {
        private TimelineAbilityAsset AbilityAsset => AbilityTimelineEditorWindow.Instance.AbilityAsset;
        
        private BuffGameplayEffectClip _clip;
        
        public static BuffGameplayEffectClipEditor Create(BuffGameplayEffectClip clip)
        {
            var window = new BuffGameplayEffectClipEditor();
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
        [OnValueChanged("OnBuffChanged")]
        public GameplayEffectAsset Buff;

        [BoxGroup]
        [Button]
        [GUIColor(0.9f,0.2f,0.2f)]
        void Delete()
        {
            _clip.Delete();
        }

        void Refresh()
        {
            RunInfo = $"<b>Run(f):{_clip.BuffGameplayEffectClipData.startFrame} -> {_clip.BuffGameplayEffectClipData.EndFrame}</b>";
            Duration = _clip.BuffGameplayEffectClipData.durationFrame;
        }
        
        private void OnDurationFrameChanged()
        {
            // 钳制
            var max = AbilityAsset.FrameCount - _clip.BuffGameplayEffectClipData.startFrame;
            var newValue = Mathf.Clamp(Duration, 1, max);
            // 保存数据
            _clip.UpdateClipDataDurationFrame(newValue);

            _clip.RefreshShow(_clip.FrameUnitWidth);
            Refresh();
        }
        
        private void OnBuffChanged()
        {
            _clip.UpdateClipDataBuff(Buff);
            _clip.RefreshShow(_clip.FrameUnitWidth);
            Refresh();
        }
        
    }
    
    
    [CustomEditor(typeof(BuffGameplayEffectClipEditor))]
    public class BuffGameplayEffectClipInspector:OdinEditorWithoutHeader
    {
    }
}
#endif
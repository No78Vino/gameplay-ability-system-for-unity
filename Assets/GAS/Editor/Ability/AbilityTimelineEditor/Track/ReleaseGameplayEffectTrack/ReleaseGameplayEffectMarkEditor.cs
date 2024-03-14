using GAS.Runtime.Effects;
using UnityEngine.Serialization;

#if UNITY_EDITOR
namespace GAS.Editor
{
    using System.Collections;
    using System.Linq;
    using GAS.Runtime.Ability;
    using System;
    using System.Collections.Generic;
    using UnityEditor;
    using Ability.AbilityTimelineEditor;
    using UnityEngine;
    using Sirenix.OdinInspector.Editor;
    using Sirenix.OdinInspector;
    using GAS.General;

    using GAS.Editor.Ability;
    
    public class ReleaseGameplayEffectMarkEditor:OdinEditorWindow
    {
        private ReleaseGameplayEffectMark _mark;
        public static ReleaseGameplayEffectMarkEditor Create(ReleaseGameplayEffectMark mark)
        {
            var window = new ReleaseGameplayEffectMarkEditor();
            window._mark = mark;
            window.UpdateMarkInfo();
            return window;
        }

        [BoxGroup]
        [HideLabel]
        [DisplayAsString(TextAlignment.Left,true)]
        public string RunInfo;
        
        // TODO TargetCatcher
        
        [Delayed]
        [BoxGroup]
        [AssetSelector]
        [ListDrawerSettings(Expanded = true, DraggableItems = true)]
        [OnValueChanged("OnGameplayEffectListChanged")]
        public List<GameplayEffectAsset> gameplayEffects;
        
        [BoxGroup]
        [Button]
        [GUIColor(0.9f,0.2f,0.2f)]
        void Delete()
        {
            _mark.Delete();
        }
        
        void UpdateMarkInfo()
        {
            RunInfo = $"<b>Trigger(f):{_mark.MarkData.startFrame}</b>";
            gameplayEffects = _mark.MarkDataForSave.gameplayEffectAssets;
           
        }
        
        void OnGameplayEffectListChanged()
        {
            _mark.MarkDataForSave.gameplayEffectAssets = gameplayEffects ;
            AbilityTimelineEditorWindow.Instance.Save();
        }
    }
    
    [CustomEditor(typeof(ReleaseGameplayEffectMarkEditor))]
    public class ReleaseGameplayEffectMarkInspector:OdinEditorWithoutHeader
    {
    }
}
#endif
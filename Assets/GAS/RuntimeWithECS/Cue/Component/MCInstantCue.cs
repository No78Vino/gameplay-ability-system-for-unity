using System;
using System.Collections.Generic;
using GAS.Editor;
using GAS.RuntimeDataHelper.GameplayEffect;
using Sirenix.OdinInspector;
using Unity.Entities;
using UnityEngine;

namespace GAS.RuntimeWithECS.Cue.Component
{
    public class MCInstantCue : IComponentData
    {
        public CueInstant cue;
        
        public MCInstantCue()
        {
        }
        
        public MCInstantCue(CueInstant cue)
        {
            this.cue = cue;
        }
    }
    
    [Serializable]
    public struct InstantCueSetting
    {
        [LabelText("播放的需求Tags")]
        [SerializeField] 
        [ListDrawerSettings]
        [ValueDropdown("@EXEditorHelper.GameplayTagCodeChoices", 
            IsUniqueList = true, 
            HideChildProperties = true)]
        public List<int> requiredTags;

        [LabelText("播放免疫的Tags")]
        [SerializeField] 
        [ListDrawerSettings]
        [ValueDropdown("@EXEditorHelper.GameplayTagCodeChoices", 
            IsUniqueList = true, 
            HideChildProperties = true)]
        public List<int> immunityTags;

        [HideLabel]
        public InstantCueSettingConfig cue;
    }
}
using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using Unity.Entities;
using UnityEngine;

namespace GAS.Runtime
{
    public class MCCue : IComponentData
    {
        public GameplayCueBase cue;
        
        public MCCue()
        {
        }
        
        public MCCue(GameplayCueBase cue)
        {
            this.cue = cue;
        }
    }
    
    [Serializable]
    public struct CueSetting
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

    }
}
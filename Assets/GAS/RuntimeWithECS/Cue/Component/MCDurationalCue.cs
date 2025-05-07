using System;
using System.Collections.Generic;
using GAS.Runtime;
using GAS.RuntimeDataHelper.GameplayEffect;
using GAS.RuntimeDataHelper.Helper;
using Sirenix.OdinInspector;
using Unity.Entities;
using UnityEngine;

namespace GAS.RuntimeWithECS.Cue.Component
{
    public class MCDurationalCue : IComponentData
    {
        public CueDurational cue;
        
        public MCDurationalCue()
        {
        }
        
        public MCDurationalCue(CueDurational cue)
        {
            this.cue = cue;
        }
    }
    
    [Serializable]
    public struct DurationalCueSetting
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
        public MMCSettingConfig MMC;
    }
}
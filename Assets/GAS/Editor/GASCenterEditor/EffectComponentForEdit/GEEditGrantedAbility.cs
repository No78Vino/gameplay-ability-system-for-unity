using System;
using System.Collections.Generic;
using GAS.Runtime;
using Sirenix.OdinInspector;
using UnityEngine;

namespace GAS.Editor
{
    [Serializable]
    public class GEEditGrantedAbility
    {
        [VerticalGroup("A")]
        [LabelText("技能")]
        [LabelWidth(70)]
        public int abilityID;
        
        [VerticalGroup("A")]
        [HorizontalGroup("A/B")]
        [LabelText("技能等级")]
        [LabelWidth(70)]
        [Min(0)]
        public int level;
        
        [VerticalGroup("A")]
        [LabelText("激活策略")]
        [LabelWidth(70)]
        [EnumToggleButtons]
        public GrantedAbilityActivationPolicy ActivationPolicy;
        
        [VerticalGroup("A")]
        [LabelText("失活策略")]
        [LabelWidth(70)]
        [EnumToggleButtons]
        public GrantedAbilityDeactivationPolicy DeactivationPolicy;
        
        [VerticalGroup("A")]
        [LabelText("移除策略")]
        [LabelWidth(70)]
        [EnumToggleButtons]
        public GrantedAbilityRemovePolicy RemovePolicy;
    }
}
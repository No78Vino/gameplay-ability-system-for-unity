using System;
using GAS.General;
using Sirenix.OdinInspector;
using UnityEngine;

namespace GAS.Runtime
{
    [Serializable]
    public struct GrantedAbilityConfig
    {
        [LabelWidth(100)]
        [LabelText(GASTextDefine.LABEL_GRANT_ABILITY_PASSIVE)]
        [Tooltip(GASTextDefine.TIP_GRANT_ABILITY_PASSIVE)]
        public bool Passive;
        
        [LabelWidth(100)]
        [LabelText(GASTextDefine.LABEL_GRANT_ABILITY_FORCE)]
        [Tooltip(GASTextDefine.TIP_GRANT_ABILITY_FORCE)]
        public bool ForcedByEffect;
        
        [LabelWidth(100)]
        [LabelText(GASTextDefine.LABEL_GRANT_ABILITY)]
        [AssetSelector]
        public AbilityAsset AbilityAsset;
    }
    
    public class GrantedAbilityFromEffect
    {
        public readonly bool Passive;
        public readonly bool ForcedByEffect;
        public readonly AbstractAbility Ability;
        public GrantedAbilityFromEffect(GrantedAbilityConfig config)
        {
            Passive = config.Passive;
            ForcedByEffect = config.ForcedByEffect;
            Ability = Activator.CreateInstance(config.AbilityAsset.AbilityType(), args: config.AbilityAsset) as AbstractAbility;
            //SourceEffect = sourceEffect;
        }

        public GrantedAbilityFromEffect(bool passive, bool forcedByEffect, AbstractAbility ability)
        {
            Passive = passive;
            Ability = ability;
            ForcedByEffect = forcedByEffect;
        }

        public GrantedAbilitySpecFromEffect CreateSpec(GameplayEffectSpec sourceEffectSpec)
        {
            var grantedAbility = new GrantedAbilitySpecFromEffect(this,sourceEffectSpec);
            // 是否宿主已经持有该技能，如果已经持有,则Grab标记为true
            if (sourceEffectSpec.Owner.AbilityContainer.HasAbility(Ability))
            {
                grantedAbility.Grab();
            }
            return grantedAbility;
        } 
    }
    
    public class GrantedAbilitySpecFromEffect
    {
        public readonly GrantedAbilityFromEffect GrantedAbility;
        public readonly GameplayEffectSpec SourceEffectSpec;
        public readonly string AbilityName;
        public readonly AbilitySystemComponent Owner;
        
        public bool Grabbed { get; private set; } = false;
        
        public GrantedAbilitySpecFromEffect(GrantedAbilityFromEffect grantedAbility,GameplayEffectSpec sourceEffectSpec)
        {
            GrantedAbility = grantedAbility;
            SourceEffectSpec = sourceEffectSpec;
            AbilityName = GrantedAbility.Ability.Name;
            Owner = SourceEffectSpec.Owner;
            Owner.GrantAbility(GrantedAbility.Ability);
        }

        public AbilitySpec AbilitySpec => Owner.AbilityContainer.AbilitySpecs()[AbilityName];
        
        public bool Passive => GrantedAbility.Passive;
        
        public void Grab() => Grabbed = true;
        public void Ungrab() => Grabbed = false;
    }
}
using System;
using System.Collections.Generic;
using GAS.Runtime.Ability;
using GAS.Runtime.Effects;
using GAS.Runtime.Tags;

namespace GAS.Runtime.Component
{
    public interface IAbilitySystemComponent
    {
        void SetPreset(AbilitySystemComponentPreset ascPreset);
        
        void Init(GameplayTag[] baseTags, Type[] attrSetTypes, AbilityAsset[] baseAbilities);
        
        void SetLevel(int level);
        
        bool HasTag(GameplayTag tag);
        
        bool HasAllTags(GameplayTagSet tags);
        
        bool HasAnyTags(GameplayTagSet tags);
        
        void AddFixedTags(GameplayTagSet tags);
        
        void RemoveFixedTags(GameplayTagSet tags);

        GameplayEffectSpec ApplyGameplayEffectTo(GameplayEffect gameplayEffect,AbilitySystemComponent target);
        
        GameplayEffectSpec ApplyGameplayEffectToSelf(GameplayEffect gameplayEffect);

        void ApplyModFromInstantGameplayEffect(GameplayEffectSpec spec);
        
        void RemoveGameplayEffect(GameplayEffectSpec spec);
        
        void Tick();
        
        Dictionary<string,float> DataSnapshot();
        
        void GrantAbility(AbstractAbility ability);
        
        void RemoveAbility(string abilityName);
        
        float? GetAttributeCurrentValue(string setName,string attributeShortName);
        float? GetAttributeBaseValue(string setName,string attributeShortName);

        bool TryActivateAbility(string abilityName, params object[] args);
        void TryEndAbility(string abilityName);

        CooldownTimer CheckCooldownFromTags(GameplayTagSet tags);
        
        T AttrSet<T>() where T : AttributeSet.AttributeSet;

        void ClearGameplayEffect();
    }
}
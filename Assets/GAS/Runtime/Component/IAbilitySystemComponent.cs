using System;
using System.Collections.Generic;

namespace GAS.Runtime
{
    public interface IAbilitySystemComponent
    {
        void SetPreset(AbilitySystemComponentPreset ascPreset);
        
        void Init(GameplayTag[] baseTags, Type[] attrSetTypes, AbilityAsset[] baseAbilities,int level);
        
        void SetLevel(int level);
        
        bool HasTag(GameplayTag tag);
        
        bool HasAllTags(GameplayTagSet tags);
        
        bool HasAnyTags(GameplayTagSet tags);
        
        void AddFixedTags(GameplayTagSet tags);
        void AddFixedTag(GameplayTag tag);
        
        void RemoveFixedTags(GameplayTagSet tags);
        void RemoveFixedTag(GameplayTag tag);

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
        
        T AttrSet<T>() where T : AttributeSet;

        void ClearGameplayEffect();
    }
}
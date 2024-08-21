using System;
using System.Collections.Generic;

namespace GAS.Runtime
{
    public interface IAbilitySystemComponent
    {
        void SetPreset(AbilitySystemComponentPreset ascPreset);

        void Init(GameplayTag[] baseTags, Type[] attrSetTypes, AbilityAsset[] baseAbilities, int level);

        void SetLevel(int level);

        bool HasTag(in GameplayTag tag);

        bool HasAllTags(in GameplayTagSet tags);

        bool HasAnyTags(in GameplayTagSet tags);

        void AddFixedTags(in GameplayTagSet tags);
        void AddFixedTag(in GameplayTag gameplayTag);

        void RemoveFixedTags(in GameplayTagSet tags);
        void RemoveFixedTag(in GameplayTag gameplayTag);

        EntityRef<GameplayEffectSpec> ApplyGameplayEffectTo(GameplayEffect gameplayEffect, AbilitySystemComponent target);

        EntityRef<GameplayEffectSpec> ApplyGameplayEffectToSelf(GameplayEffect gameplayEffect);

        void ApplyModFromInstantGameplayEffect(GameplayEffectSpec spec);

        void RemoveGameplayEffect(GameplayEffectSpec spec);

        void Tick();

        Dictionary<string, float> DataSnapshot();

        AbilitySpec GrantAbility(AbstractAbility ability);

        void RemoveAbility(string abilityName);

        float? GetAttributeCurrentValue(string setName, string attributeShortName);
        float? GetAttributeBaseValue(string setName, string attributeShortName);

        bool TryActivateAbility(string abilityName, object arg = null);
        void TryEndAbility(string abilityName);

        CooldownTimer CheckCooldownFromTags(in GameplayTagSet tags);

        T AttrSet<T>() where T : AttributeSet;

        void ClearGameplayEffect();

        public object UserData { get; set; }
    }
}
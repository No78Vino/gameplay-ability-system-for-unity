using System.Collections.Generic;
using GAS.Runtime.Effects;
using GAS.Runtime.Tags;

namespace GAS.Runtime.Component
{
    public interface IAbilitySystemComponent
    {
        bool HasAllTags(GameplayTagSet tags);
        
        bool HasAnyTags(GameplayTagSet tags);

        GameplayEffectSpec ApplyGameplayEffectTo(GameplayEffect gameplayEffect,AbilitySystemComponent target);
        
        GameplayEffectSpec ApplyGameplayEffectToSelf(GameplayEffect gameplayEffect);
        
        void RemoveGameplayEffect(GameplayEffectSpec spec);
        
        void Tick();
    }
}
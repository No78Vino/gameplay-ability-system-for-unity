using System.Collections.Generic;
using GAS.Runtime.Effects;
using GAS.Runtime.Tags;

namespace GAS.Runtime.AbilitySystemComponent
{
    public interface IAbilitySystemComponent
    {
        bool HasAllTags(List<GameplayTag> tags);
        
        bool HasAnyTags(List<GameplayTag> tags);

        void ApplyGameplayEffectToSelf(GameplayEffectSpec spec);
        
        void RemoveActiveGameplayEffect(GameplayEffectSpec spec);
        
        void Tick();
    }
}
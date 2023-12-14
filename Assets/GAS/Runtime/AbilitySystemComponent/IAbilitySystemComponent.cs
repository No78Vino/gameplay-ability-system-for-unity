using System.Collections.Generic;
using GAS.Runtime.Tags;

namespace GAS.Runtime.AbilitySystemComponent
{
    public interface IAbilitySystemComponent
    {
        bool HasAllTags(List<GameplayTag> tags);
        
        bool HasAnyTags(List<GameplayTag> tags);
    }
}
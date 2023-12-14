using System.Collections.Generic;
using GAS.Runtime.Tags;
using UnityEngine;

namespace GAS.Runtime.AbilitySystemComponent
{
    public class AbilitySystemComponent: MonoBehaviour,IAbilitySystemComponent
    {
        public bool HasAllTags(List<GameplayTag> tags)
        {
            if (tags.Count == 0) return true;
            // TODO
            // Check ASC Has All Tags
            return false;
        }

        public bool HasAnyTags(List<GameplayTag> tags)
        {
            if (tags.Count == 0) return true;
            // TODO
            return false;
        }
    }
}
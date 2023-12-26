using System;
using GAS.Runtime.Tags;
using UnityEngine;

namespace GAS.Runtime.Ability
{
    /// <summary>
    /// https://github.com/BillEliot/GASDocumentation_Chinese?tab=readme-ov-file#4610-gameplay-ability-spec
    /// goto 4.6.9 Ability Tag
    /// </summary>
    [Serializable]
    public class AbilityTagContainer
    {
        public GameplayTagSet AssetTag;

        public GameplayTagSet CancelAbilityTags;
        public GameplayTagSet BlockAbilityTags;

        public GameplayTagSet ActivationOwnedTag;
        
        public GameplayTagSet ActivationRequiredTags;
        public GameplayTagSet ActivationBlockedTags;

        public GameplayTagSet SourceRequiredTags;
        public GameplayTagSet SourceBlockedTags;
        
        public GameplayTagSet TargetRequiredTags;
        public GameplayTagSet TargetBlockedTags;
    }
}
using GAS.Runtime.Tags;
using UnityEngine;

namespace GAS.Runtime.Ability
{
    public class AbilityTagContainer
    {
        /// <summary>
        /// This tag describes the Gameplay Ability
        /// </summary>
        [SerializeField] public GameplayTagSet AssetTag;
        
        /// <summary>
        ///  These tags are granted to the character while the ability is applied
        /// </summary>
        [SerializeField] public GameplayTagSet GrantedTag;

        /// <summary>
        /// Active Gameplay Abilities (on the same character) that have these tags will be cancelled
        /// </summary>
        [SerializeField] public GameplayTagSet CancelAbilityTags;

        /// <summary>
        /// Gameplay Abilities that have these tags will be blocked from activating on the same character
        /// </summary>
        [SerializeField] public GameplayTagSet BlockAbilityTags;

        /// <summary>
        /// This ability can only be activated if the source character has all of the Required tags
        /// and none of the Ignore tags
        /// </summary>
        [SerializeField] public GameplayTagSet SourceTags;

        /// <summary>
        /// This ability can only be activated if the target character has all of the Required tags
        /// and none of the Ignore tags
        /// </summary>
        [SerializeField] public GameplayTagSet TargetTags;
    }
}
using System.Collections.Generic;
using GAS.Runtime.Ability.AbilityTask;
using GAS.Runtime.Effects;
using GAS.Runtime.Component;
using GAS.Runtime.Tags;

namespace GAS.Runtime.Ability
{
    public abstract class AbstractAbility
    {
        public readonly string Name;
        protected AbilityAsset _dataReference;
        
        // public List<OngoingAbilityTask> OngoingAbilityTasks=new List<OngoingAbilityTask>();
        // public List<AsyncAbilityTask> AsyncAbilityTasks = new List<AsyncAbilityTask>();

        /// <summary>
        ///     For the description of the ability
        /// </summary>
        public readonly AbilityTagContainer Tag;

        public readonly GameplayEffect Cooldown;

        public readonly GameplayEffect Cost;

        public AbstractAbility(AbilityAsset abilityAsset)
        {
            _dataReference = abilityAsset;

            Name = _dataReference.UniqueName;
            Tag = new AbilityTagContainer(
                _dataReference.AssetTag,_dataReference.CancelAbilityTags,_dataReference.BlockAbilityTags,
                _dataReference.ActivationOwnedTag,_dataReference.ActivationRequiredTags,_dataReference.ActivationBlockedTags);
            Cooldown = _dataReference.Cooldown?new GameplayEffect(_dataReference.Cooldown):default;
            Cost = _dataReference.Cost?new GameplayEffect(_dataReference.Cost):default;
        }
        
        public abstract AbilitySpec CreateSpec(AbilitySystemComponent owner);
    }
}
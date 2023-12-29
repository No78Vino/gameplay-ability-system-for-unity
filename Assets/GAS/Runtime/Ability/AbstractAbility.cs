using System.Collections.Generic;
using GAS.Runtime.Ability.AbilityTask;
using GAS.Runtime.Effects;
using GAS.Runtime.Component;
using GAS.Runtime.Tags;

namespace GAS.Runtime.Ability
{
    public abstract class AbstractAbility
    {
        public virtual string Name { get;protected set; }
        protected AbilityAsset _dataReference;
        
        // public List<OngoingAbilityTask> OngoingAbilityTasks=new List<OngoingAbilityTask>();
        // public List<AsyncAbilityTask> AsyncAbilityTasks = new List<AsyncAbilityTask>();

        /// <summary>
        ///     For the description of the ability
        /// </summary>
        public AbilityTagContainer Tag{ get; private set; }

        public GameplayEffect Cooldown { get; private set; }

        public GameplayEffect Cost{ get; private set; }

        public AbstractAbility(AbilityAsset abilityAsset)
        {
            _dataReference = abilityAsset;
            
            Tag = new AbilityTagContainer(
                _dataReference.AssetTag,_dataReference.CancelAbilityTags,_dataReference.BlockAbilityTags,
                _dataReference.ActivationOwnedTag,_dataReference.ActivationRequiredTags,_dataReference.ActivationBlockedTags,
                _dataReference.SourceRequiredTags,_dataReference.SourceBlockedTags,_dataReference.TargetRequiredTags,
                _dataReference.TargetBlockedTags);
            Cooldown = new GameplayEffect(_dataReference.Cooldown);
            Cost = new GameplayEffect(_dataReference.Cost);
        }
        
        public abstract AbilitySpec CreateSpec(AbilitySystemComponent owner);
    }
}
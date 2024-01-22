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
        protected readonly AbilityAsset DataReference;
        
        // public List<OngoingAbilityTask> OngoingAbilityTasks=new List<OngoingAbilityTask>();
        // public List<AsyncAbilityTask> AsyncAbilityTasks = new List<AsyncAbilityTask>();

        public AbilityTagContainer Tag { get; protected set; }

        public GameplayEffect Cooldown{ get; protected set; }

        public  float CooldownTime{ get; protected set; }

        public  GameplayEffect Cost{ get; protected set; }

        public AbstractAbility(AbilityAsset abilityAsset)
        {
            DataReference = abilityAsset;

            Name = DataReference.UniqueName;
            Tag = new AbilityTagContainer(
                DataReference.AssetTag,DataReference.CancelAbilityTags,DataReference.BlockAbilityTags,
                DataReference.ActivationOwnedTag,DataReference.ActivationRequiredTags,DataReference.ActivationBlockedTags);
            Cooldown = DataReference.Cooldown?new GameplayEffect(DataReference.Cooldown):default;
            Cost = DataReference.Cost?new GameplayEffect(DataReference.Cost):default;
            
            CooldownTime = DataReference.CooldownTime;
        }
        
        public AbstractAbility()
        {
        }
        
        public abstract AbilitySpec CreateSpec(AbilitySystemComponent owner);
    }
}
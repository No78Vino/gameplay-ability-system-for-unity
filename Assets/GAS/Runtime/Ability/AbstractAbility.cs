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

        public List<OngoingAbilityTask> OngoingAbilityTasks=new List<OngoingAbilityTask>();
        public List<AsyncAbilityTask> AsyncAbilityTasks = new List<AsyncAbilityTask>();

        public List<GameplayEffect> AppliedEffects = new List<GameplayEffect>();

        /// <summary>
        ///     For the description of the ability
        /// </summary>
        public AbilityTagContainer tag;

        public GameplayEffect Cooldown;

        public GameplayEffect Cost;
        
        public abstract AbilitySpec CreateSpec(AbilitySystemComponent owner);
    }
}
using System.Collections.Generic;
using GAS.Runtime.Ability.AbilityTask;
using GAS.Runtime.Effects;
using GAS.Runtime.Tags;

namespace GAS.Runtime.Ability
{
    public abstract class AbstractAbility
    {
        public string Name { get;protected set; }

        public List<OngoingAbilityTask> OngoingAbilityTasks=new List<OngoingAbilityTask>();
        public List<AsyncAbilityTask> AsyncAbilityTasks = new List<AsyncAbilityTask>();

        public List<GameplayEffect> AppliedEffects = new();

        /// <summary>
        ///     For the description of the ability
        /// </summary>
        public AbilityTagContainer tag;

        public GameplayEffect Cooldown;

        public GameplayEffect Cost;
        
        public abstract AbilitySpec CreateSpec(Component.AbilitySystemComponent owner);
        
        public abstract void ActivateAbility();

        public abstract void EndAbility();

        private GameplayEffectSpec ApplyTo(Component.AbilitySystemComponent asc, GameplayEffect gameplayEffect)
        {
            return !gameplayEffect.NULL ? asc.ApplyGameplayEffectToSelf(gameplayEffect) : null;
        }

        public GameplayEffectSpec ApplyCooldownTo(Component.AbilitySystemComponent asc)
        {
            return ApplyTo(asc, Cooldown);
        }

        public GameplayEffectSpec ApplyCostTo(Component.AbilitySystemComponent asc)
        {
            return ApplyTo(asc, Cost);
        }
    }
}
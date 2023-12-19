using System.Collections.Generic;
using GAS.Runtime.Ability.AbilityTask;
using GAS.Runtime.Effects;
using GAS.Runtime.Tags;

namespace GAS.Runtime.Ability
{
    public abstract class AbstractAbility
    {
        public string Name { get;protected set; }

        public List<OngoingAbilityTask> OngoingAbilityTasks;
        public List<AsyncAbilityTask> AsyncAbilityTasks;

        public List<GameplayEffect> AppliedEffects = new();

        /// <summary>
        ///     For the description of the ability
        /// </summary>
        public GameplayTag AssetTag;

        public GameplayEffect Cooldown;

        public GameplayEffect Cost;
        
        public abstract AbilitySpec CreateSpec(AbilitySystemComponent.AbilitySystemComponent owner);
        
        public abstract void ActivateAbility();

        public abstract void EndAbility();

        private GameplayEffectSpec ApplyTo(AbilitySystemComponent.AbilitySystemComponent asc,
            GameplayEffect gameplayEffect)
        {
            if (!gameplayEffect.Empty)
            {
                var cdSpec = asc.CreateGameplayEffectSpec(Cooldown);
                asc.ApplyGameplayEffectToSelf(cdSpec);
                return cdSpec;
            }

            return null;
        }

        public GameplayEffectSpec ApplyCooldownTo(AbilitySystemComponent.AbilitySystemComponent asc)
        {
            return ApplyTo(asc, Cooldown);
        }

        public GameplayEffectSpec ApplyCostTo(AbilitySystemComponent.AbilitySystemComponent asc)
        {
            return ApplyTo(asc, Cost);
        }
    }
}
using System.Collections.Generic;
using GAS.Runtime.Ability.AbilityTask;
using GAS.Runtime.Effects;
using GAS.Runtime.Tags;

namespace GAS.Runtime.Ability
{
    public struct AbilityCooldownTime
    {
        public float TimeRemaining;
        public float TotalDuration;
    }

    public abstract class AbilitySpec
    {
        public AbstractAbility Ability { get; }

        public Component.AbilitySystemComponent Owner { get; protected set; }

        public float Level { get; private set; }

        public bool IsActive { get;private set; }
        
        public int ActiveCount { get;private set; }

        private object[] _abilityArguments;
        public AbilitySpec(AbstractAbility ability, Component.AbilitySystemComponent owner)
        {
            this.Ability = ability;
            this.Owner = owner;
        }
        
        public virtual bool CanActivateAbility()
        {
            return !IsActive
                    && CheckGameplayTags()
                    && CheckCost()
                    && CheckCooldown().TimeRemaining <= 0;
        }
        
        public abstract bool CheckGameplayTags();
        
        public virtual AbilityCooldownTime CheckCooldown()
        {
            // TODO
            // if (Ability.Cooldown.NULL) return new AbilityCooldownTime();
            // var cooldownTags = Ability.Cooldown.GetGameplayEffectTags().GrantedTags;

            //return Owner.CheckCooldownForTags(cooldownTags);
            // TODO
            return new AbilityCooldownTime();
        }
        
        public virtual void ActivateAbility(params object[] args)
        {
            _abilityArguments = args;
            
            if(IsActive) return;
            IsActive = true;
            ActiveCount++;
            Ability.ActivateAbility();
            
            Ability.AsyncAbilityTasks.ForEach(task => task.Execute(_abilityArguments));
        }
        
        public virtual void EndAbility()
        {
            if(!IsActive) return;
            IsActive = false;
            Ability.EndAbility();
        }

        private GameplayEffectSpec _costCache;

        private GameplayEffectSpec TryGetCostSpec()
        {
            if (_costCache == null || _costCache.Level != Level)
            {
                _costCache = Owner.ApplyGameplayEffectToSelf(Ability.Cost);
            }
            return _costCache;
        }


        public virtual bool CheckCost()
        {
            // TODO
            // if (Ability.Cost.NULL) return true;
            // var costGe = TryGetCostSpec();
            // if (costGe == null) return false;
            //
            // if (costGe.GameplayEffect.DurationPolicy != EffectsDurationPolicy.Instant) return true;

            // for (var i = 0; i < costGe.GameplayEffect.Modifiers.Count; i++)
            // {
            //     var modifier = costGe.GameplayEffect.Modifiers[i];
            //
            //     // Only worry about additive.  Anything else passes.
            //     if (modifier.ModifierOperator != EAttributeModifier.Add) continue;
            //     var costValue = (modifier.ModifierMagnitude.CalculateMagnitude(costGe, modifier.Multiplier)).GetValueOrDefault();
            //
            //     this.Owner.AttributeSystem.GetAttributeValue(modifier.Attribute, out var attributeValue);
            //
            //     // The total attribute after accounting for cost should be >= 0 for the cost check to succeed
            //     if (attributeValue.CurrentValue + costValue < 0) return false;
            //
            // }
            return true;
        }

        public void Tick()
        {
            if (!IsActive) return;
            foreach (var task in Ability.OngoingAbilityTasks)
            {
                task.Execute(_abilityArguments);
            }
        }
    }
}
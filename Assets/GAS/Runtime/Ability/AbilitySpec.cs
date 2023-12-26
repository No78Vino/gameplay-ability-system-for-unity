using GAS.Runtime.Component;
using GAS.Runtime.Effects;
using GAS.Runtime.Effects.Modifier;

namespace GAS.Runtime.Ability
{
    public abstract class AbilitySpec
    {
        private object[] _abilityArguments;

        private GameplayEffectSpec _costSpec;

        public AbilitySpec(AbstractAbility ability, AbilitySystemComponent owner)
        {
            Ability = ability;
            Owner = owner;
        }

        public AbstractAbility Ability { get; }

        public AbilitySystemComponent Owner { get; protected set; }

        public float Level { get; }

        public bool IsActive { get; private set; }

        public int ActiveCount { get; private set; }

        public virtual bool CanActivate()
        {
            return !IsActive
                   && CheckGameplayTags()
                   && CheckCost()
                   && CheckCooldown().TimeRemaining <= 0;
        }

        private bool CheckGameplayTags()
        {
            return Owner.HasAllTags(Ability.tag.SourceRequiredTags);
        }

        protected virtual CooldownTimer CheckCooldown()
        {
            return Ability.Cooldown.NULL
                ? new CooldownTimer()
                : Owner.CheckCooldownFromTags(Ability.Cooldown.TagContainer.GrantedTags);
        }

        public virtual void TryActivateAbility(params object[] args)
        {
            _abilityArguments = args;

            if (!CanActivate()) return;
            IsActive = true;
            ActiveCount++;
            
            ActivateAbility(_abilityArguments);
            Ability.AsyncAbilityTasks.ForEach(task => task.Execute(_abilityArguments));
        }

        public virtual void TryEndAbility()
        {
            if (!IsActive) return;
            IsActive = false;
            EndAbility();
        }

        private GameplayEffectSpec CostSpec()
        {
            if (_costSpec == null || _costSpec.Level != Level)
                _costSpec = Owner.ApplyGameplayEffectToSelf(Ability.Cost);
            return _costSpec;
        }


        public virtual bool CheckCost()
        {
            if (Ability.Cost.NULL) return true;
            var costSpec = CostSpec();
            if (costSpec == null) return false;

            if (costSpec.GameplayEffect.DurationPolicy != EffectsDurationPolicy.Instant) return true;

            foreach (var modifier in costSpec.GameplayEffect.Modifiers)
            {
                // Cost can't be multiply or override ,so only care about additive.
                if (modifier.Operation != GEOperation.Add) continue;

                var costValue = modifier.MMC.CalculateMagnitude(modifier.ModiferMagnitude);
                var attribute = Owner.GetAttribute(modifier.AttributeSetName, modifier.AttributeShortName);

                // The total attribute after accounting for cost should be >= 0 for the cost check to succeed
                if (attribute.CurrentValue + costValue < 0) return false;
            }

            return true;
        }

        public void Tick()
        {
            if (!IsActive) return;
            foreach (var task in Ability.OngoingAbilityTasks) task.Execute(_abilityArguments);
        }
        
        public abstract void ActivateAbility(params object[] args);
        
        public abstract void CancelAbility();

        public abstract void EndAbility();
    }
}
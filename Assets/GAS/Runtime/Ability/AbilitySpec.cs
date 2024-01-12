using GAS.Runtime.Component;
using GAS.Runtime.Effects;
using GAS.Runtime.Effects.Modifier;

namespace GAS.Runtime.Ability
{
    public abstract class AbilitySpec
    {
        private object[] _abilityArguments;

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
                   && CheckGameplayTagsValidTpActivate()
                   && CheckCost()
                   && CheckCooldown().TimeRemaining <= 0;
        }

        private bool CheckGameplayTagsValidTpActivate()
        {
            bool hasAllTags = Owner.HasAllTags(Ability.Tag.ActivationRequiredTags);
            bool notHasAnyTags = !Owner.HasAnyTags(Ability.Tag.ActivationBlockedTags);
            bool notBlockedByOtherAbility = true;
           
            foreach (var kv in Owner.AbilityContainer.AbilitySpecs())
            {
                var abilitySpec = kv.Value;
                if (abilitySpec.IsActive)
                {
                    if (Ability.Tag.AssetTag.HasAnyTags(abilitySpec.Ability.Tag.BlockAbilitiesWithTags))
                    {
                        notBlockedByOtherAbility = false;
                        break;
                    }
                }
                
            }
            return hasAllTags && notHasAnyTags && notBlockedByOtherAbility;
        }

        protected virtual CooldownTimer CheckCooldown()
        {
            return Ability.Cooldown.NULL
                ? new CooldownTimer()
                : Owner.CheckCooldownFromTags(Ability.Cooldown.TagContainer.GrantedTags);
        }

        public virtual bool TryActivateAbility(params object[] args)
        {
            if (!CanActivate()) return false;

            _abilityArguments = args;
            IsActive = true;
            ActiveCount++;
            
            Owner.GameplayTagAggregator.ApplyGameplayAbilityDynamicTag(this);
            ActivateAbility(_abilityArguments);
            return true;
        }

        public virtual void TryEndAbility()
        {
            if (!IsActive) return;
            IsActive = false;

            Owner.GameplayTagAggregator.RestoreGameplayAbilityDynamicTags(this);
            EndAbility();
        }

        public virtual void TryCancelAbility()
        {
            if (!IsActive) return;
            IsActive = false;
            CancelAbility();
        }

        private GameplayEffectSpec CostSpec()
        {
            return Owner.ApplyGameplayEffectToSelf(Ability.Cost);
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

                var costValue = modifier.MMC.CalculateMagnitude(costSpec,modifier.ModiferMagnitude);
                var attributeCurrentValue =
                    Owner.GetAttributeCurrentValue(modifier.AttributeSetName, modifier.AttributeShortName);

                // The total attribute after accounting for cost should be >= 0 for the cost check to succeed
                if (attributeCurrentValue + costValue < 0) return false;
            }

            return true;
        }

        public void Tick()
        {
            // TODO
            if (!IsActive) return;
            AbilityTick();
        }

        protected virtual void AbilityTick()
        {
        }
        
        public abstract void ActivateAbility(params object[] args);

        public abstract void CancelAbility();

        public abstract void EndAbility();
    }
}
using System;

namespace GAS.Runtime
{
    public abstract class AbilitySpec
    {
        protected object[] _abilityArguments;

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
        protected event Action<AbilityActivateResult> _onActivateResult;
        protected event Action _onEndAbility;
        protected event Action _onCancelAbility;

        public void RegisterActivateResult(Action<AbilityActivateResult> onActivateResult)
        {
            _onActivateResult += onActivateResult;
        }
        
        public void UnregisterActivateResult(Action<AbilityActivateResult> onActivateResult)
        {
            _onActivateResult -= onActivateResult;
        }

        public void RegisterEndAbility(Action onEndAbility)
        {
            _onEndAbility += onEndAbility;
        }
        
        public void UnregisterEndAbility(Action onEndAbility)
        {
            _onEndAbility -= onEndAbility;
        }
        
        public void RegisterCancelAbility(Action onCancelAbility)
        {
            _onCancelAbility += onCancelAbility;
        }
        
        public void UnregisterCancelAbility(Action onCancelAbility)
        {
            _onCancelAbility -= onCancelAbility;
        }
        
        public virtual AbilityActivateResult CanActivate()
        {
            if (IsActive) return AbilityActivateResult.FailHasActivated;
            if (!CheckGameplayTagsValidTpActivate()) return AbilityActivateResult.FailTagRequirement;
            if (!CheckCost()) return AbilityActivateResult.FailCost;
            if (CheckCooldown().TimeRemaining > 0) return AbilityActivateResult.FailCooldown;

            return AbilityActivateResult.Success;
        }

        private bool CheckGameplayTagsValidTpActivate()
        {
            var hasAllTags = Owner.HasAllTags(Ability.Tag.ActivationRequiredTags);
            var notHasAnyTags = !Owner.HasAnyTags(Ability.Tag.ActivationBlockedTags);
            var notBlockedByOtherAbility = true;

            foreach (var kv in Owner.AbilityContainer.AbilitySpecs())
            {
                var abilitySpec = kv.Value;
                if (abilitySpec.IsActive)
                    if (Ability.Tag.AssetTag.HasAnyTags(abilitySpec.Ability.Tag.BlockAbilitiesWithTags))
                    {
                        notBlockedByOtherAbility = false;
                        break;
                    }
            }

            return hasAllTags && notHasAnyTags && notBlockedByOtherAbility;
        }

        protected virtual bool CheckCost()
        {
            if (Ability.Cost == null) return true;
            var costSpec = Ability.Cost.CreateSpec(Owner, Owner, Level);
            if (costSpec == null) return false;

            if (Ability.Cost.DurationPolicy != EffectsDurationPolicy.Instant) return true;

            foreach (var modifier in Ability.Cost.Modifiers)
            {
                if (modifier.Operation != GEOperation.Add) continue;

                var costValue = modifier.CalculateMagnitude(costSpec, modifier.ModiferMagnitude);
                var attributeCurrentValue =
                    Owner.GetAttributeCurrentValue(modifier.AttributeSetName, modifier.AttributeShortName);

                if (attributeCurrentValue + costValue < 0) return false;
            }

            return true;
        }

        protected virtual CooldownTimer CheckCooldown()
        {
            return Ability.Cooldown == null
                ? new CooldownTimer { TimeRemaining = 0, Duration = Ability.CooldownTime }
                : Owner.CheckCooldownFromTags(Ability.Cooldown.TagContainer.GrantedTags);
        }

        /// <summary>
        ///     Some skills include wind-up and follow-through, where the wind-up may be interrupted, causing the skill not to be
        ///     successfully released.
        ///     Therefore, the actual timing and logic of skill release (triggering costs and initiating cooldown) should be
        ///     determined by developers within the AbilitySpec,
        ///     rather than being systematically standardized.
        /// </summary>
        public void DoCost()
        {
            if (Ability.Cost != null) Owner.ApplyGameplayEffectToSelf(Ability.Cost);

            if (Ability.Cooldown != null)
            {
                var cdSpec = Owner.ApplyGameplayEffectToSelf(Ability.Cooldown);
                cdSpec.SetDuration(Ability.CooldownTime); // Actually, it should be set by the ability's cooldown time.
            }
        }

        public virtual bool TryActivateAbility(params object[] args)
        {
            _abilityArguments = args;
            var result = CanActivate();
            var success = result == AbilityActivateResult.Success;
            if (success)
            {
                IsActive = true;
                ActiveCount++;
                Owner.GameplayTagAggregator.ApplyGameplayAbilityDynamicTag(this);
                ActivateAbility(_abilityArguments);
            }

            _onActivateResult?.Invoke(result);
            return success;
        }

        public virtual void TryEndAbility()
        {
            if (!IsActive) return;
            IsActive = false;

            Owner.GameplayTagAggregator.RestoreGameplayAbilityDynamicTags(this);
            EndAbility();
            _onEndAbility?.Invoke();
        }

        public virtual void TryCancelAbility()
        {
            if (!IsActive) return;
            IsActive = false;

            Owner.GameplayTagAggregator.RestoreGameplayAbilityDynamicTags(this);
            CancelAbility();
            _onCancelAbility?.Invoke();
        }

        public void Tick()
        {
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

    public abstract class AbilitySpec<T> : AbilitySpec where T : AbstractAbility
    {
        protected T data;

        protected AbilitySpec(T ability, AbilitySystemComponent owner) : base(ability, owner)
        {
            data = ability;
        }
    }
}
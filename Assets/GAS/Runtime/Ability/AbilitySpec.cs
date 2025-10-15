using System;

namespace GAS.Runtime
{
    public abstract class AbilitySpec
    {
        protected object[] _abilityArguments = Array.Empty<object>();

        /// <summary>
        /// 获取激活能力时传递给能力的参数。
        /// </summary>
        /// <remarks>
        /// <para>该属性返回一个对象数组，表示激活能力时传入的参数。</para>
        /// <para>即使没有参数传递，该数组也绝不会是 <c>null</c>，在这种情况下，它将是一个空数组。</para>
        /// </remarks>
        public object[] AbilityArguments => _abilityArguments;

        /// <summary>
        /// 获取或设置与能力关联的自定义数据。
        /// </summary>
        /// <remarks>
        /// <para>此属性用于存储能力的自定义信息，以便在能力的不同任务之间共享数据。</para>
        /// <para>例如，可以在一个技能的任务(AbilityTask)中设置此数据，然后在同一个技能的另一个任务(AbilityTask)中检索和使用该数据。</para>
        /// </remarks>
        public object UserData { get; set; }


        public AbilitySpec(AbstractAbility ability, AbilitySystemCellMono owner)
        {
            Ability = ability;
            Owner = owner;
        }

        public virtual void Dispose()
        {
            _onActivateResult = null;
            _onEndAbility = null;
            _onCancelAbility = null;
        }

        public AbstractAbility Ability { get; }

        public AbilitySystemCellMono Owner { get; protected set; }

        public int Level { get; protected set; }

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

        public virtual void SetLevel(int level)
        {
            Level = level;
        }

        public virtual AbilityActivateResult CanActivate()
        {
            // if (IsActive) return AbilityActivateResult.FailHasActivated;
            // if (!CheckGameplayTagsValidTpActivate()) return AbilityActivateResult.FailTagRequirement;
            // if (!CheckCost()) return AbilityActivateResult.FailCost;
            // if (CheckCooldown().TimeRemaining > 0) return AbilityActivateResult.FailCooldown;

            return AbilityActivateResult.Success;
        }

        protected virtual CooldownTimer CheckCooldown()
        {
            return new CooldownTimer { TimeRemaining = 0, Duration = Ability.CooldownTime };
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
                //Owner.GameplayTagAggregator.ApplyGameplayAbilityDynamicTag(this);

                ActivateAbility(_abilityArguments);
            }

            _onActivateResult?.Invoke(result);
            return success;
        }

        public virtual void TryEndAbility()
        {
            if (!IsActive) return;
            IsActive = false;
            // Owner.GameplayTagAggregator.RestoreGameplayAbilityDynamicTags(this);
            EndAbility();
            _onEndAbility?.Invoke();
        }

        public virtual void TryCancelAbility()
        {
            if (!IsActive) return;
            IsActive = false;

            //Owner.GameplayTagAggregator.RestoreGameplayAbilityDynamicTags(this);
            CancelAbility();
            _onCancelAbility?.Invoke();
        }

        public void Tick()
        {
            if (IsActive)
            {
                AbilityTick();
            }
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
        public T Data { get; private set; }

        protected AbilitySpec(T ability, AbilitySystemCellMono owner) : base(ability, owner)
        {
            Data = ability;
        }
    }
}
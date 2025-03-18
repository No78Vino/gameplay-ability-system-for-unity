using UnityEngine;

namespace GAS.Runtime
{
    internal sealed class GameplayEffectPeriodTicker : IEntity
    {
        public ulong InstanceId { get; private set; }

        public bool IsValid => InstanceId != 0;

        private float _periodRemaining;
        private GameplayEffectSpec _spec;

        public void Awake(GameplayEffectSpec spec)
        {
            InstanceId = IdGenerator.Next;
            _spec = spec;
            _periodRemaining = Period;
        }

        public void Release()
        {
            InstanceId = default;
            _spec = default;
            _periodRemaining = default;
        }

        private float Period => _spec.GameplayEffect.Period;

        public void Tick()
        {
            _spec.TriggerOnTick();

            UpdatePeriod();

            // 上面的代码可能导致spec或自身被移除, 需要再次判断有效性
            if (!this.IsValid || !_spec.IsValid)
            {
                return;
            }

            if (_spec.DurationPolicy == EffectsDurationPolicy.Duration && _spec.DurationRemaining() <= 0)
            {
                // 处理STACKING
                if (_spec.GameplayEffect.Stacking.stackingType == StackingType.None)
                {
                    _spec.RemoveSelf();
                }
                else
                {
                    if (_spec.GameplayEffect.Stacking.expirationPolicy == ExpirationPolicy.ClearEntireStack)
                    {
                        _spec.RemoveSelf();
                    }
                    else if (_spec.GameplayEffect.Stacking.expirationPolicy ==
                             ExpirationPolicy.RemoveSingleStackAndRefreshDuration)
                    {
                        if (_spec.StackCount > 1)
                        {
                            _spec.RefreshStack(_spec.StackCount - 1);
                            _spec.RefreshDuration();
                        }
                        else
                        {
                            _spec.RemoveSelf();
                        }
                    }
                    else if (_spec.GameplayEffect.Stacking.expirationPolicy == ExpirationPolicy.RefreshDuration)
                    {
                        //持续时间结束时,再次刷新Duration，这相当于无限Duration，
                        _spec.RefreshDuration();
                    }
                }
            }
        }

        /// <summary>
        /// 注意: Period 小于 0.01f 可能出现误差, 基本够用了
        /// </summary>
        private void UpdatePeriod()
        {
            // 前提: Period不会动态修改
            if (Period <= 0) return;

            var actualDuration = Time.time - _spec.ActivationTime;
            if (actualDuration < Mathf.Epsilon)
            {
                // 第一次执行
                return;
            }

            var dt = Time.deltaTime;

            if (_spec.DurationPolicy == EffectsDurationPolicy.Duration)
            {
                var excessDuration = actualDuration - _spec.Duration;
                if (excessDuration >= 0)
                {
                    // 如果超出了持续时间，就减去超出的时间, 此时应该是最后一次执行
                    dt -= excessDuration;
                    // 为了避免误差, 保证最后一次边界得到执行机会
                    dt += 0.0001f;
                }
            }

            _periodRemaining -= dt;

            // period触发的行为可能导致所属的GE(_spec)失活/移除, 所属GE移除时会将_spec设置为null
            while (_periodRemaining < 0 && _spec is { IsValid: true, IsActive: true })
            {
                // 不能直接将_periodRemaining重置为Period, 这将累计误差
                _periodRemaining += Period;
                _spec.PeriodExecution.Value?.TriggerOnExecute();
            }
        }

        public void ResetPeriod()
        {
            _periodRemaining = Period;
        }
    }
}
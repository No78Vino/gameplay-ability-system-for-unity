using System;
using UnityEngine;

namespace GAS.Runtime
{
    public class GameplayEffectPeriodTicker
    {
        private float _periodRemaining;
        private readonly GameplayEffectSpec _spec;

        public GameplayEffectPeriodTicker(GameplayEffectSpec spec)
        {
            _spec = spec;
            _periodRemaining = Period;
        }

        private float Period => _spec.GameplayEffect.Period;

        public void Tick()
        {
            _spec.TriggerOnTick();

            UpdatePeriod();

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
        /// 注意：经测试发现，当周期（Period）设置为 0.0001f 时，本功能表现正常。
        /// 然而，当周期减小至 0.00001f 时，由于浮点数精度限制，可能会出现计算误差。
        /// 请在使用较小周期值时特别注意，并考虑可能的误差影响。
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
            var excessDuration = actualDuration - _spec.Duration;
            if (excessDuration > 0)
            {
                // 如果超出了持续时间，就减去超出的时间, 此时应该是最后一次执行
                dt -= excessDuration;
            }

            _periodRemaining -= dt;

            while (_periodRemaining < Mathf.Epsilon)
            {
                // 不能直接将_periodRemaining置为0, 这将累计误差
                _periodRemaining += Period;
                _spec.PeriodExecution?.TriggerOnExecute();
            }
        }

        public void ResetPeriod()
        {
            _periodRemaining = Period;
        }
    }
}
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
            
            _periodRemaining -= Time.deltaTime;
            
            // 保证duration判断结束，在周期GE生效之后
            if (_periodRemaining <= 0)
            {
                ResetPeriod();
                _spec.PeriodExecution?.TriggerOnExecute();
            }
            
            if (_spec.DurationPolicy== EffectsDurationPolicy.Duration && _spec.DurationRemaining() <= 0)
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
        
        public void ResetPeriod()
        {
            _periodRemaining = Period;
        }
    }
}
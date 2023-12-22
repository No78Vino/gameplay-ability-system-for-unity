using UnityEngine;

namespace GAS.Runtime.Effects
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
            
            if (_periodRemaining <= 0)
            {
                _periodRemaining = Period;
                _spec.TriggerOnExecute();
            }
            else
            {
                _periodRemaining -= Time.deltaTime;
            }

            if (_spec.DurationRemaining() <= 0)
            {
                _spec.RemoveSelf();
            }
        }
    }
}
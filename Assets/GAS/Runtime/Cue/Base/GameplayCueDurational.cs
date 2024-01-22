using GAS.Runtime.Component;
using GAS.Runtime.Effects;

namespace GAS.Runtime.Cue
{
    public abstract class GameplayCueDurational : GameplayCue<GameplayCueDurationalSpec>
    {
    }

    public abstract class GameplayCueDurationalSpec : GameplayCueSpec
    {
        protected AbilitySystemComponent _targetASC;

        protected GameplayCueDurationalSpec(GameplayCueDurational cue, GameplayCueParameters parameters) : 
            base(cue, parameters)
        {
            
            if (_parameters.sourceGameplayEffectSpec != null)
            {
                _targetASC = _parameters.sourceGameplayEffectSpec.Owner;
            }
            else if (_parameters.sourceAbilitySpec != null)
            {
                _targetASC = _parameters.sourceAbilitySpec.Owner;
            }
        }

        public abstract void OnAdd();
        public abstract void OnRemove();
        public abstract void OnGameplayEffectActivate();
        public abstract void OnGameplayEffectDeactivate();
        public abstract void OnTick();
    }
}
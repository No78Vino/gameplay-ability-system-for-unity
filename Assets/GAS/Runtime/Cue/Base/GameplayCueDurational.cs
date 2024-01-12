using GAS.Runtime.Component;
using GAS.Runtime.Effects;

namespace GAS.Runtime.Cue
{
    public abstract class GameplayCueDurational : GameplayCue
    {
    }

    public abstract class GameplayCueDurationalSpec : GameplayCueSpec
    {
        protected AbilitySystemComponent _targetASC;

        protected GameplayCueDurationalSpec(GameplayCue cue, GameplayEffectSpec sourceGameplayEffectSpec) : base(cue,
            sourceGameplayEffectSpec)
        {
            _targetASC = _gameplayEffectSpec.Owner;
        }

        public abstract void OnGameplayEffectAdd();
        public abstract void OnGameplayEffectRemove();
        public abstract void OnGameplayEffectActivate();
        public abstract void OnGameplayEffectDeactivate();
        public abstract void OnTick();
    }
}
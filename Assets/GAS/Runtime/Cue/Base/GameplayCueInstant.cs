using GAS.Runtime.Component;
using GAS.Runtime.Effects;

namespace GAS.Runtime.Cue
{
    public enum InstantCueApplyTarget
    {
        Owner,
        Source
    }

    public abstract class GameplayCueInstant : GameplayCue
    {
        public InstantCueApplyTarget applyTarget;
        // public override GameplayCueSpec CreateSpec(GameplayEffectSpec sourceGameplayEffectSpec)
        // {
        //     return new GameplayCueInstantSpec(this,)
        // }
    }

    public abstract class GameplayCueInstantSpec : GameplayCueSpec
    {
        protected AbilitySystemComponent _targetASC;

        public GameplayCueInstantSpec(GameplayCue cue, GameplayEffectSpec sourceGameplayEffectSpec) : base(cue,
            sourceGameplayEffectSpec)
        {
            _targetASC = instantCue.applyTarget == InstantCueApplyTarget.Owner
                ? _gameplayEffectSpec.Owner
                : _gameplayEffectSpec.Source;
        }

        public GameplayCueInstantSpec(GameplayCue cue) : base(cue, null)
        {
        }
        
        private GameplayCueInstant instantCue => _cue as GameplayCueInstant;

        public abstract void Trigger();
    }
}
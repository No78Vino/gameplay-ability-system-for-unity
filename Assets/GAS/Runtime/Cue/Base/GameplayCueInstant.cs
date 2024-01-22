using GAS.Runtime.Component;
using GAS.Runtime.Effects;

namespace GAS.Runtime.Cue
{
    public enum InstantCueApplyTarget
    {
        Owner,
        Source
    }

    public abstract class GameplayCueInstant : GameplayCue<GameplayCueInstantSpec>
    {
        public InstantCueApplyTarget applyTarget;
    }

    public abstract class GameplayCueInstantSpec : GameplayCueSpec
    {
        protected AbilitySystemComponent _targetASC;

        public GameplayCueInstantSpec(GameplayCueInstant cue, GameplayCueParameters parameters) : base(cue,
            parameters)
        {
            if (_parameters.sourceGameplayEffectSpec != null)
            {
                _targetASC = instantCue.applyTarget == InstantCueApplyTarget.Owner
                    ? _parameters.sourceGameplayEffectSpec.Owner
                    : _parameters.sourceGameplayEffectSpec.Source;
            }
            else if (_parameters.sourceAbilitySpec != null)
            {
                _targetASC = _parameters.sourceAbilitySpec.Owner;
            }
        }

        // public GameplayCueInstantSpec(GameplayCue cue) : base(cue, null)
        // {
        // }
        
        private GameplayCueInstant instantCue => _cue as GameplayCueInstant;

        public abstract void Trigger();
    }
}
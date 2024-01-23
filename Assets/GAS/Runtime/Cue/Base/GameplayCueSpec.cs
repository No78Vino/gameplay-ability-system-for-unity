using GAS.Runtime.Component;

namespace GAS.Runtime.Cue
{
    public abstract class GameplayCueSpec
    {
        protected readonly GameplayCue _cue;
        protected readonly GameplayCueParameters _parameters;
        public AbilitySystemComponent Owner { get; protected set; }

        public GameplayCueSpec(GameplayCue cue, GameplayCueParameters cueParameters)
        {
            _cue = cue;
            _parameters = cueParameters;
            if (_parameters.sourceGameplayEffectSpec != null)
            {
                Owner = _parameters.sourceGameplayEffectSpec.Owner;
            }
            else if (_parameters.sourceAbilitySpec != null)
            {
                Owner = _parameters.sourceAbilitySpec.Owner;
            }
        }
    }
}
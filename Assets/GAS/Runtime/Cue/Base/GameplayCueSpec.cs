
namespace GAS.Runtime
{
    public abstract class GameplayCueSpec
    {
        protected readonly GameplayCue _cue;
        protected readonly GameplayCueParameters _parameters;
        public AbilitySystemComponent Owner { get; protected set; }

        public virtual bool Triggerable()
        {
            return _cue.Triggerable(Owner);
        }
        
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
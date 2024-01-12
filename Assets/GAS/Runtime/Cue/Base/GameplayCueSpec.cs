using GAS.Runtime.Effects;

namespace GAS.Runtime.Cue
{
    public abstract class GameplayCueSpec
    {
        protected readonly GameplayCue _cue;
        protected readonly GameplayEffectSpec _gameplayEffectSpec;

        public GameplayCueSpec(GameplayCue cue, GameplayEffectSpec sourceGameplayEffectSpec)
        {
            _cue = cue;
            _gameplayEffectSpec = sourceGameplayEffectSpec;
        }
    }
}
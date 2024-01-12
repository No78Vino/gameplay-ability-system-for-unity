using System;
using GAS.Runtime.Effects;

namespace GAS.Runtime.Cue
{
    public class CuePlaySound : GameplayCueInstant
    {
        // TODO

        public override GameplayCueSpec CreateSpec(GameplayEffectSpec sourceGameplayEffectSpec)
        {
            return new CuePlaySoundSpec(this, sourceGameplayEffectSpec);
        }
    }
    
    public class CuePlaySoundSpec : GameplayCueInstantSpec
    {
        // TODO

        public CuePlaySoundSpec(GameplayCue cue, GameplayEffectSpec sourceGameplayEffectSpec) : base(cue,
            sourceGameplayEffectSpec)
        {
            throw new NotImplementedException();
        }

        public override void Trigger()
        {
            throw new NotImplementedException();
        }
    }
}
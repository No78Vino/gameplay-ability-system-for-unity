using System;

namespace GAS.Runtime
{
    public class CuePlaySound : GameplayCueInstant
    {
        // TODO

        public override GameplayCueInstantSpec CreateSpec(GameplayCueParameters parameters)
        {
            return new CuePlaySoundSpec(this, parameters);
        }
    }
    
    public class CuePlaySoundSpec : GameplayCueInstantSpec
    {
        // TODO

        public CuePlaySoundSpec(CuePlaySound cue, GameplayCueParameters parameters) : base(cue,
            parameters)
        {
        }

        public override void Trigger()
        {
        }
    }
}
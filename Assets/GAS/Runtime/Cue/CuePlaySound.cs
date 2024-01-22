using System;
using GAS.Runtime.Effects;

namespace GAS.Runtime.Cue
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
            throw new NotImplementedException();
        }

        public override void Trigger()
        {
            throw new NotImplementedException();
        }
    }
}
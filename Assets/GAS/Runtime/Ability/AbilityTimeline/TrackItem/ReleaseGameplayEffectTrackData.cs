using System;
using System.Collections.Generic;
using GAS.Runtime.Cue;

namespace GAS.Runtime.Ability.AbilityTimeline
{
    [Serializable]
    public class ReleaseGameplayEffectTrackData:TrackDataBase
    {
        public ReleaseGameplayEffectTargetMethod targetMethod;
        public List<ReleaseGameplayEffectMarkEvent> markEvents;
        
        public override void DefaultInit(int index)
        {
            base.DefaultInit(index);
            markEvents = new List<ReleaseGameplayEffectMarkEvent>();
        }
    }
    
    [Serializable]
    public class ReleaseGameplayEffectMarkEvent:MarkEventBase
    {
        public List<GameplayCueInstant> cues = new List<GameplayCueInstant>();
    }

    public class ReleaseGameplayEffectTargetMethod
    {
        
    }
}
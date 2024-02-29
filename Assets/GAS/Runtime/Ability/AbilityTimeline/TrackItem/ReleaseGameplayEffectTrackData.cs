using System;
using System.Collections.Generic;
using GAS.Runtime.Cue;
using GAS.Runtime.Effects;
using UnityEngine.Serialization;

namespace GAS.Runtime.Ability.AbilityTimeline
{
    [Serializable]
    public class ReleaseGameplayEffectTrackData:TrackDataBase
    {
        public ReleaseGameplayEffectTargetMethod targetMethod;
        public List<ReleaseGameplayEffectMarkEvent> markEvents = new List<ReleaseGameplayEffectMarkEvent>();
    }
    
    [Serializable]
    public class ReleaseGameplayEffectMarkEvent:MarkEventBase
    {
        public List<GameplayEffectAsset> gameplayEffectAssets = new List<GameplayEffectAsset>();
    }

    public class ReleaseGameplayEffectTargetMethod
    {
        
    }
}
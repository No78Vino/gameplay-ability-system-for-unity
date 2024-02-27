using System;
using System.Collections.Generic;
using GAS.Runtime.Effects;

namespace GAS.Runtime.Ability.AbilityTimeline
{
    [Serializable]
    public class GameplayEffectTrackData:TrackDataBase
    {
        public GameplayEffectAsset gameplayEffect;
        public List<GameplayEffectClipEvent> clipEvents = new List<GameplayEffectClipEvent>();
    }
    
    [Serializable]
    public class GameplayEffectClipEvent : ClipEventBase
    {
    }
}
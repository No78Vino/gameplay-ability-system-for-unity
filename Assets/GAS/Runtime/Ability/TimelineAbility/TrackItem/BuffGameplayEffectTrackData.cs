using System;
using System.Collections.Generic;
using GAS.Runtime;
using UnityEngine.Serialization;

namespace GAS.Runtime
{
    [Serializable]
    public class BuffGameplayEffectTrackData:TrackDataBase
    {
        public List<BuffGameplayEffectClipEvent> clipEvents = new List<BuffGameplayEffectClipEvent>();

        public override void AddToAbilityAsset(TimelineAbilityAssetBase abilityAsset)
        {
            base.AddToAbilityAsset(abilityAsset);
            abilityAsset.BuffGameplayEffects.Add(this);
        }

        public override void DefaultInit()
        {
            base.DefaultInit();
            trackName = "Buff";
        }
    }
    
    [Serializable]
    public class BuffGameplayEffectClipEvent : ClipEventBase
    {
        public BuffTarget buffTarget;
        [FormerlySerializedAs("gameplayEffects")] public GameplayEffectAsset gameplayEffect;
    }

    public enum BuffTarget
    {
        Self,
    }
}
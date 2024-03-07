using System;

namespace GAS.Runtime.Ability.TimelineAbility
{
    [Serializable]
    public class TrackDataBase
    {
        public string trackName;
        
        public virtual void AddToAbilityAsset(TimelineAbilityAsset abilityAsset)
        {
        }
        
        public virtual void DefaultInit()
        {
        }
    }
}
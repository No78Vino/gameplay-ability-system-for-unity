using System;

namespace GAS.Runtime
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
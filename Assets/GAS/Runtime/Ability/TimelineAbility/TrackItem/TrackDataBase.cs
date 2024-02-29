using System;

namespace GAS.Runtime.Ability.TimelineAbility
{
    [Serializable]
    public class TrackDataBase
    {
        public int trackIndex;
        
        public virtual void AddToAbilityAsset(TimelineAbilityAsset abilityAsset)
        {
        }
        
        public virtual void DefaultInit(int index)
        {
            trackIndex = index;
        }
    }
}
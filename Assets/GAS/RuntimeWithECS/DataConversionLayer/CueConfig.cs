using System;
using GAS.RuntimeWithECS.Cue;

namespace GAS.Runtime
{
    public class GameplayCueConfig
    {
        public Type CueType { get; set; }

        public ICueParameter CueParameter { get; set; }

        public int[] RequiredTags { get; set; }

        public int[] ImmunityTags { get; set; }

        public GameplayCueConfig(Type cueType, ICueParameter parameter, int[] requiredTags = null, int[] immunityTags = null)
        {
            CueType = cueType;
            CueParameter = parameter;
            RequiredTags = requiredTags;
            ImmunityTags = immunityTags;
        }
        
        public void SetCueTypeAndParameter(Type cueType, ICueParameter parameter)
        {
            CueType = cueType;
            CueParameter = parameter;
        }
        
        public void SetRequiredTags(int[] requiredTags)
        {
            RequiredTags = requiredTags;
        }
        
        public void SetImmunityTags(int[] immunityTags)
        {
            ImmunityTags = immunityTags;
        }
        
        public GameplayCueBase CreateCue()
        {
            return CueHelper.TryCreateCue(CueType, CueParameter);
        }
    }
}
using System;

namespace GAS.Runtime
{
    public class GameplayCueConfig
    {
        public Type CueType { get; set; }

        public IExParameterBase ExParameterBase { get; set; }

        public int[] RequiredTags { get; set; }

        public int[] ImmunityTags { get; set; }

        public GameplayCueConfig(Type cueType, IExParameterBase exParameterBase, int[] requiredTags = null, int[] immunityTags = null)
        {
            CueType = cueType;
            ExParameterBase = exParameterBase;
            RequiredTags = requiredTags;
            ImmunityTags = immunityTags;
        }
        
        public void SetCueTypeAndParameter(Type cueType, IExParameterBase exParameterBase)
        {
            CueType = cueType;
            ExParameterBase = exParameterBase;
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
            return CueHelper.TryCreateCue(CueType, ExParameterBase);
        }
    }
}
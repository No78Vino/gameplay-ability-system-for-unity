using System;

namespace GAS.Runtime
{
    [Serializable]
    public class GameplayCueConfig
    {
        public Type CueType { get; set; }

        public XParam Param { get; set; }

        public int[] RequiredTags { get; set; }

        public int[] ImmunityTags { get; set; }

        public GameplayCueConfig(Type cueType, XParam param, int[] requiredTags = null, int[] immunityTags = null)
        {
            CueType = cueType;
            Param = param;
            RequiredTags = requiredTags;
            ImmunityTags = immunityTags;
        }
        
        public void SetCueTypeAndParameter(Type cueType, XParam xParam)
        {
            CueType = cueType;
            Param = xParam;
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
            return CueHelper.TryCreateCue(CueType, Param);
        }
    }
}
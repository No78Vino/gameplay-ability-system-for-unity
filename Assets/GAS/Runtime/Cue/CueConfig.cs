using System;

namespace GAS.Runtime
{
    [Serializable]
    public class GameplayCueConfig
    {
        public Type CueType { get; set; }

        public XParam Param { get; set; }

        public int[] RequiredTags
        {
            get => RequiredAllTags;
            set => RequiredAllTags = value;
        }

        public int[] ImmunityTags
        {
            get => ImmunityNoneTags;
            set => ImmunityNoneTags = value;
        }

        public int[] RequiredAllTags { get; set; }
        public int[] RequiredAnyTags { get; set; }
        public int[] RequiredNoneTags { get; set; }
        public int[] ImmunityAllTags { get; set; }
        public int[] ImmunityAnyTags { get; set; }
        public int[] ImmunityNoneTags { get; set; }

        public GameplayCueConfig(Type cueType, XParam param, int[] requiredTags = null, int[] immunityTags = null)
        {
            CueType = cueType;
            Param = param;
            SetRequiredTagRequirement(requiredTags, Array.Empty<int>(), Array.Empty<int>());
            SetImmunityTagRequirement(Array.Empty<int>(), Array.Empty<int>(), immunityTags);
        }

        public GameplayCueConfig(Type cueType, XParam param,
            int[] requiredAllTags, int[] requiredAnyTags, int[] requiredNoneTags,
            int[] immunityAllTags, int[] immunityAnyTags, int[] immunityNoneTags)
        {
            CueType = cueType;
            Param = param;
            SetRequiredTagRequirement(requiredAllTags, requiredAnyTags, requiredNoneTags);
            SetImmunityTagRequirement(immunityAllTags, immunityAnyTags, immunityNoneTags);
        }
        
        public void SetCueTypeAndParameter(Type cueType, XParam xParam)
        {
            CueType = cueType;
            Param = xParam;
        }
        
        public void SetRequiredTags(int[] requiredTags)
        {
            SetRequiredTagRequirement(requiredTags, Array.Empty<int>(), Array.Empty<int>());
        }
        
        public void SetImmunityTags(int[] immunityTags)
        {
            SetImmunityTagRequirement(Array.Empty<int>(), Array.Empty<int>(), immunityTags);
        }

        public void SetRequiredTagRequirement(int[] all, int[] any, int[] none)
        {
            RequiredAllTags = all;
            RequiredAnyTags = any;
            RequiredNoneTags = none;
        }

        public void SetImmunityTagRequirement(int[] all, int[] any, int[] none)
        {
            ImmunityAllTags = all;
            ImmunityAnyTags = any;
            ImmunityNoneTags = none;
        }
        
        public GameplayCueBase CreateCue()
        {
            return CueHelper.TryCreateCue(CueType, Param);
        }
    }
}

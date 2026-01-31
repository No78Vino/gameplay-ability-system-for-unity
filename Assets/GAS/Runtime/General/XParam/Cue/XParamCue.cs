using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;

namespace GAS.Runtime
{
    public class XParamCue : XParam
    {
        [ShowInInspector] public int[] RequiredTags { get; set; }

        [ShowInInspector] public int[] ImmunityTags { get; set; }
        
        [ShowInInspector] public string CueType { get; private set; }

        [ShowInInspector] public XParam Param { get; set; }
        
        public void SetCueType(string cueType)
        {
            CueType = cueType;
        }

        public void SetParam(XParam param)
        {
            Param = param;
        }

        public void SetRequiredTags(int[] requiredTags)
        {
            RequiredTags = requiredTags;
        }

        public void SetImmunityTags(int[] immunityTags)
        {
            ImmunityTags = immunityTags;
        }

        public XParamCue()
        {
            CueType = "";
            Param = null;
            RequiredTags = Array.Empty<int>();
            ImmunityTags = Array.Empty<int>();
        }

        public XParamCue(string cueType, XParam param = null, int[] requiredTags = null,
            int[] immunityTags = null)
        {
            CueType = cueType;
            Param = param;
            RequiredTags = requiredTags ?? Array.Empty<int>();
            ImmunityTags = immunityTags ?? Array.Empty<int>();
        }

        public GameplayCueConfig GetCueConfig()
        {
            var cueType = CueHelper.GetCueType(CueType);
            return new GameplayCueConfig(cueType, Param, RequiredTags, ImmunityTags);
        }

#if UNITY_EDITOR
        public void DecodeExcelData(List<object> paramData)
        {
            // RequiredTags
            if (paramData.Count > 0)
            {
                var strTags = paramData[0].ToString();
                var tags = strTags.Split(';');
                var tagList = new List<int>();
                if (tagList == null) throw new ArgumentNullException(nameof(tagList));
                foreach (var tag in tags)
                {
                    if (int.TryParse(tag, out var tagInt))
                    {
                        tagList.Add(tagInt);
                    }
                }
            }
            
            // ImmunityTags
            if (paramData.Count > 1)
            {
                var strTags = paramData[1].ToString();
                var tags = strTags.Split(';');
                var tagList = new List<int>();
                if (tagList == null) throw new ArgumentNullException(nameof(tagList));
                foreach (var tag in tags)
                {
                    if (int.TryParse(tag, out var tagInt))
                    {
                        tagList.Add(tagInt);
                    }
                }
            }
            
            // CueType
            if (paramData.Count > 2)
            {
                CueType = paramData[2].ToString();
            }
            
            // Param
            if (paramData.Count > 3)
            {
                List<object> paramDataForCue = new List<object>();
                for (int i = 3; i < paramData.Count; i++)
                {
                    paramDataForCue.Add(paramData[i]);
                }
                //Param = 
            }
        }

        public List<object> EncodeExcelData()
        {
            var result = new List<object>();
            // if (Value == null || Value.Length == 0)
            // {
            //     result.Add(string.Empty);
            //     return result;
            // }
            //
            // var strData = string.Join(";", Value);
            // result.Add(strData);
            return result;
        }
#endif
    }
}
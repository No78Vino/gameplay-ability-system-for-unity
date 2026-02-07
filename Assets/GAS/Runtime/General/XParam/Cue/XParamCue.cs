using System;
using System.Collections.Generic;
using System.Linq;
using GAS.General;
using Sirenix.OdinInspector;

namespace GAS.Runtime
{
    public class XParamCue : XParam
    {
        [ShowInInspector] [LabelText("需求标签")] [ValueDropdown(nameof(TagChoices), IsUniqueList = true)]
        public List<int> RequiredTags;

        [ShowInInspector] [LabelText("免疫标签")] [ValueDropdown(nameof(TagChoices), IsUniqueList = true)]
        public List<int> ImmunityTags;
        
        
        [ShowInInspector]
        [LabelText("Cue类型")]
        [ValueDropdown(nameof(CueClassChoice))]
        [OnValueChanged(nameof(OnTypeChange))]
        public string CueType { get; private set; }

        [ShowInInspector]
        [HideLabel]
        [HideReferenceObjectPicker]
        public XParam Param { get; set; }
        
        public void SetCueType(string cueType)
        {
            CueType = cueType;
        }

        public void SetParam(XParam param)
        {
            Param = param;
        }

        public void SetCueLogic(GameplayCueUnit cueLogic)
        {
            CueType = cueLogic.CueType.Name;
            Param = cueLogic.Param;
        }
        
        public void SetRequiredTags(int[] requiredTags)
        {
            RequiredTags = requiredTags.ToList();
        }

        public void SetImmunityTags(int[] immunityTags)
        {
            ImmunityTags = immunityTags.ToList();
        }

        public XParamCue()
        {
            CueType = "";
            Param = null;
            RequiredTags = new List<int>();
            ImmunityTags = new List<int>();
        }

        public XParamCue(string cueType, XParam param = null, int[] requiredTags = null,
            int[] immunityTags = null)
        {
            CueType = cueType;
            Param = param;
            RequiredTags = requiredTags!=null ? requiredTags.ToList(): new List<int>();
            ImmunityTags = immunityTags!=null ? immunityTags.ToList(): new List<int>();
        }

        public GameplayCueConfig GetCueConfig()
        {
            var cueType = CueHelper.GetCueType(CueType);
            return new GameplayCueConfig(cueType, Param, RequiredTags.ToArray(), ImmunityTags.ToArray());
        }
        
        public List<ValueDropdownItem> TagChoices => GeneralGasChoiceHelper.Tags();
        public IEnumerable<string> CueClassChoice => CueHelper.GetCueTypeNames();

        private void OnTypeChange()
        {
            Param = CueHelper.CreateCueParameter(CueType);
        }

        
#if UNITY_EDITOR
        public void DecodeExcelData(List<object> paramData)
        {
            // RequiredTags
            RequiredTags = new List<int>();
            if (paramData.Count > 0)
            {
                var strTags = paramData[0].ToString();
                if (strTags != "0")
                {
                    var tags = strTags.Split(';');
                    foreach (var tag in tags)
                        if (int.TryParse(tag, out var tagInt))
                            RequiredTags.Add(tagInt);
                }
            }
            
            // ImmunityTags
            ImmunityTags = new List<int>();
            if (paramData.Count > 1)
            {
                var strTags = paramData[1].ToString();
                if (strTags != "0")
                {
                    var tags = strTags.Split(';');
                    foreach (var tag in tags)
                        if (int.TryParse(tag, out var tagInt))
                            ImmunityTags.Add(tagInt);
                }
            }
            
            // CueType
            if (paramData.Count > 2) 
                CueType = paramData[2].ToString();
            
            // Param
            if (paramData.Count > 3)
            {
                List<object> paramDataForCue = new List<object>();
                for (int i = 3; i < paramData.Count; i++)
                {
                    paramDataForCue.Add(paramData[i]);
                }

                var cueParamType = CueHelper.GetCueLogicParamType(CueType);
                Param = (XParam)Activator.CreateInstance(cueParamType);
                Param.DecodeExcelData(paramDataForCue);
            }
        }

        public List<object> EncodeExcelData()
        {
            var result = new List<object>();
            // RequiredTags
            var strRequiredTags = "";
            if (RequiredTags.Count == 0)
            {
                strRequiredTags = "0";
            }
            else
            {
                for (var i = 0; i < RequiredTags.Count; i++)
                {
                    strRequiredTags += RequiredTags[i].ToString();
                    if (i < RequiredTags.Count - 1) strRequiredTags += ";";
                }
            }

            result.Add(strRequiredTags);
            
            // ImmunityTags
            var strImmunityTags = "";
            if (ImmunityTags.Count == 0)
            {
                strImmunityTags = "0";
            }
            else
            {
                for (var i = 0; i < ImmunityTags.Count; i++)
                {
                    strImmunityTags += ImmunityTags[i].ToString();
                    if (i < ImmunityTags.Count - 1) strImmunityTags += ";";
                }
            }

            result.Add(strImmunityTags);
            // CueType
            result.Add(CueType);
            // Param
            if (Param != null)
                result.AddRange(Param.EncodeExcelData());
           
            return result;
        }


#endif
    }
}
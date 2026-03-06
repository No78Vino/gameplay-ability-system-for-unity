using System;
using System.Collections.Generic;
using GAS.General;
using Sirenix.OdinInspector;

namespace GAS.Runtime
{
    public class XParamEffectIDs:XParam
    {
        [ShowInInspector] 
        [LabelText("buff效果ID")] 
        [ValueDropdown(nameof(GameplayEffectIDChoices), IsUniqueList = true)]
        [BeanField(nameof(SetIDs))]
        public int[] IDs;

        public void SetIDs(int[] value)
        {
            IDs = value;
        }
        
        public XParamEffectIDs()
        {
            IDs = Array.Empty<int>();
        }
        
        public XParamEffectIDs(int[] ds)
        {
            IDs = ds;
        }
        
        public List<ValueDropdownItem> GameplayEffectIDChoices => GeneralGasChoiceHelper.GameplayEffects();
#if UNITY_EDITOR
        public void DecodeExcelData(List<object> paramData)
        {
            if (paramData == null || paramData.Count == 0)
            {
                IDs = Array.Empty<int>();
                return;
            }

            var strData = paramData[0] as string;
            if (string.IsNullOrEmpty(strData))
            {
                IDs = Array.Empty<int>();
                return;
            }

            var strArray = strData.Split(';');
            IDs = new int[strArray.Length];
            for (var i = 0; i < strArray.Length; i++)
                if (int.TryParse(strArray[i], out var val))
                    IDs[i] = val;
                else
                    IDs[i] = 0;
        }

        public List<object> EncodeExcelData()
        {
            var result = new List<object>();
            if (IDs == null || IDs.Length == 0)
            {
                result.Add(string.Empty);
                return result;
            }

            var strData = string.Join(";", IDs);
            result.Add(strData);
            return result;
        }
#endif
    }
}
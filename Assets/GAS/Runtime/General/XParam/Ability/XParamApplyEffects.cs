using System;
using System.Collections.Generic;
using GAS.General;
using Sirenix.OdinInspector;

namespace GAS.Runtime
{
    public class XParamApplyEffects : XParam
    {
        [ShowInInspector] 
        [LabelText("buff效果ID")] 
        [ValueDropdown(nameof(GameplayEffectIDChoices), IsUniqueList = true)]
        public int[] IDs;
        
        [ShowInInspector]
        [LabelText("Catcher类型")]
        //[ValueDropdown(nameof(CatcherClassChoice))]
        [OnValueChanged(nameof(OnTypeChange))]
        public string CatcherType { get; private set; }

        [ShowInInspector]
        [HideLabel]
        [HideReferenceObjectPicker]
        public XParam Param { get; set; }
        
        public void SetIDs(int[] value)
        {
            IDs = value;
        }
        
        public void SetCatcherType(string catcherType)
        {
            CatcherType = catcherType;
        }

        public void SetParam(XParam param)
        {
            Param = param;
        }

        public XParamApplyEffects()
        {
            IDs = Array.Empty<int>();
            CatcherType = string.Empty;
            Param = null;
        }

        public XParamApplyEffects(int[] ids)
        {
            IDs = ids;
        }
        
        public List<ValueDropdownItem> GameplayEffectIDChoices => GeneralGasChoiceHelper.GameplayEffects();
        
        //public IEnumerable<string> CatcherClassChoice => GeneralGasChoiceHelper.Catchers();

        private void OnTypeChange()
        {
#if UNITY_EDITOR
            //Param = AbilityHelper.CreateCatcherParameter(CatcherType);
#endif
        }
        
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
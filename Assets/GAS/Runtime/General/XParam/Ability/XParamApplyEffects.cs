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
        [ValueDropdown(nameof(CatcherClassChoice))]
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
        
        public IEnumerable<string> CatcherClassChoice => TargetCatcherHelper.GetCatcherTypeNames();

        private void OnTypeChange()  
        {  
#if UNITY_EDITOR  
            Param = TargetCatcherHelper.CreateCatcherParameter(CatcherType);  
#endif  
        }
        
#if UNITY_EDITOR
        public void DecodeExcelData(List<object> paramData)  
        {  
            // IDs（原有逻辑保留，slot 0）  
            IDs = Array.Empty<int>();  
            if (paramData.Count > 0)  
            {  
                var strData = paramData[0]?.ToString();  
                if (!string.IsNullOrEmpty(strData))  
                {  
                    var strArray = strData.Split(';');  
                    IDs = new int[strArray.Length];  
                    for (var i = 0; i < strArray.Length; i++)  
                        IDs[i] = int.TryParse(strArray[i], out var val) ? val : 0;  
                }  
            }  
  
            // CatcherType（slot 1）  
            if (paramData.Count > 1)  
                CatcherType = paramData[1]?.ToString() ?? string.Empty;  
  
            // CatcherParam（slot 2+）  
            if (paramData.Count > 2 && !string.IsNullOrEmpty(CatcherType))  
            {  
                var paramDataForCatcher = new List<object>();  
                for (int i = 2; i < paramData.Count; i++)  
                    paramDataForCatcher.Add(paramData[i]);  
  
                var catcherParamType = TargetCatcherHelper.GetCatcherParamType(CatcherType);  
                Param = (XParam)Activator.CreateInstance(catcherParamType);  
                Param.DecodeExcelData(paramDataForCatcher);  
            }  
        }  
  
        public List<object> EncodeExcelData()  
        {  
            var result = new List<object>();  
            // IDs（slot 0）  
            result.Add(IDs == null || IDs.Length == 0 ? string.Empty : string.Join(";", IDs));  
            // CatcherType（slot 1）  
            result.Add(CatcherType ?? string.Empty);  
            // CatcherParam（slot 2+）  
            if (Param != null)  
                result.AddRange(Param.EncodeExcelData());  
            return result;  
        }
#endif
    }
}
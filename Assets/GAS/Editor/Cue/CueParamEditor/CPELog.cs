using System.Collections.Generic;
using GAS.Runtime;
using Sirenix.OdinInspector;

namespace GAS.Editor
{
    public class CPELog:CueParamEditorBase<GameplayCueLog>
    {
        [LabelText("输出日志文本")] public string Message;
        
        public override void DecodeExcelData(List<object> paramData)
        {
            Message = paramData.Count > 0 ? paramData[0].ToString() : string.Empty;
        }

        public override List<object> EncodeExcelData()
        {
            var data = new List<object> { Message };
            return data;
        }
    }
}
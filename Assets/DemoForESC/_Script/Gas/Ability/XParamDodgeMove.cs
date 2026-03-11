using System.Collections.Generic;
using Sirenix.OdinInspector;

namespace GAS.Runtime  
{  
    public class XParamDodgeMove : XParam  
    {  
        [LabelText("闪避速度")]  
        [BeanField(nameof(SetDodgeSpeed),Order = 1)]  
        public float DodgeSpeed;  
  
        [LabelText("前摇帧数")]  
        [BeanField(nameof(SetWindUpFrames),Order = 2)]  
        public int WindUpFrames;  
  
        public void SetDodgeSpeed(float value) => DodgeSpeed = value;  
        public void SetWindUpFrames(int value) => WindUpFrames = value;

#if UNITY_EDITOR
        public void DecodeExcelData(List<object> paramData)
        {
            if (paramData == null || paramData.Count == 0) return;
            
            var strData = paramData[0] as string;
            float.TryParse(strData, out DodgeSpeed);
            
            
            if (paramData.Count <2) return;

            var strData1 = paramData[1] as string;
            int.TryParse(strData1, out WindUpFrames);
        }

        public List<object> EncodeExcelData()
        {
            return new List<object>() { DodgeSpeed,WindUpFrames };
        }
#endif
        
    }  
}
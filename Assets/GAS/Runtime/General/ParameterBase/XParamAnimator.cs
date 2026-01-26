using System.Collections.Generic;
using Sirenix.OdinInspector;

namespace GAS.Runtime
{
    public class XParamAnimator : IExParameterBase
    {
        [ShowInInspector]
        [LabelText("动画机节点路径")]
        public string AnimatorNodePath;
        
        [ShowInInspector]
        [LabelText("动画状态名称")]
        public string AnimationName;

        public void SetAnimatorNodePath(string animatorNodePath)
        {
            AnimatorNodePath = animatorNodePath;
        }
        
        public void SetAnimationName(string animationName)
        {
            AnimationName = animationName;
            
        }
#if UNITY_EDITOR
        public void DecodeExcelData(List<object> paramData)
        {
            if (paramData == null || paramData.Count < 2)
            {
                AnimatorNodePath = string.Empty;
                AnimationName = string.Empty;
                return;
            }

            AnimatorNodePath = paramData[0] as string ?? string.Empty;
            AnimationName = paramData[1] as string ?? string.Empty;
        }

        public List<object> EncodeExcelData()
        {
            var paramData = new List<object>
            {
                AnimatorNodePath,
                AnimationName
            };
            return paramData;
        }
#endif
    }
}
using System.Collections.Generic;
using Sirenix.OdinInspector;

namespace GAS.Runtime
{
    public class XParamAnimator : XParam
    {
        [ShowInInspector]
        [LabelText("动画机节点路径")]
        [BeanField(nameof(SetAnimatorNodePath), Comment = "动画机节点路径")]
        public string AnimatorNodePath;
        
        [ShowInInspector]
        [LabelText("动画状态名称")]
        [BeanField(nameof(SetAnimationName), Comment = "动画状态名称")]
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

            if (paramData[0] is string s)
                AnimatorNodePath = s == XParamDefault.DefaultString ? string.Empty : s;

            if (paramData[1] is string s2)
                AnimationName = s2 == XParamDefault.DefaultString ? string.Empty : s2;
        }

        public List<object> EncodeExcelData()
        {
            var paramData = new List<object>
            {
                string.IsNullOrEmpty(AnimatorNodePath)?XParamDefault.DefaultString:AnimatorNodePath,
                string.IsNullOrEmpty(AnimationName)?XParamDefault.DefaultString:AnimationName,     
            };
            return paramData;
        }
#endif
    }
}
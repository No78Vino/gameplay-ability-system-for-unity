using UnityEngine;

namespace GAS.Runtime
{
    public class TaskDebug : AbilityTaskBase<XParamString>
    {
        protected override void OnBegin(int startFrame)
        {
            Debug.Log(Parameter.Value);
        }

        public TaskDebug(AbilityLogicBase logic) : base(logic)
        {
        }
    }
}
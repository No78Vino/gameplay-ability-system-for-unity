using System;

namespace GAS.Runtime
{
    [Serializable]
    public class TaskDoCost : AbilityTaskBase<XParamNone>
    {
        protected override void OnBegin(int startFrame)
        {
            AbilityUtil.DoCost(_logic.GetAbilityEntity());
        }

        public TaskDoCost(AbilityLogicBase logic) : base(logic)
        {
        }
    }
}
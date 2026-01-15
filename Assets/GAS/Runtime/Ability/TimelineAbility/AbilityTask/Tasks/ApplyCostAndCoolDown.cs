using System;

namespace GAS.Runtime
{
    [Serializable]
    public class ApplyCostAndCoolDown : AbilityTaskBase<ParamNone>
    {
        protected override void OnBegin(int startFrame)
        {
            GAUtil.DoCost(_logic.GetAbilityEntity());
        }

        public ApplyCostAndCoolDown(AbilityLogicBase logic) : base(logic)
        {
        }
    }
}
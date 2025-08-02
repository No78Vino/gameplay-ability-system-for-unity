using System;
using GAS.RuntimeWithECS;

namespace GAS.Runtime
{
    [Serializable]
    public class ApplyCostAndCoolDown : AbilityTaskBase
    {
        public override void OnStart(int startFrame)
        {
            GAUtil.DoCost(_logic.GetAbilityEntity());
        }
    }
}
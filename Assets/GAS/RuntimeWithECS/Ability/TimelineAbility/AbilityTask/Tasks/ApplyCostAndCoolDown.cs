using System;
using GAS.RuntimeWithECS.Ability;

namespace GAS.Runtime
{
    [Serializable]
    public class ApplyCostAndCoolDown : AbilityTaskBase
    {
        public override void OnStart(int startFrame)
        {
            GAUtil.DoCost(_logic.GetAbilityEntity());
        }

        public override void OnEnd(int endFrame)
        {
        }

        public override void OnTick(int frameIndex, int startFrame, int endFrame)
        {
        }
    }
}
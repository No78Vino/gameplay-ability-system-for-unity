using GAS.Runtime;
using Unity.Burst;
using Unity.Entities;

namespace GAS.Runtime
{
    [UpdateInGroup(typeof(SysGrpDurationalEffect))]
    [ UpdateAfter(typeof(SysInvokeEffectContainerIsDirtyEvent))]
    public partial struct SActivateInApplication : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            
        }

        [BurstCompile]
        public void OnDestroy(ref SystemState state)
        {

        }
    }
}
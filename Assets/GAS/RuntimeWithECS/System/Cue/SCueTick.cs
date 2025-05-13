using Unity.Burst;
using Unity.Entities;

namespace GAS.Runtime
{
    [DisableAutoCreation]
    [UpdateInGroup(typeof(SysGroupDisplay))]
    [UpdateAfter(typeof(SCueStart))]
    public partial struct SCueTick : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<ECCuePlaying>();
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
using Unity.Burst;
using Unity.Entities;

namespace GAS.Runtime
{
    [DisableAutoCreation]
    [UpdateInGroup(typeof(SysGroupDisplay))]
    [UpdateAfter(typeof(SCueEnd))]
    public partial struct SCueEnd : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<ECCuePlayable>();
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
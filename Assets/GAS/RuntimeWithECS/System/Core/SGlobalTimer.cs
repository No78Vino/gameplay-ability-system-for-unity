using Unity.Burst;
using Unity.Entities;

namespace GAS.Runtime
{
    public struct GlobalTimer : IComponentData
    {
        public int Frame;
        public int Turn;
    }
        
    [DisableAutoCreation]
    [UpdateInGroup(typeof(FixedStepSimulationSystemGroup))] 
    //[UpdateAfter(typeof(GASManagerInputSystem))]
    public partial struct SGlobalTimer : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            //state.RequireForUpdate<GASRunningTag>();
            state.RequireForUpdate<GlobalTimer>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            var globalFrameTimer = SystemAPI.GetSingletonRW<GlobalTimer>();
            globalFrameTimer.ValueRW.Frame += 1;
        }

        [BurstCompile]
        public void OnDestroy(ref SystemState state)
        {

        }
    }
}
using Unity.Burst;
using Unity.Entities;

namespace GAS.RuntimeWithECS.Core
{
    public struct GlobalFrameTimer : IComponentData
    {
        public int FrameCount;
    }
        
    [UpdateInGroup(typeof(FixedStepSimulationSystemGroup))] 
    [UpdateAfter(typeof(GASManagerInputSystem))]
    public partial struct GASTimerSystem : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<GASRunningTag>();
            state.RequireForUpdate<GlobalFrameTimer>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            foreach (var timer in SystemAPI.Query<RefRW<GlobalFrameTimer>>())
            {
                timer.ValueRW.FrameCount++;
            }
        }

        [BurstCompile]
        public void OnDestroy(ref SystemState state)
        {

        }
    }
}
using Unity.Burst;
using Unity.Entities;

namespace GAS.RuntimeWithECS.Core
{
    public struct GlobalFrameTimer : IComponentData
    {
        public int FrameCount;
    }
        
    [UpdateInGroup(typeof(FixedStepSimulationSystemGroup))] 
    public partial struct GASTimerSystem : ISystem
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
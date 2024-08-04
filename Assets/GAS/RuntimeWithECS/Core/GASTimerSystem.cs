using Unity.Burst;
using Unity.Entities;

namespace GAS.RuntimeWithECS.Core
{
    public struct GlobalFrameTimer : IComponentData
    {
        public int FrameCount;
    }
    
    public struct TurnTimer : IComponentData
    {
        public int Count;
    }
    
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
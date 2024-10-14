using Unity.Burst;
using Unity.Entities;

namespace GAS.RuntimeWithECS.System.GameplayEffect
{
    public partial struct SysApplyEffect : ISystem
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
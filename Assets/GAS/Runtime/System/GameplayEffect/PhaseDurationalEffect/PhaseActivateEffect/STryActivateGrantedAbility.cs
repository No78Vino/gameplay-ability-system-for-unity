using GAS.Runtime;
using Unity.Burst;
using Unity.Entities;

namespace GAS.Runtime.System.GameplayEffect.PhaseDurationalEffect
{
    [UpdateInGroup(typeof(SysGroupActivateEffect))]
    [UpdateBefore(typeof(SActivateEnd))]
    public partial struct STryActivateGrantedAbility : ISystem
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
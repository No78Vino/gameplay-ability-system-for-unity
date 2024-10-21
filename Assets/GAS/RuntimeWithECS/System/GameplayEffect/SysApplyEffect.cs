using GAS.RuntimeWithECS.Core;
using GAS.RuntimeWithECS.System.Core;
using Unity.Burst;
using Unity.Entities;

namespace GAS.RuntimeWithECS.System.GameplayEffect
{
    [UpdateAfter(typeof(SysLaunchGameplayAbilitySystem))]
    public partial struct SysApplyEffect : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            
        }

        //[BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            var queue = GasQueueCenter.EffectsWaitingForApplication();
            foreach (var effect in queue)
            {
                // TODO 应用GE
            }
            if(queue.Count>0) GasQueueCenter.ClearEffectsWaitingForApplication();
        }

        [BurstCompile]
        public void OnDestroy(ref SystemState state)
        {

        }
    }
}
using GAS.Runtime;
using Unity.Burst;
using Unity.Entities;

namespace GAS.Runtime
{
    [UpdateInGroup(typeof(SysGroupDurationalEffect))]
    [UpdateAfter(typeof(SInitDuartionalEffect))]
    public partial struct SysInvokeEffectContainerIsDirtyEvent : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<CEffectDirty>();
        }

        //[BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            // var ecb = new EntityCommandBuffer(Unity.Collections.Allocator.Temp);
            // foreach (var (_, asc) in SystemAPI.Query<RefRW<EffectContainerDirty>>().WithEntityAccess())
            // {
            //     // 触发ASC的EffectContainerDirty事件
            //     GASEventCenter.InvokeOnGameplayEffectContainerIsDirty(asc);
            //     
            //     ecb.RemoveComponent<EffectContainerDirty>(asc);
            // }
            // ecb.Playback(state.EntityManager);
            // ecb.Dispose();
        }

        [BurstCompile]
        public void OnDestroy(ref SystemState state)
        {

        }
    }
}
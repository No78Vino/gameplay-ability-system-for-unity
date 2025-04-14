using GAS.RuntimeWithECS.GameplayEffect.Component;
using GAS.RuntimeWithECS.System.SystemGroup;
using GAS.RuntimeWithECS.Tag.Component;
using Unity.Burst;
using Unity.Entities;

namespace GAS.RuntimeWithECS.System.GameplayEffect.PhaseDurationalEffect
{
    [UpdateInGroup(typeof(SysGroupActivateEffect))]
    [UpdateBefore(typeof(SActivateEnd))]
    public partial struct SEffectGrantedTag : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<CInUsage>();
            state.RequireForUpdate<CInActivationProgress>();
            state.RequireForUpdate<CEffectGrantedTags>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            foreach (var (inUsage, _, grantedTags, ge) in SystemAPI
                         .Query<RefRO<CInUsage>, RefRO<CInActivationProgress>, RefRO<CEffectGrantedTags>>()
                         .WithEntityAccess())
            {
                var owner = inUsage.ValueRO.Target;
                var tags = grantedTags.ValueRO.tags;
                var buff = state.EntityManager.GetBuffer<BTemporaryTag>(owner);
                foreach (var tag in tags)
                    buff.Add(new BTemporaryTag { tag = tag, source = ge });
            }
        }

        [BurstCompile]
        public void OnDestroy(ref SystemState state)
        {
        }
    }
}
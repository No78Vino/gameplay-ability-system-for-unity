using Unity.Burst;
using Unity.Entities;

namespace GAS.Runtime
{
    [UpdateInGroup(typeof(SGActivateEffect))]
    public partial struct SEffectGrantedTagsActivate : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<WipActivateEffect>();
            state.RequireForUpdate<CEffectInstance>();
            state.RequireForUpdate<CEffectGrantedTags>();
            state.RequireForUpdate<CEffectInUsage>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            foreach (var (_, _, grantedTags, inUsage) in
                     SystemAPI.Query<
                         RefRO<CEffectInstance>,
                         RefRO<WipActivateEffect>,
                         RefRO<CEffectGrantedTags>,
                         RefRO<CEffectInUsage>>())
            {

                var tags = grantedTags.ValueRO.tags;
                var entityManager = state.EntityManager;
                var targetAsc = inUsage.ValueRO.Target;
                
                // TODO 添加granted tags到asc
            }
        }

        [BurstCompile]
        public void OnDestroy(ref SystemState state)
        {

        }
    }
}
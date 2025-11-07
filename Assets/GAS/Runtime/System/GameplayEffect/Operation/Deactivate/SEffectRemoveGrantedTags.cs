using Unity.Burst;
using Unity.Entities;

namespace GAS.Runtime
{
    [UpdateInGroup(typeof(SGDeactivateEffect))]
    [UpdateBefore(typeof(SDeactivateEnd))]
    public partial struct SEffectRemoveGrantedTags : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<WipDeactivateEffect>();
            state.RequireForUpdate<CEffectInstance>();
            state.RequireForUpdate<CEffectGrantedTags>();
            state.RequireForUpdate<CEffectInUsage>();
        }

        //[BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            foreach (var (_, _, grantedTags, inUsage, ge) in
                     SystemAPI.Query<
                         RefRO<CEffectInstance>,
                         RefRO<WipDeactivateEffect>,
                         RefRO<CEffectGrantedTags>,
                         RefRO<CEffectInUsage>>().WithEntityAccess())
            {

                var tags = grantedTags.ValueRO.tags;
                var targetAsc = inUsage.ValueRO.Target;
                ASCHelper.RestoreDynamicTags(targetAsc, ge, tags);
            }
        }

        [BurstCompile]
        public void OnDestroy(ref SystemState state)
        {

        }
    }
}
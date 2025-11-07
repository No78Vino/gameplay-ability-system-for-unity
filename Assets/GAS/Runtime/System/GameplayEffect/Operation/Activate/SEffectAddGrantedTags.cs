using Unity.Burst;
using Unity.Entities;

namespace GAS.Runtime
{
    [UpdateInGroup(typeof(SGActivateEffect))]
    [UpdateBefore(typeof(SActivateEnd))]
    public partial struct SEffectAddGrantedTags : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<WipActivateEffect>();
            state.RequireForUpdate<CEffectInstance>();
            state.RequireForUpdate<CEffectGrantedTags>();
            state.RequireForUpdate<CEffectInUsage>();
        }

        // [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            foreach (var (_, _, grantedTags, inUsage,ge) in
                     SystemAPI.Query<
                         RefRO<CEffectInstance>,
                         RefRO<WipActivateEffect>,
                         RefRO<CEffectGrantedTags>,
                         RefRO<CEffectInUsage>>().WithEntityAccess())
            {

                var tags = grantedTags.ValueRO.tags;
                var targetAsc = inUsage.ValueRO.Target;
                ASCHelper.TryAddDynamicAddedTags(targetAsc, ge, tags);
            }
        }

        [BurstCompile]
        public void OnDestroy(ref SystemState state)
        {

        }
    }
}
using Unity.Burst;
using Unity.Entities;

namespace GAS.Runtime
{
    [UpdateInGroup(typeof(SGDurationalEffect))]
    public partial struct SPlayCueOnAdd : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<CEffectInstance>();
            state.RequireForUpdate<CCueOnAdd>();
            state.RequireForUpdate<CEffectInUsage>();
            state.RequireForUpdate<WipApplyEffect>();
        }

        //[BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            foreach (var (_, _, cueOnAdd, inUsage, ge) in
                     SystemAPI.Query<
                         RefRO<CEffectInstance>,
                         RefRO<WipApplyEffect>,
                         RefRO<CCueOnAdd>,
                         RefRO<CEffectInUsage>
                     >().WithEntityAccess())
            {

                var cues = cueOnAdd.ValueRO.cues;
                var entityManager = state.EntityManager;
                var targetAsc = inUsage.ValueRO.Target;
                foreach (var cueEntity in cues)
                    CueHelper.TryPlayCueOnAsc(entityManager, targetAsc, cueEntity, ge);
            }
        }

        [BurstCompile]
        public void OnDestroy(ref SystemState state)
        {

        }
    }
}
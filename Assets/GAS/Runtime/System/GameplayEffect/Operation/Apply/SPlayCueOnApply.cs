using Unity.Burst;
using Unity.Entities;

namespace GAS.Runtime
{
    [UpdateInGroup(typeof(SGApplyEffect))]
    [UpdateBefore(typeof(SGInstantEffect))]
    public partial struct SPlayCueOnApply : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<CEffectInstance>();
            state.RequireForUpdate<CCueOnApply>();
            state.RequireForUpdate<CEffectInUsage>();
            state.RequireForUpdate<WipApplyEffect>();
        }

        //[BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            foreach (var (_, _, cueOnApply, inUsage, ge) in
                     SystemAPI.Query<
                         RefRO<CEffectInstance>,
                         RefRO<WipApplyEffect>,
                         RefRO<CCueOnApply>,
                         RefRO<CEffectInUsage>
                     >().WithEntityAccess())
            {

                var cues = cueOnApply.ValueRO.cues;
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
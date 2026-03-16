using Unity.Burst;
using Unity.Entities;

namespace GAS.Runtime
{
    [UpdateInGroup(typeof(SGActivateEffect))]
    [UpdateBefore(typeof(SActivateEnd))]
    public partial struct SPlayCueOnActivate : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<WipActivateEffect>();
            state.RequireForUpdate<CEffectInstance>();
            state.RequireForUpdate<CCueOnActivate>();
            state.RequireForUpdate<CEffectInUsage>();
        }

        //[BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            foreach (var (_, _, cueOnActivate, inUsage, ge) in  
                     SystemAPI.Query<  
                         RefRO<CEffectInstance>,  
                         RefRO<WipActivateEffect>,  
                         RefRO<CCueOnActivate>,  
                         RefRO<CEffectInUsage>>().WithEntityAccess())
            {

                var cues = cueOnActivate.ValueRO.cues;
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
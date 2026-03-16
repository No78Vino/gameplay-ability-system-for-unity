using Unity.Burst;
using Unity.Entities;

namespace GAS.Runtime
{
    [UpdateInGroup(typeof(SGDeactivateEffect))]
    [UpdateBefore( typeof(SDeactivateEnd))]
    public partial struct SPlayCueOnDeactivate : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<WipDeactivateEffect>();
            state.RequireForUpdate<CEffectInstance>();
            state.RequireForUpdate<CCueOnDeactivate>();
            state.RequireForUpdate<CEffectInUsage>();
        }

        //[BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            foreach (var (_, _, cueOnDeactivate, inUsage, ge) in  
                     SystemAPI.Query<  
                         RefRO<CEffectInstance>,  
                         RefRO<WipActivateEffect>,  
                         RefRO<CCueOnDeactivate>,  
                         RefRO<CEffectInUsage>>().WithEntityAccess())
            {

                var cues = cueOnDeactivate.ValueRO.cues;
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
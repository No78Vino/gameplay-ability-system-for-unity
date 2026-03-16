using Unity.Burst;
using Unity.Entities;

namespace GAS.Runtime
{
    [UpdateInGroup(typeof(SGRemoveEffect))]
    [UpdateBefore(typeof(SEffectRemoveEnd))]
    public partial struct SPlayCueOnRemove : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<WipRemoveEffect>();
            state.RequireForUpdate<CEffectInstance>();
            state.RequireForUpdate<CEffectInUsage>();
            state.RequireForUpdate<CDuration>();
            state.RequireForUpdate<CCueOnRemove>();
        }

        //[BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            // 这里播放移除音效/特效等
            foreach (var (_, _, inUsage, cueOnRemove, _, ge) in  
                     SystemAPI.Query<  
                         RefRO<CEffectInstance>,  
                         RefRO<WipRemoveEffect>,  
                         RefRO<CEffectInUsage>,  
                         RefRO<CCueOnRemove>,  
                         RefRO<CDuration>>().WithEntityAccess())
            {
                var cues = cueOnRemove.ValueRO.cues;
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
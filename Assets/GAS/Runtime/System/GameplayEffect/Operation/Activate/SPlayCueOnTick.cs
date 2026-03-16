using Unity.Burst;  
using Unity.Entities;  
  
namespace GAS.Runtime  
{  
    [UpdateInGroup(typeof(SGActivateEffect))]  
    [UpdateBefore(typeof(SActivateEnd))]  
    public partial struct SPlayCueOnTick : ISystem  
    {  
        [BurstCompile]  
        public void OnCreate(ref SystemState state)  
        {  
            state.RequireForUpdate<WipActivateEffect>();  
            state.RequireForUpdate<CEffectInstance>();  
            state.RequireForUpdate<CCueOnTick>();  
            state.RequireForUpdate<CEffectInUsage>();  
        }  
  
        //[BurstCompile]  
        public void OnUpdate(ref SystemState state)  
        {  
            foreach (var (_, _, cueOnTick, inUsage,ge) in  
                     SystemAPI.Query<  
                         RefRO<CEffectInstance>,  
                         RefRO<WipActivateEffect>,  
                         RefRO<CCueOnTick>,  
                         RefRO<CEffectInUsage>>().WithEntityAccess())  
            {  
                var cues = cueOnTick.ValueRO.cues;  
                var entityManager = state.EntityManager;  
                var targetAsc = inUsage.ValueRO.Target;  
                foreach (var cueEntity in cues)  
                    CueHelper.TryPlayCueOnAsc(entityManager, targetAsc, cueEntity,ge);  
            }  
        }  
  
        [BurstCompile]  
        public void OnDestroy(ref SystemState state)  
        {  
        }  
    }  
}
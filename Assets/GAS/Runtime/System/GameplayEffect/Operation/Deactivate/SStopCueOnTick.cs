using Unity.Burst;  
using Unity.Entities;  
  
namespace GAS.Runtime  
{  
    [UpdateInGroup(typeof(SGDeactivateEffect))]  
    [UpdateBefore(typeof(SDeactivateEnd))]  
    public partial struct SStopCueOnTick : ISystem  
    {  
        [BurstCompile]  
        public void OnCreate(ref SystemState state)  
        {  
            state.RequireForUpdate<WipDeactivateEffect>();  
            state.RequireForUpdate<CEffectInstance>();  
            state.RequireForUpdate<CCueOnTick>();  
            state.RequireForUpdate<CEffectInUsage>();  
        }  
  
        //[BurstCompile]  
        public void OnUpdate(ref SystemState state)  
        {  
            foreach (var (_, _, cueOnTick, _) in  
                     SystemAPI.Query<  
                         RefRO<CEffectInstance>,  
                         RefRO<WipDeactivateEffect>,  
                         RefRO<CCueOnTick>,  
                         RefRO<CEffectInUsage>>())  
            {  
                var cues = cueOnTick.ValueRO.cues;  
                var entityManager = state.EntityManager;  
                foreach (var cueEntity in cues)  
                {  
                    // 停止 Cue，禁用 ECCuePlayable 使 SCueTick 不再调用 OnTick()  
                    var cueLogic = entityManager.GetComponentData<MCCue>(cueEntity);  
                    cueLogic.cue.Stop();  
                }  
            }  
        }  
  
        [BurstCompile]  
        public void OnDestroy(ref SystemState state)  
        {  
        }  
    }  
}
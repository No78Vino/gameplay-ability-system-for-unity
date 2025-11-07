using Unity.Burst;
using Unity.Entities;

namespace GAS.Runtime
{
    [UpdateInGroup(typeof(SGActivateEffect))]
    [UpdateBefore(typeof(SActivateEnd))]
    public partial struct SSetEffectActive : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<GlobalTimer>();
            state.RequireForUpdate<WipActivateEffect>();
            state.RequireForUpdate<CEffectInstance>();
            state.RequireForUpdate<CDuration>();
            state.RequireForUpdate<CEffectInUsage>();
        }

        //[BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            var globalTimer = SystemAPI.GetSingleton<GlobalTimer>();
            foreach (var (_, _,inUsage, durationComp, _) in
                     SystemAPI.Query<
                         RefRO<CEffectInstance>,
                         RefRO<WipActivateEffect>,
                         RefRO<CEffectInUsage>,
                         RefRW<CDuration>,
                         RefRO<CEffectInUsage>>())
            {
                // 设置效果为激活状态
                var duration = durationComp.ValueRW;
                duration.active = true;
                duration.activeTime = 
                    duration.timeUnit == TimeUnit.Frame
                    ? globalTimer.Frame
                    : globalTimer.Turn;
                durationComp.ValueRW = duration;
                
                
                var targetAsc = inUsage.ValueRO.Target;
                GASEventCenter.InvokeOnGameplayEffectContainerIsDirty(targetAsc);
            }
        }

        [BurstCompile]
        public void OnDestroy(ref SystemState state)
        {

        }
    }
}
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;

namespace GAS.Runtime
{
    [UpdateInGroup(typeof(SGRunningEffect))]
    [UpdateAfter(typeof(SEffectPeriodTick))]
    public partial struct SEffectStackingTick : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<GlobalTimer>();
            state.RequireForUpdate<CEffectInstance>();
            state.RequireForUpdate<CEffectInUsage>();
            state.RequireForUpdate<CStacking>();
        }

        //[BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            var globalFrameTimer = SystemAPI.GetSingletonRW<GlobalTimer>();
            var currentFrame = globalFrameTimer.ValueRO.Frame;
            var currentTurn = globalFrameTimer.ValueRO.Turn;
            var ecb = new EntityCommandBuffer(Allocator.Temp);
            foreach (var (duration, stacking, _, _, geEntity) in SystemAPI
                         .Query<RefRW<CDuration>, RefRW<CStacking>, RefRO<CEffectInstance>, RefRO<CEffectInUsage>>()
                         .WithEntityAccess())
            {
                // 过滤：
                // 1.持续时间无限的GE
                // 2.未激活的GE
                if (duration.ValueRO.duration <= 0 || !duration.ValueRO.active) continue;

                var durRO = duration.ValueRO;
                var countTime = duration.ValueRO.timeUnit == TimeUnit.Frame ? currentFrame : currentTurn;
                bool expired;
                if (duration.ValueRO.StopTickWhenDeactivated)
                    expired = countTime - durRO.lastActiveTime >= durRO.remianTime;
                else
                    expired = countTime - durRO.activeTime >= durRO.duration;

                if (expired)
                {
                    // 根据Stacking的配置类型，决定过期逻辑
                    if (stacking.ValueRO.EffectExpirationPolicy == EffectExpirationPolicy.ClearEntireStack)
                    {
                        // 清除整个Stack，相当于直接销毁
                        ecb.RemoveComponent<CEffectApplied>(geEntity);
                        ecb.AddComponent<CEffectDestroy>(geEntity);
                    }
                    else if (stacking.ValueRO.EffectExpirationPolicy ==
                             EffectExpirationPolicy.RemoveSingleStackAndRefreshDuration)
                    {
                        // 1.移除一层stack
                        TryChangeStackCount(
                            state.EntityManager,
                            geEntity,
                            stacking.ValueRO,
                            stacking.ValueRO.StackCount - 1,
                            duration,
                            globalFrameTimer.ValueRO);
                        // 2.刷新持续时间
                        RefreshDuration(ref duration.ValueRW, globalFrameTimer.ValueRO);
                    }
                    else if (stacking.ValueRO.EffectExpirationPolicy == EffectExpirationPolicy.RefreshDuration)
                    {
                        // 刷新持续时间
                        RefreshDuration(ref duration.ValueRW, globalFrameTimer.ValueRO);
                    }
                }
            }

            ecb.Playback(state.EntityManager);
            ecb.Dispose();
        }

        [BurstCompile]
        public void OnDestroy(ref SystemState state)
        {

        }

        /// <summary>  
        /// 刷新Duration的激活时间（用于Stacking过期策略的Duration刷新）。  
        /// 无论当前是否已激活，都强制重置 activeTime 为当前时间。  
        /// </summary>  
        private void RefreshDuration(ref CDuration duration, GlobalTimer globalFrameTimer)
        {
            var currentFrame = globalFrameTimer.Frame;
            var currentTurn = globalFrameTimer.Turn;

            duration.active = true;
            if (duration.timeUnit == TimeUnit.Frame)
            {
                duration.activeTime = currentFrame;
                duration.lastActiveTime = currentFrame;
            }
            else
            {
                duration.activeTime = currentTurn;
                duration.lastActiveTime = currentTurn;
            }
        }
        
        private void TryChangeStackCount(EntityManager entityManager, Entity ge, CStacking stacking,
            int stackCount, RefRW<CDuration> duration, GlobalTimer globalFrameTimer)
        {
            var oldStackCount = entityManager.GetComponentData<CStacking>(ge).StackCount;
            int newStackCount = stackCount;

            if (stackCount <= 0)
            {
                // Fix #4: 层数减到0，销毁GE  
                newStackCount = 0;
                entityManager.RemoveComponent<CEffectApplied>(ge);
                entityManager.AddComponent<CEffectDestroy>(ge);
            }
            else if (stackCount <= stacking.LimitCount)
            {
                // Fix #5: 回写 StackCount  
                newStackCount = stackCount;
                stacking.StackCount = newStackCount;
                entityManager.SetComponentData(ge, stacking);

                // Fix #3: 用 RefreshDuration 替代 SActivateEffect.UpdateActiveTime  
                if (stacking.EffectDurationRefreshPolicy == EffectDurationRefreshPolicy.RefreshOnSuccessfulApplication)
                {
                    RefreshDuration(ref duration.ValueRW, globalFrameTimer);
                }

                if (stacking.EffectPeriodResetPolicy == EffectPeriodResetPolicy.ResetOnSuccessfulApplication)
                {
                    if (entityManager.HasComponent<CPeriod>(ge))
                    {
                        var period = entityManager.GetComponentData<CPeriod>(ge);
                        var time = duration.ValueRO.timeUnit == TimeUnit.Frame
                            ? globalFrameTimer.Frame
                            : globalFrameTimer.Turn;
                        period.StartTime = time;
                        entityManager.SetComponentData(ge, period);
                    }
                }
            }
            else
            {
                // 溢出逻辑  
                if (stacking.overflowEffects.Length > 0)
                {
                    var inUsage = entityManager.GetComponentData<CEffectInUsage>(ge);
                    foreach (var overflowEffect in stacking.overflowEffects)
                        GameplayEffectHelper.ApplyGameplayEffectImmediate(overflowEffect, inUsage.Target, inUsage.Source);
                }

                // Fix #7: clearStackOnOverflow 独立于 DurationRefreshPolicy  
                if (stacking.denyOverflowApplication)
                {
                    if (stacking.clearStackOnOverflow)
                    {
                        entityManager.RemoveComponent<CEffectApplied>(ge);
                        entityManager.AddComponent<CEffectDestroy>(ge);
                    }
                }
                else if (stacking.EffectDurationRefreshPolicy ==
                         EffectDurationRefreshPolicy.RefreshOnSuccessfulApplication)
                {
                    RefreshDuration(ref duration.ValueRW, globalFrameTimer);
                }
            }

            GASEventCenter.InvokeOnTryChangeGameplayEffectStackCount(ge, oldStackCount, newStackCount);
            if (oldStackCount != newStackCount)
            {
                var inUsage = entityManager.GetComponentData<CEffectInUsage>(ge);
                GASEventCenter.InvokeOnGameplayEffectContainerIsDirty(inUsage.Target);
            }
        }
    }
}
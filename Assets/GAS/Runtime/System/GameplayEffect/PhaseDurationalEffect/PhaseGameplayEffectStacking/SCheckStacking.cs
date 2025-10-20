using System;
using GAS.Runtime;
using Unity.Burst;
using Unity.Entities;

namespace GAS.Runtime
{
    [UpdateInGroup(typeof(SysGrpApplicationGameplayEffectStacking))]
    public partial struct SCheckStacking : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<GlobalTimer>();
            state.RequireForUpdate<CInApplicationProgress>();
            state.RequireForUpdate<CStacking>();
            state.RequireForUpdate<CDuration>();
        }

        //[BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            // var ecb = new EntityCommandBuffer(Allocator.Temp);
            // var globalFrameTimer = SystemAPI.GetSingletonRW<GlobalTimer>().ValueRO;
            // foreach (var (inUsage, _, stacking,duration, ge) in SystemAPI
            //              .Query<RefRO<CInUsage>, RefRO<CInActivationProgress>, RefRO<CStacking>,RefRW<CDuration>>()
            //              .WithEntityAccess())
            // {
            //     // 判断是否是新添加的GE
            //     var stackingGe = GetStackingGameplayEffect(state.EntityManager, 
            //         inUsage.ValueRO.Target,
            //         inUsage.ValueRO.Source,
            //         stacking.ValueRO);
            //     if (stackingGe != Entity.Null)
            //     {
            //         // 1.叠加的GE不走Activate流程，并且直接销毁
            //         ecb.RemoveComponent<CInApplicationProgress>(ge);
            //         ecb.RemoveComponent<CValidEffect>(ge);
            //         ecb.AddComponent<CEffectDestroy>(ge);
            //
            //         // 2.尝试更新Stack层数，触发OnStackCountChanged事件
            //         // 3.如果层数改变，额外触发OnGameplayEffectContainerIsDirty
            //         TryChangeStackCount(
            //             state.EntityManager, 
            //             stackingGe, stacking.ValueRO,
            //             stacking.ValueRO.StackCount+1,
            //             duration,
            //             globalFrameTimer);
            //     }
            // }
            //
            // ecb.Playback(state.EntityManager);
            // ecb.Dispose();
        }

        [BurstCompile]
        public void OnDestroy(ref SystemState state)
        {
        }

        /// <summary>
        ///     判断是否是新添加的GameplayEffect
        /// </summary>
        private Entity GetStackingGameplayEffect(EntityManager entityManager, Entity owner, Entity source,
            CStacking stacking)
        {
            var effects = entityManager.GetBuffer<BGameplayEffect>(owner);
            for (var i = 0; i < effects.Length; i++)
            {
                var effect = effects[i].GameplayEffect;
                var hasStacking = entityManager.HasComponent<CStacking>(effect);
                if (hasStacking)
                {
                    var s = entityManager.GetComponentData<CStacking>(effect);
                    if (stacking.StackingCode == s.StackingCode)
                    {
                        if (stacking.StackType == EffectStackType.AggregateByTarget)
                            return effect;
                        if (stacking.StackType == EffectStackType.AggregateBySource)
                        {
                            var inUsage = entityManager.GetComponentData<CEffectInUsage>(effect);
                            if (inUsage.Source == source)
                                return effect;
                        }
                    }
                }
            }

            return Entity.Null;
        }

        public static void TryChangeStackCount(EntityManager entityManager, Entity ge,CStacking stacking, int stackCount,RefRW<CDuration> duration,GlobalTimer globalFrameTimer)
        {
            // 获取旧Stacking数据
            var oldStackCount = entityManager.GetComponentData<CStacking>(ge).StackCount;
            int newStackCount = stackCount;
            if (stackCount <= stacking.LimitCount)
            {
                // 更新栈数
                newStackCount = Math.Max(1,stackCount); // 最小层数为1
                // 是否刷新Duration
                if (stacking.EffectDurationRefreshPolicy == EffectDurationRefreshPolicy.RefreshOnSuccessfulApplication)
                {
                    SActivateEffect.UpdateActiveTime(ref duration.ValueRW,globalFrameTimer);
                }
                // 是否重置Period
                if (stacking.EffectPeriodResetPolicy == EffectPeriodResetPolicy.ResetOnSuccessfulApplication)
                {
                    bool hasPeriodTicker = entityManager.HasComponent<CPeriod>(ge);
                    if (hasPeriodTicker)
                    {
                        // 重置Period
                        var period = entityManager.GetComponentData<CPeriod>(ge);
                        var currentFrame = globalFrameTimer.Frame;
                        var currentTurn = globalFrameTimer.Turn;
                        var time = duration.ValueRO.timeUnit == TimeUnit.Frame ? currentFrame : currentTurn;
                        period.StartTime = time;
                        entityManager.SetComponentData(ge,period);
                    }
                }
            }
            else
            {
                // 溢出GE生效
                if (stacking.overflowEffects.Length > 0)
                {
                    var inUsage = entityManager.GetComponentData<CEffectInUsage>(ge);
                    var target = inUsage.Target;
                    var source = inUsage.Source;
                    foreach (var overflowEffect in stacking.overflowEffects)
                        EffectUtil.ApplyGameplayEffectImmediate(overflowEffect, target, source);
                }

                if (stacking.EffectDurationRefreshPolicy == EffectDurationRefreshPolicy.RefreshOnSuccessfulApplication)
                {
                    if (stacking.denyOverflowApplication)
                    {
                        //当DenyOverflowApplication为True是才有效，当Overflow时是否直接删除所有层数
                        if (stacking.clearStackOnOverflow)
                        {
                            // 移除自身
                            entityManager.RemoveComponent<CEffectApplied>(ge);
                            entityManager.AddComponent<CEffectDestroy>(ge);
                        }
                    }
                    else
                    {
                        // 刷新Duration
                        SActivateEffect.UpdateActiveTime(ref duration.ValueRW,globalFrameTimer);
                    }
                }
            }
           
            // StackCount尝试改变，事件
            GASEventCenter.InvokeOnTryChangeGameplayEffectStackCount(ge,oldStackCount, newStackCount);
            
            if (oldStackCount != newStackCount)
            {
                var inUsage = entityManager.GetComponentData<CEffectInUsage>(ge);
                GASEventCenter.InvokeOnGameplayEffectContainerIsDirty(inUsage.Target);
            }
        }
    }
}
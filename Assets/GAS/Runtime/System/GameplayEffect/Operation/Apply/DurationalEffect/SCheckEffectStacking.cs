using System;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;

namespace GAS.Runtime
{
    [UpdateInGroup(typeof(SGDurationalEffect))]
    public partial struct SCheckEffectStacking : ISystem
    {
        private GlobalTimer _globalTimer;

        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<CEffectInstance>();
            state.RequireForUpdate<CEffectInUsage>();
            state.RequireForUpdate<WipApplyEffect>();
            state.RequireForUpdate<CStacking>();
        }

        //[BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            _globalTimer = SystemAPI.GetSingletonRW<GlobalTimer>().ValueRO;
            var ecb = new EntityCommandBuffer(Allocator.Temp);

            foreach (var (_, _, _, inUsage, stacking, ge) in SystemAPI
                         .Query<
                             RefRO<CEffectInstance>,
                             RefRO<WipApplyEffect>,
                             RefRO<CDuration>,
                             RefRO<CEffectInUsage>,
                             RefRO<CStacking>>()
                         .WithEntityAccess())
            {
                // 处理有堆叠组件的GameplayEffect
                var stackGe = stacking.ValueRO.StackType switch
                {
                    EffectStackType.AggregateBySource =>
                        GameplayEffectHelper.GetStackingEffectBySource(stacking.ValueRO.StackingCode,
                            inUsage.ValueRO.Target, inUsage.ValueRO.Source, state.EntityManager),
                    EffectStackType.AggregateByTarget =>
                        GameplayEffectHelper.GetStackingEffectByTarget(stacking.ValueRO.StackingCode,
                            inUsage.ValueRO.Source, state.EntityManager),
                    _ => Entity.Null
                };

                if (stackGe == Entity.Null)
                    AddToAscBuffList(state.EntityManager, ge, inUsage.ValueRO.Target);
                else
                {
                    ecb.RemoveComponent<CEffectInstance>(ge);
                    ecb.AddComponent<CEffectDestroy>(ge);
                }
                
                var operatedEffect = stackGe == Entity.Null ? ge : stackGe;
                TryChangeStackCount(state.EntityManager, operatedEffect, stacking.ValueRO, stacking.ValueRO.StackCount + 1);
            }

            ecb.Playback(state.EntityManager);
            ecb.Dispose();
        }

        [BurstCompile]
        public void OnDestroy(ref SystemState state)
        {
        }

        [BurstCompile]
        private void AddToAscBuffList(EntityManager entityManager, Entity ge, Entity asc)
        {
            var geBuff = entityManager.GetBuffer<BGameplayEffect>(asc);
            var alreadyExist = false;
            foreach (var geElem in geBuff)
                if (geElem.GameplayEffect == ge)
                {
                    alreadyExist = true;
                    break;
                }

            if (!alreadyExist) geBuff.Add(new BGameplayEffect { GameplayEffect = ge });
        }

        private void TryChangeStackCount(EntityManager entityManager, Entity ge, CStacking stacking, int stackCount)
        {
            // 获取旧Stacking数据
            var globalFrameTimer = _globalTimer;
            var oldStackCount = entityManager.GetComponentData<CStacking>(ge).StackCount;
            var newStackCount = stackCount;
            if (stackCount <= stacking.LimitCount)
            {
                // 更新栈数
                newStackCount = Math.Max(1, stackCount); // 最小层数为1
                stacking.StackCount = newStackCount;
                entityManager.SetComponentData(ge, stacking);

                // 是否刷新Duration
                if (stacking.EffectDurationRefreshPolicy == EffectDurationRefreshPolicy.RefreshOnSuccessfulApplication)
                {
                    var duration = entityManager.GetComponentData<CDuration>(ge);
                    duration = UpdateActiveTime(duration, globalFrameTimer);
                    entityManager.SetComponentData(ge, duration);
                }

                // 是否重置Period
                if (stacking.EffectPeriodResetPolicy == EffectPeriodResetPolicy.ResetOnSuccessfulApplication)
                {
                    var hasPeriodTicker = entityManager.HasComponent<CPeriod>(ge);
                    if (hasPeriodTicker)
                    {
                        // 重置Period
                        var period = entityManager.GetComponentData<CPeriod>(ge);
                        var currentFrame = globalFrameTimer.Frame;
                        var currentTurn = globalFrameTimer.Turn;
                        var duration = entityManager.GetComponentData<CDuration>(ge);
                        var time = duration.timeUnit == TimeUnit.Frame ? currentFrame : currentTurn;
                        period.StartTime = time;
                        entityManager.SetComponentData(ge, period);
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
                            EntityHelper.RemoveComponent<CEffectApplied>(ge);
                            EntityHelper.AddComponent<CEffectDestroy>(ge);
                        }
                    }
                    else
                    {
                        // 刷新Duration
                        var duration = entityManager.GetComponentData<CDuration>(ge);
                        duration = UpdateActiveTime(duration, globalFrameTimer);
                        entityManager.SetComponentData(ge, duration);
                    }
                }
            }

            // StackCount尝试改变，事件
            GASEventCenter.InvokeOnTryChangeGameplayEffectStackCount(ge, oldStackCount, newStackCount);

            if (oldStackCount != newStackCount)
            {
                var inUsage = entityManager.GetComponentData<CEffectInUsage>(ge);
                GASEventCenter.InvokeOnGameplayEffectContainerIsDirty(inUsage.Target);
            }
        }

        private CDuration UpdateActiveTime(CDuration duration, GlobalTimer globalFrameTimer)
        {
            var currentFrame = globalFrameTimer.Frame;
            var currentTurn = globalFrameTimer.Turn;
            //  更新激活时间
            if (duration.active) return duration;
            duration.active = true;
            if (duration.timeUnit == TimeUnit.Frame)
            {
                if (duration.activeTime == 0 || duration.ResetStartTimeWhenActivated)
                    duration.activeTime = currentFrame;

                duration.lastActiveTime = currentFrame;
            }
            else
            {
                if (duration.activeTime == 0 || duration.ResetStartTimeWhenActivated)
                    duration.activeTime = currentTurn;

                duration.lastActiveTime = currentTurn;
            }

            return duration;
        }
    }
}
using System;
using GAS.Runtime;
using GAS.RuntimeWithECS.AbilitySystemCell;
using GAS.RuntimeWithECS.AttributeSet.Component;
using GAS.RuntimeWithECS.Common.Component;
using GAS.RuntimeWithECS.Cue.Component;
using GAS.RuntimeWithECS.GameplayEffect;
using GAS.RuntimeWithECS.GameplayEffect.Component;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;

namespace GAS.Runtime
{
    [UpdateInGroup(typeof(SysGroupEffect))]
    public partial struct SApplyGameplayEffect : ISystem
    {
        private GlobalTimer _globalTimer;
        private EntityCommandBuffer _ecb;
        
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<CEffectInUsage>();
        }
        
        //[BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            _globalTimer = SystemAPI.GetSingletonRW<GlobalTimer>().ValueRO;
            var ecb = new EntityCommandBuffer( Allocator.Temp);
            _ecb = ecb;
            foreach (var (inUsage, ge) in SystemAPI.Query<RefRO<CEffectInUsage>>()
                         .WithEntityAccess())
            {
                var target = inUsage.ValueRO.Target;
                var source = inUsage.ValueRO.Source;
                // 1.校验 ApplicationRequiredTags
                if(!CheckCanApply(state.EntityManager,ge,target,ecb))
                    continue;
                
                // 2.校验免疫
                if(!CheckImmunity(state.EntityManager,ge,target,ecb))
                    continue;
                
                // 3.Instant GE应用逻辑
                if(!TryExecuteInstantEffect(state.EntityManager,ge,target,ecb))
                    continue;
                
                // 4.Durational GE逻辑
                CheckDurationAndStacking(state.EntityManager, ge, target, ecb);
            }
            
            ecb.Playback(state.EntityManager);
            ecb.Dispose();
        }

        [BurstCompile]
        public void OnDestroy(ref SystemState state)
        {

        }


        #region 各个阶段的GE处理，之后优化一个一个分离出System

        private bool CheckCanApply(EntityManager entityManager,Entity ge,Entity asc,EntityCommandBuffer ecb)
        {
            bool hasRequiredTags = entityManager.HasComponent<CApplicationRequiredTags>(ge);
            if(!hasRequiredTags) return true;
            
            var requiredTags = entityManager.GetComponentData<CApplicationRequiredTags>(ge);
            bool hasAllTags = ASCUtil.HasAllTags(asc, requiredTags.tags);

            // 不满足则直接销毁GE
            if (!hasAllTags) ecb.AddComponent<CEffectDestroy>(ge);
            
            return hasAllTags;
        }

        private bool CheckImmunity(EntityManager entityManager,Entity ge,Entity asc,EntityCommandBuffer ecb)
        {
            bool hasImmunityTag = entityManager.HasComponent<CEffectImmunityTags>(ge);
            if(!hasImmunityTag) return true;

            var immunityTags = entityManager.GetComponentData<CEffectImmunityTags>(ge);
            bool hasAnyTags = ASCUtil.HasAnyTags(asc,immunityTags.tags);
            
            // 有任意免疫标签，则直接销毁
            if (hasAnyTags) ecb.AddComponent<CEffectDestroy>(ge);

            return !hasAnyTags;
        }
        
        /// <summary>
        /// 执行Instant GE
        /// </summary>
        /// <param name="entityManager"></param>
        /// <param name="ge"></param>
        /// <param name="asc"></param>
        /// <param name="ecb"></param>
        /// <returns></returns>
        private bool TryExecuteInstantEffect(EntityManager entityManager,Entity ge,Entity asc,EntityCommandBuffer ecb)
        {
            if (!entityManager.HasComponent<CDuration>(ge))
            {
                // 1.移除指定Tag的GE
                if (entityManager.HasComponent<CRemoveEffectWithTags>(ge))
                {
                    var removeEffectWithTags = entityManager.GetComponentData<CRemoveEffectWithTags>(ge);
                    // 获取ASC的GE容器
                    var geContainer = entityManager.GetBuffer<BEGameplayEffect>(asc);
                    // 遍历ASC的GE，销毁移除符合条件的GE
                    for (var i = 0; i < geContainer.Length; i++)
                    {
                        var effect = geContainer[i].GameplayEffect;
                        bool geHasAnyTag = GEUtil.HasAnyTags(effect, removeEffectWithTags.tags);
                        if (!geHasAnyTag) continue;
                        // 2.添加到销毁ge集合中
                        ecb.AddComponent<CEffectDestroy>(effect);
                    }
                }
                
                // 2.执行Instant GE的修改器
                if (entityManager.HasComponent<MCModifiers>(ge))
                {
                    bool change = false;
                    var modifiers = entityManager.GetComponentData<MCModifiers>(ge);
                    var attrSets = entityManager.GetBuffer<BEAttributeSet>(asc);
                    foreach (var modifier in modifiers.Modifiers)
                    {
                        var attrSetIndex = attrSets.IndexOfAttrSetCode(modifier.AttrSetCode);
                        if (attrSetIndex == -1) continue;
                        
                        var attrSet = attrSets[attrSetIndex];
                        var attributes = attrSet.Attributes;
                        
                        var attrIndex = attributes.IndexOfAttrCode(modifier.AttrCode);
                        if (attrIndex == -1) continue;
                        
                        var data = attributes[attrIndex];
                        var oldValue = data.BaseValue;
                        var newValue = MmcHelper.Calculate(ge, modifier, data.BaseValue);
                        
                        // OnChangeBefore
                        // BaseValue 不做钳制，因为Max，Min是只针对Current Value
                        newValue = GASEventCenter.InvokeOnBaseValueChangeBefore(asc, modifier.AttrSetCode, modifier.AttrCode,
                            newValue);
                        
                        data.BaseValue = newValue;
                        
                        // OnChangeAfter
                        if (newValue != oldValue)
                        {
                            // BaseValue 改变，需要标记Dirty
                            data.Dirty = true;
                            change = true;
                            GASEventCenter.InvokeOnBaseValueChangeAfter(asc, modifier.AttrSetCode, modifier.AttrCode, oldValue,
                                newValue);
                        }
                        
                        attrSet.Attributes[attrIndex] = data;
                        attrSets[attrSetIndex] = attrSet;
                    }
                    
                    // TODO 触发刷新CurrentValue的事件
                    if (change) ecb.AddComponent<CAttributeIsDirty>(asc);
                }
                
                // TODO
                // 3.执行GE的OnExecute的Cue逻辑
                // TriggerCueOnExecute();
                
                ecb.AddComponent<CEffectDestroy>(ge);
                return false;
            }
            
            return true;
        }
        
        private bool CheckDurationAndStacking(EntityManager entityManager,Entity ge,Entity asc,EntityCommandBuffer ecb)
        {
            if (entityManager.HasComponent<CDuration>(ge))
            {
                var inUsage = entityManager.GetComponentData<CEffectInUsage>(ge);
                if (!entityManager.HasComponent<CStacking>(ge))
                    Operation_AddNewGameplayEffect(ge, inUsage.Source, inUsage.Target, entityManager, ecb);
                else
                {
                    var stacking = entityManager.GetComponentData<CStacking>(ge);
                    var stackGe = stacking.StackType switch
                    {
                        EffectStackType.AggregateBySource => GetStackingEffectBySource(stacking.StackingCode,
                            inUsage.Target, inUsage.Source, entityManager),
                        EffectStackType.AggregateByTarget => GetStackingEffectByTarget(stacking.StackingCode,
                            inUsage.Source, entityManager),
                        _ => Entity.Null
                    };
                    
                    if (stackGe == Entity.Null)
                        Operation_AddNewGameplayEffect(ge, inUsage.Source, inUsage.Target, entityManager, ecb);
                        
                    TryChangeStackCount(entityManager, ge, stacking, stacking.StackCount + 1);
                }

                return true;
            }

            ecb.AddComponent<CEffectDestroy>(ge);
            return false;
        }
        
        private void Operation_AddNewGameplayEffect(Entity gameplayEffect,Entity sourceAsc,Entity targetAsc,EntityManager entityManager,EntityCommandBuffer ecb)
        {
            // TODO 1.Period 组件生效
            //     PeriodExecution = GameplayEffect.PeriodExecution?.CreateSpec(source, owner);
            
            // TODO 2.GrantedAbilities 组件生效
            //     SetGrantedAbility(GameplayEffect.GrantedAbilities);
            
            // 优化：快照将在属性类mmc的modifier生效时才捕捉
            // CaptureAttributesSnapshot();
            
            targetAsc.TryAddGameplayEffect(gameplayEffect);

            TriggerCueOnAdd(gameplayEffect,targetAsc,entityManager,ecb);
            
            if (entityManager.HasComponent<CEffectApplied>(gameplayEffect)) return;
            ecb.AddComponent<CEffectApplied>(gameplayEffect);
            
            if (CheckOngoingRequiredTags(gameplayEffect,targetAsc,entityManager))
            {
                var duration = entityManager.GetComponentData<CDuration>(gameplayEffect);
                if (!duration.active)
                {
                    duration.active = true;
                    duration.activeTime = duration.timeUnit == TimeUnit.Frame
                        ? _globalTimer.Frame
                        : _globalTimer.Turn;
                    entityManager.SetComponentData(gameplayEffect,duration);
                    TriggerOnActivation(gameplayEffect, targetAsc);
                }
            }
            
#if UNITY_EDITOR
            if (!targetAsc.HasGameplayEffect(gameplayEffect))
            {
                UnityEngine.Debug.LogWarning(
                    $"GameplayEffect {gameplayEffect.ToString()} was removed immediately after being applied. " +
                    $"This may indicate a problem with the RemoveGameplayEffectsWithTags.");
            }
#endif
            if (entityManager.HasComponent<MCModifiers>(gameplayEffect))
            {
                // 标记相关属性为Dirty
                var modifiers = entityManager.GetComponentData<MCModifiers>(gameplayEffect);
                var attrSets = entityManager.GetBuffer<BEAttributeSet>(targetAsc);
                bool isAttrDirty = false;
                foreach (var modifier in modifiers.Modifiers)
                {
                    var attrSetIndex = attrSets.IndexOfAttrSetCode(modifier.AttrSetCode);
                    if (attrSetIndex == -1) continue;
                    
                    var attrSet = attrSets[attrSetIndex];
                    var attributes = attrSet.Attributes;
                    
                    var attrIndex = attributes.IndexOfAttrCode(modifier.AttrCode);
                    if (attrIndex == -1) continue;
                    
                    var data = attributes[attrIndex];
                    data.Dirty = true;
                    attributes[attrIndex] = data;
                    attrSet.Attributes = attributes;
                    attrSets[attrSetIndex] = attrSet;
                    
                    isAttrDirty = true;
                }
                if(isAttrDirty) ecb.AddComponent<CAttributeIsDirty>(targetAsc);
            }

            GASEventCenter.InvokeOnGameplayEffectContainerIsDirty(targetAsc);
        }

        private bool CheckOngoingRequiredTags(Entity gameplayEffect,Entity targetAsc,EntityManager entityManager)
        {
            if (!entityManager.HasComponent<COngoingRequiredTags>(gameplayEffect)) return true;
            var ongoingRequiredTags = entityManager.GetComponentData<COngoingRequiredTags>(gameplayEffect);
            return ASCUtil.HasAllTags(targetAsc,ongoingRequiredTags.tags);
        }
        
        // TODO 激活 CueOnAdd
        private void TriggerCueOnAdd(Entity gameplayEffect,Entity targetAsc,EntityManager entityManager,EntityCommandBuffer ecb)
        {
            // TODO
            if (!entityManager.HasComponent<CCueOnAdd>(gameplayEffect)) return;

            var cues = entityManager.GetComponentData<CCueOnAdd>(gameplayEffect).cues;
            foreach (var cueEntity in cues)
            {
                // 1.先判断tag是否可以播放cue
                if (entityManager.HasComponent<CPlayRequiredTags>(cueEntity))
                {
                    var requiredTags = entityManager.GetComponentData<CPlayRequiredTags>(cueEntity);
                    if(!ASCUtil.HasAllTags(targetAsc,requiredTags.tags)) continue;
                }
                if (entityManager.HasComponent<CPlayImmunitedTags>(cueEntity))
                {
                    var immunitedTags = entityManager.GetComponentData<CPlayImmunitedTags>(cueEntity);
                    if(ASCUtil.HasAnyTags(targetAsc,immunitedTags.tags)) continue;
                }
                // 2.重置Cue逻辑单元
                var cueLogic = entityManager.GetComponentData<MCInstantCue>(cueEntity);
                cueLogic.cue.Reset();
                // 3.挂载激活Cue Tag
                ecb.AddComponent<CCuePlaying>(cueEntity);
            }
        }
        
        // TODO 激活 CueOnActivation
        private void TriggerOnActivation(Entity gameplayEffect,Entity targetAsc)
        {
            // TriggerCueOnActivation();
            // Owner.GameplayTagAggregator.ApplyGameplayEffectDynamicTag(this);
            // Owner.GameplayEffectContainer.RemoveGameplayEffectWithAnyTags(GameplayEffect.TagContainer
            //     .RemoveGameplayEffectsWithTags);
            //
            // TryActivateGrantedAbilities();
        }
        
        private Entity GetStackingEffectBySource(int stackingCode,Entity targetAsc, Entity sourceAsc, EntityManager entityManager)
        {
            var effects = entityManager.GetBuffer<BEGameplayEffect>(targetAsc);
         
            for (var i = 0; i < effects.Length; i++)
            {
                var effect = effects[i].GameplayEffect;
 
                var hasStacking = entityManager.HasComponent<CStacking>(effect);
                if (!hasStacking) continue;
                
                var stacking = entityManager.GetComponentData<CStacking>(effect);
                if (stacking.StackType != EffectStackType.AggregateBySource) continue;

                var source = entityManager.GetComponentData<CEffectInUsage>(effect).Source;
                if (source != sourceAsc) continue;
                
                if (stacking.StackingCode == stackingCode)
                    return effect;
            }
            return Entity.Null;
        }
        
        private Entity GetStackingEffectByTarget(int stackingCode,Entity targetAsc,EntityManager entityManager)
        {
            var effects = entityManager.GetBuffer<BEGameplayEffect>(targetAsc);
         
            for (var i = 0; i < effects.Length; i++)
            {
                var effect = effects[i].GameplayEffect;
 
                var hasStacking = entityManager.HasComponent<CStacking>(effect);
                if (!hasStacking) continue;
                
                var stacking = entityManager.GetComponentData<CStacking>(effect);
                if (stacking.StackType != EffectStackType.AggregateByTarget) continue;
                
                if (stacking.StackingCode == stackingCode)
                    return effect;
            }
            return Entity.Null;
        }
        
        private  void TryChangeStackCount(EntityManager entityManager, Entity ge,CStacking stacking, int stackCount)
        {
            // 获取旧Stacking数据
            var globalFrameTimer = _globalTimer;
            var oldStackCount = entityManager.GetComponentData<CStacking>(ge).StackCount;
            int newStackCount = stackCount;
            if (stackCount <= stacking.LimitCount)
            {
                // 更新栈数
                newStackCount = Math.Max(1,stackCount); // 最小层数为1
                stacking.StackCount = newStackCount;
                entityManager.SetComponentData(ge,stacking);
                
                // 是否刷新Duration
                if (stacking.EffectDurationRefreshPolicy == EffectDurationRefreshPolicy.RefreshOnSuccessfulApplication)
                {
                    var duration = entityManager.GetComponentData<CDuration>(ge);
                    duration = UpdateActiveTime(duration,globalFrameTimer);
                    entityManager.SetComponentData(ge,duration);
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
                        var duration = entityManager.GetComponentData<CDuration>(ge);
                        var time = duration.timeUnit == TimeUnit.Frame ? currentFrame : currentTurn;
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
                        GEUtil.ApplyGameplayEffectImmediate(overflowEffect, target, source);
                }

                if (stacking.EffectDurationRefreshPolicy == EffectDurationRefreshPolicy.RefreshOnSuccessfulApplication)
                {
                    if (stacking.denyOverflowApplication)
                    {
                        //当DenyOverflowApplication为True是才有效，当Overflow时是否直接删除所有层数
                        if (stacking.clearStackOnOverflow)
                        {
                            // 移除自身
                            _ecb.RemoveComponent<CEffectApplied>(ge);
                            _ecb.AddComponent<CEffectDestroy>(ge);
                        }
                    }
                    else
                    {
                        // 刷新Duration
                        var duration = entityManager.GetComponentData<CDuration>(ge);
                        duration = UpdateActiveTime(duration,globalFrameTimer);
                        entityManager.SetComponentData(ge,duration);
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
         
        public CDuration UpdateActiveTime(CDuration duration, GlobalTimer globalFrameTimer)
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
        #endregion
    }
}
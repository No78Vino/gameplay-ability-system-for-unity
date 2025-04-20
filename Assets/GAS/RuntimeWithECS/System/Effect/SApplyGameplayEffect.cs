using System;
using GAS.Runtime;
using GAS.RuntimeWithECS.Ability.Component.Dynamic;
using GAS.RuntimeWithECS.AbilitySystemCell;
using GAS.RuntimeWithECS.Attribute.Component;
using GAS.RuntimeWithECS.AttributeSet.Component;
using GAS.RuntimeWithECS.Common.Component;
using GAS.RuntimeWithECS.Core;
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
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<CEffectInUsage>();
        }
        
        //[BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            var ecb = new EntityCommandBuffer( Allocator.Temp);
            var globalTimer = SystemAPI.GetSingletonRW<GlobalTimer>();

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
                
                // 4.Instant GE应用逻辑
                if(!TryExecuteInstantEffect(state.EntityManager,ge,target,ecb))
                    continue;
                
                // 3.Durational GE逻辑
                if(!CheckDurationAndStacking(state.EntityManager,ge,target,ecb))
                    continue;
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
        
        // TODO
        private bool CheckDurationAndStacking(EntityManager entityManager,Entity ge,Entity asc,EntityCommandBuffer ecb)
        {
            if (entityManager.HasComponent<CDuration>(ge))
            {
                if (entityManager.HasComponent<CStacking>(ge))
                {
                    // Operation_AddNewGameplayEffectSpec
                }
                else
                {
                    var stacking = entityManager.GetComponentData<CStacking>(ge);
                    if (stacking.StackType == EffectStackType.AggregateBySource)
                    {
                        //                GetStackingEffectSpecByDataFrom(effectSpec.GameplayEffect,source, out var geSpec);
                        //                if (geSpec == null)
                        //                    return Operation_AddNewGameplayEffectSpec(source, effectSpec,overwriteEffectLevel,effectLevel);
                        //                bool stackCountChange = geSpec.RefreshStack();
                        //                if (stackCountChange) OnRefreshStackCountMakeContainerDirty();
                    }else if (stacking.StackType == EffectStackType.AggregateByTarget)
                    {
                        //                GetStackingEffectSpecByData(effectSpec.GameplayEffect, out var geSpec);
                        //                // 新添加GE
                        //                if (geSpec == null)
                        //                    return Operation_AddNewGameplayEffectSpec(source, effectSpec,overwriteEffectLevel,effectLevel);
                        //                bool stackCountChange = geSpec.RefreshStack();
                        //                if (stackCountChange) OnRefreshStackCountMakeContainerDirty();
                    }
                }
                
            }
            ecb.AddComponent<CEffectDestroy>(ge);
            return false;
        }
        
        private void Operation_AddNewGameplayEffectSpec(Entity gameplayEffect,Entity sourceAsc,Entity targetAsc,EntityManager entityManager,EntityCommandBuffer ecb)
        {
            // TODO 1.Period 组件生效
            //     PeriodExecution = GameplayEffect.PeriodExecution?.CreateSpec(source, owner);
            
            // TODO 2.GrantedAbilities 组件生效
            //     SetGrantedAbility(GameplayEffect.GrantedAbilities);
            
            // 优化：快照将在属性类mmc的modifier生效时才捕捉
            // CaptureAttributesSnapshot();
            
            targetAsc.TryAddGameplayEffect(gameplayEffect);

            TriggerCueOnAdd(gameplayEffect,targetAsc);
            
            if (entityManager.HasComponent<CEffectApplied>(gameplayEffect)) return;
            ecb.AddComponent<CEffectApplied>(gameplayEffect);
            
            if (CheckOngoingRequiredTags(gameplayEffect,targetAsc,entityManager))
            {
                var duration = entityManager.GetComponentData<CDuration>(gameplayEffect);
                if (!duration.active)
                {
                    duration.active = true;
                    var globalTimer = SystemAPI.GetSingletonRW<GlobalTimer>();
                    duration.activeTime = duration.timeUnit == TimeUnit.Frame
                        ? globalTimer.ValueRO.Frame
                        : globalTimer.ValueRO.Turn;
                    TriggerOnActivation(gameplayEffect, targetAsc);
                }
            }

            // If the gameplay effect was removed immediately after being applied, return false
            if (!targetAsc.HasGameplayEffect(gameplayEffect))
            {
#if UNITY_EDITOR
                UnityEngine.Debug.LogWarning(
                    $"GameplayEffect {gameplayEffect.ToString()} was removed immediately after being applied. This may indicate a problem with the RemoveGameplayEffectsWithTags.");
#endif
                // No need to trigger OnGameplayEffectContainerIsDirty, it has already been triggered when it was removed.
            }
            
            GASEventCenter.InvokeOnGameplayEffectContainerIsDirty(targetAsc);
            //OnGameplayEffectContainerIsDirty?.Invoke();
        }

        private bool CheckOngoingRequiredTags(Entity gameplayEffect,Entity targetAsc,EntityManager entityManager)
        {
            if (!entityManager.HasComponent<COngoingRequiredTags>(gameplayEffect)) return true;
            var ongoingRequiredTags = entityManager.GetComponentData<COngoingRequiredTags>(gameplayEffect);
            return ASCUtil.HasAllTags(targetAsc,ongoingRequiredTags.tags);
        }
        
        // TODO 激活 CueOnAdd
        private void TriggerCueOnAdd(Entity gameplayEffect,Entity targetAsc)
        {
            // TODO
        }
        
        // TODO 激活 CueOnAdd
        private void TriggerOnActivation(Entity gameplayEffect,Entity targetAsc)
        {
            // TriggerCueOnActivation();
            // Owner.GameplayTagAggregator.ApplyGameplayEffectDynamicTag(this);
            // Owner.GameplayEffectContainer.RemoveGameplayEffectWithAnyTags(GameplayEffect.TagContainer
            //     .RemoveGameplayEffectsWithTags);
            //
            // TryActivateGrantedAbilities();
        }
        
        #endregion
    }
}
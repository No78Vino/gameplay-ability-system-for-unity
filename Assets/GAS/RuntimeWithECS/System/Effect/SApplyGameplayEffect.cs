using System;
using GAS.Runtime;
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
            state.RequireForUpdate<CInUsage>();
        }

        #region MyRegion
        
    
 //            var level = overwriteEffectLevel ? effectLevel : source.Level;
 //            if (effectSpec.DurationPolicy == EffectsDurationPolicy.Instant)
 //            {
 //                effectSpec.Init(source, _owner, level);
 //                effectSpec.TriggerOnExecute();
 //                return null;
 //            }
 //
 //            // Check GE Stacking
 //            if (effectSpec.Stacking.stackingType == StackingType.None)
 //            {
 //                return Operation_AddNewGameplayEffectSpec(source, effectSpec,overwriteEffectLevel,effectLevel);
 //            }
 //            
 //            // 处理GE堆叠
 //            // 基于Target类型GE堆叠
 //            if (effectSpec.Stacking.stackingType == StackingType.AggregateByTarget)
 //            {
 //                GetStackingEffectSpecByData(effectSpec.GameplayEffect, out var geSpec);
 //                // 新添加GE
 //                if (geSpec == null)
 //                    return Operation_AddNewGameplayEffectSpec(source, effectSpec,overwriteEffectLevel,effectLevel);
 //                bool stackCountChange = geSpec.RefreshStack();
 //                if (stackCountChange) OnRefreshStackCountMakeContainerDirty();
 //                return geSpec;
 //            }
 //            
 //            // 基于Source类型GE堆叠
 //            if (effectSpec.Stacking.stackingType == StackingType.AggregateBySource)
 //            {
 //                GetStackingEffectSpecByDataFrom(effectSpec.GameplayEffect,source, out var geSpec);
 //                if (geSpec == null)
 //                    return Operation_AddNewGameplayEffectSpec(source, effectSpec,overwriteEffectLevel,effectLevel);
 //                bool stackCountChange = geSpec.RefreshStack();
 //                if (stackCountChange) OnRefreshStackCountMakeContainerDirty();
 //                return geSpec;
 //            }
 //
 //            return null;

        #endregion
        //[BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            var ecb = new EntityCommandBuffer( Allocator.Temp);
            var globalTimer = SystemAPI.GetSingletonRW<GlobalTimer>();

            foreach (var (inUsage, ge) in SystemAPI.Query<RefRO<CInUsage>>()
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
                if(!CheckDuration(state.EntityManager,ge,target,ecb))
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
        private bool CheckDuration(EntityManager entityManager,Entity ge,Entity asc,EntityCommandBuffer ecb)
        {
            if (entityManager.HasComponent<CDuration>(ge))
            {
                // Operation_AddNewGameplayEffectSpec
            }
            ecb.AddComponent<CEffectDestroy>(ge);
            return false;
        }

        #endregion
    }
}
using GAS.Runtime;
using GAS.RuntimeWithECS.AbilitySystemCell;
using GAS.RuntimeWithECS.Common.Component;
using GAS.RuntimeWithECS.Core;
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
                
                // 3.校验是否是Durational GE
                if(!CheckDuration(state.EntityManager,ge,target,ecb))
                    continue;
            }
            
            ecb.Playback(state.EntityManager);
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
        
        // TODO
        private bool CheckDuration(EntityManager entityManager,Entity ge,Entity asc,EntityCommandBuffer ecb)
        {
            return true;
            // bool hasImmunityTag = entityManager.HasComponent<CEffectImmunityTags>(ge);
            // if(!hasImmunityTag) return true;
            //
            // var immunityTags = entityManager.GetComponentData<CEffectImmunityTags>(ge);
            // bool hasAnyTags = ASCUtil.HasAnyTags(asc,immunityTags.tags);
            //
            // // 有任意免疫标签，则直接销毁
            // if (hasAnyTags) ecb.AddComponent<CEffectDestroy>(ge);
            //
            // return !hasAnyTags;
        }

        #endregion
    }
}
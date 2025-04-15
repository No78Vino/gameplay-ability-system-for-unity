using GAS.Runtime;
using Unity.Burst;
using Unity.Entities;

namespace GAS.Runtime
{
    [UpdateInGroup(typeof(SysGroupEffect))]
    public partial struct SApplyGameplayEffect : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
        }

        #region MyRegion

        // if (!effectSpec.GameplayEffect.CanApplyTo(_owner)) return null;
 //            
 //            if (effectSpec.GameplayEffect.IsImmune(_owner))
 //            {
 //                // TODO 免疫Cue触发
 //                // var lv = overwriteEffectLevel ? effectLevel : source.Level;
 //                // effectSpec.Init(source, _owner, lv);
 //                // effectSpec.TriggerOnImmunity();
 //                return null;
 //            }
 //            
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
        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            
        }

        [BurstCompile]
        public void OnDestroy(ref SystemState state)
        {

        }
    }
}
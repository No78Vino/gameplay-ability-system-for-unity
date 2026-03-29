﻿using Unity.Burst;
using Unity.Collections;
using Unity.Entities;

namespace GAS.Runtime
{
    [UpdateInGroup(typeof(SGCheckApplyEffect))]
    [UpdateBefore(typeof(SCheckApplyEnd))]
    public partial struct SCheckApplicationRequiredTags : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<SingletonGameplayTagMap>();
            state.RequireForUpdate<CEffectInstance>();
            state.RequireForUpdate(SystemAPI.QueryBuilder().WithAny<CApplicationTagRequirement, CApplicationRequiredTags>().Build());
            state.RequireForUpdate<CEffectInUsage>();
            state.RequireForUpdate<WipCheckApplyEffect>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            var ecb = new EntityCommandBuffer(Allocator.Temp);
            var tagMap = SystemAPI.GetSingleton<SingletonGameplayTagMap>();

            foreach (var (_, _, inUsage, ge) in
                     SystemAPI.Query<
                         RefRO<CEffectInstance>,
                         RefRO<WipCheckApplyEffect>,
                         RefRO<CEffectInUsage>
                     >().WithAny<CApplicationTagRequirement, CApplicationRequiredTags>().WithEntityAccess())
            {
                bool hasRequirement = state.EntityManager.HasComponent<CApplicationTagRequirement>(ge);
                // 兼容旧版本，旧版本使用 CApplicationRequiredTags 来指定应用条件
                bool hasLegacyRequired = state.EntityManager.HasComponent<CApplicationRequiredTags>(ge);
                if (!hasRequirement && !hasLegacyRequired)
                    continue;

                var asc = inUsage.ValueRO.Target;
                TagRequirementData query;
                if (hasRequirement)
                {
                    query = state.EntityManager.GetComponentData<CApplicationTagRequirement>(ge).query;
                }
                else
                {
                    var required = state.EntityManager.GetComponentData<CApplicationRequiredTags>(ge).tags;
                    query = new TagRequirementData { all = required, any = default, none = default };
                }

                if (tagMap.AscEvaluateTagRequirement(state.EntityManager, asc, query)) continue;
                ecb.RemoveComponent<CEffectInstance>(ge);
                ecb.AddComponent<CEffectDestroy>(ge);
            }

            ecb.Playback(state.EntityManager);
            ecb.Dispose();
        }

        [BurstCompile]
        public void OnDestroy(ref SystemState state)
        {

        }
    }
}
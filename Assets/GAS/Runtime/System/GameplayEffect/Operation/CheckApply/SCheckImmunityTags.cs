using Unity.Burst;
using Unity.Collections;
using Unity.Entities;

namespace GAS.Runtime
{
    [UpdateInGroup(typeof(SGCheckApplyEffect))]
    [UpdateBefore(typeof(SCheckApplyEnd))]
    public partial struct SCheckImmunityTags : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<SingletonGameplayTagMap>();
            state.RequireForUpdate<CEffectInstance>();
            state.RequireForUpdate(SystemAPI.QueryBuilder().WithAny<CEffectImmunityTagRequirement, CEffectImmunityTags>().Build());
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
                     >().WithAny<CEffectImmunityTagRequirement, CEffectImmunityTags>().WithEntityAccess())
            {
                bool hasRequirement = state.EntityManager.HasComponent<CEffectImmunityTagRequirement>(ge);
                // 兼容旧版本，旧版本使用 CImmunityTags 来指定免疫条件
                bool hasLegacyImmunity = state.EntityManager.HasComponent<CEffectImmunityTags>(ge);
                if (!hasRequirement && !hasLegacyImmunity)
                    continue;

                var asc = inUsage.ValueRO.Target;
                TagRequirementData requirement;
                if (hasRequirement)
                {
                    requirement = state.EntityManager.GetComponentData<CEffectImmunityTagRequirement>(ge).requirement;
                }
                else
                {
                    var any = state.EntityManager.GetComponentData<CEffectImmunityTags>(ge);
                    requirement = new TagRequirementData { all = default, any = any.tags, none = default };
                }

                if(!tagMap.AscEvaluateTagRequirement(state.EntityManager, asc, requirement)) continue;

                ecb.RemoveComponent<CEffectInstance>(ge);
                ecb.AddComponent<CEffectDestroy>(ge);
                // TODO 触发免疫Cue

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
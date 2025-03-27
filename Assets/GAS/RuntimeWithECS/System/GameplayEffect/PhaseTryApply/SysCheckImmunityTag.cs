using GAS.RuntimeWithECS.Common.Component;
using GAS.RuntimeWithECS.GameplayEffect.Component;
using GAS.RuntimeWithECS.System.SystemGroup;
using GAS.RuntimeWithECS.Tag.Component;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;

namespace GAS.RuntimeWithECS.System.GameplayEffect
{
    [UpdateInGroup(typeof(SysGroupTryApplyEffect))]
    [UpdateAfter(typeof(SysCheckApplicationCondition))]
    public partial struct SysCheckImmunityTag : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<SingletonGameplayTagMap>();
            state.RequireForUpdate<CEffectImmunityTags>();
            state.RequireForUpdate<CInUsage>();
            state.RequireForUpdate<CValidEffect>();
        }

        //[BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            var tagMap = SystemAPI.GetSingleton<SingletonGameplayTagMap>();

            var ecb = new EntityCommandBuffer(Allocator.Temp);
            foreach (var (immunityTags, comInUsage, _,ge) in SystemAPI
                         .Query<RefRO<CEffectImmunityTags>, RefRW<CInUsage>,RefRO<CValidEffect>>().WithEntityAccess())
            {
                var asc = comInUsage.ValueRO.Target;
                var fixedTags = SystemAPI.GetBuffer<BFixedTag>(asc);
                var tempTags = SystemAPI.GetBuffer<BTemporaryTag>(asc);

                foreach (var tag in immunityTags.ValueRO.tags)
                {
                    var hasTag = false;
                    // 遍历固有Tag
                    foreach (var fixedTag in fixedTags)
                        if (tagMap.IsTagAIncludeTagB(fixedTag.tag, tag))
                        {
                            hasTag = true;
                            break;
                        }

                    // 遍历临时Tag
                    if (!hasTag)
                        foreach (var tempTag in tempTags)
                            if (tagMap.IsTagAIncludeTagB(tempTag.tag, tag))
                            {
                                hasTag = true;
                                break;
                            }

                    if (hasTag)
                    {
                        ecb.RemoveComponent<CValidEffect>(ge);
                        ecb.AddComponent<CDestroy>(ge);
                        
                        // TODO 触发免疫 Cue
                        
                        break;
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
    }
}
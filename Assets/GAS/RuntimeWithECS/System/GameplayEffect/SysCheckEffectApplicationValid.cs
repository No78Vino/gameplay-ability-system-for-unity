﻿using GAS.RuntimeWithECS.GameplayEffect.Component;
using GAS.RuntimeWithECS.Tag;
using GAS.RuntimeWithECS.Tag.Component;
using Unity.Burst;
using Unity.Entities;

namespace GAS.RuntimeWithECS.System.GameplayEffect
{
    [UpdateAfter(typeof(SysApplyEffect))]
    public partial struct SysCheckEffectApplicationValid : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<SingletonGameplayTagMap>();
            state.RequireForUpdate<ComApplicationRequiredTags>();
            state.RequireForUpdate<ComInUsage>();
            state.RequireForUpdate<ComBasicInfo>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            var tagMap = SystemAPI.GetSingleton<SingletonGameplayTagMap>();
            
            foreach (var (vBasicInfo, requiredTags, comInUsage) in SystemAPI
                         .Query<RefRW<ComBasicInfo>, RefRO<ComApplicationRequiredTags>, RefRW<ComInUsage>>())
            {
                var asc = comInUsage.ValueRO.Target;
                var fixedTags = SystemAPI.GetBuffer<BuffElemFixedTag>(asc);
                var tempTags = SystemAPI.GetBuffer<BuffElemTemporaryTag>(asc);

                comInUsage.ValueRW.Valid = true;
                foreach (var tag in requiredTags.ValueRO.tags)
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

                    if (!hasTag)
                    {
                        comInUsage.ValueRW.Valid = false;
                        break;
                    }
                    
                }
            }
        }

        [BurstCompile]
        public void OnDestroy(ref SystemState state)
        {
        }
    }
}
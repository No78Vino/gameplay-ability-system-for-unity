using GAS.RuntimeWithECS.AbilitySystemCell.Component;
using GAS.RuntimeWithECS.GameplayEffect.Component;
using GAS.RuntimeWithECS.Tag.Component;
using Unity.Burst;
using Unity.Entities;

namespace GAS.RuntimeWithECS.System.GameplayEffect
{
    [UpdateAfter(typeof(SysCheckEffectApplicationValid))]
    [UpdateBefore(typeof(SysRemoveInvalidEffect))]
    public partial struct SysCheckEffectOngoingActive : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<SingletonGameplayTagMap>();
            state.RequireForUpdate<GameplayEffectBufferElement>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            var tagMap = SystemAPI.GetSingleton<SingletonGameplayTagMap>();
            foreach (var (gameplayEffectBuffs, _,asc) in SystemAPI
                         .Query<DynamicBuffer<GameplayEffectBufferElement>, RefRO<NeedCheckEffects>>().WithEntityAccess())
            {
                bool needCheckEffects = SystemAPI.IsComponentEnabled<NeedCheckEffects>(asc);
                // 过滤掉不需要检测GE的ASC实例
                if(!needCheckEffects) continue;
                
                var fixedTags = SystemAPI.GetBuffer<BuffElemFixedTag>(asc);
                var tempTags = SystemAPI.GetBuffer<BuffElemTemporaryTag>(asc);
                
                foreach (var geElement in gameplayEffectBuffs)
                {
                    var geEntity = geElement.GameplayEffect;
                    var ongoingRequiredTags = SystemAPI.GetComponentRO<ComOngoingRequiredTags>(geEntity);
                    var duration = SystemAPI.GetComponentRW<ComDuration>(geEntity);
                    
                    duration.ValueRW.active = true;
                    foreach (var tag in ongoingRequiredTags.ValueRO.tags)
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
                            duration.ValueRW.active = false;
                            break;
                        }
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
using GAS.RuntimeWithECS.AbilitySystemCell.Component;
using GAS.RuntimeWithECS.GameplayEffect.Component;
using GAS.RuntimeWithECS.Tag.Component;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;

namespace GAS.RuntimeWithECS.System.GameplayEffect
{
    [UpdateAfter(typeof(SysCheckEffectApplicationValid))]
    public partial struct SysCheckEffectOngoingActive : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<SingletonGameplayTagMap>();
            state.RequireForUpdate<GameplayEffectBufferElement>();
        }

        //[BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            var ecb = new EntityCommandBuffer(Allocator.Temp);

            var tagMap = SystemAPI.GetSingleton<SingletonGameplayTagMap>();
            foreach (var (gameplayEffectBuffs, _, asc) in SystemAPI
                         .Query<DynamicBuffer<GameplayEffectBufferElement>, RefRO<NeedCheckEffects>>()
                         .WithEntityAccess())
            {
                var needCheckEffects = SystemAPI.IsComponentEnabled<NeedCheckEffects>(asc);
                // 过滤掉不需要检测GE的ASC实例
                if (!needCheckEffects) continue;

                var fixedTags = SystemAPI.GetBuffer<BuffElemFixedTag>(asc);
                var tempTags = SystemAPI.GetBuffer<BuffElemTemporaryTag>(asc);

                foreach (var geElement in gameplayEffectBuffs)
                {
                    var geEntity = geElement.GameplayEffect;

                    // 已经不合法的GE不需要校验 是否可激活
                    var valid = SystemAPI.IsComponentEnabled<ComInUsage>(geEntity);
                    if (!valid) continue;
                    
                    var duration = SystemAPI.GetComponentRW<ComDuration>(geEntity);
                    var oldActive = duration.ValueRO.active;
                    
                    var newActive = true;
                    if (SystemAPI.HasComponent<ComOngoingRequiredTags>(geEntity))
                    {
                        var ongoingRequiredTags = SystemAPI.GetComponentRO<ComOngoingRequiredTags>(geEntity);
                        newActive =
                            IsOngoingRequiredTagsValid(ongoingRequiredTags.ValueRO.tags, fixedTags, tempTags, tagMap);
                    }

                    // 激活状态发生变化
                    if (oldActive != newActive)
                    {
                        duration.ValueRW.active = newActive;

                        // 挂在需要激活/失活的标签组件
                        if (newActive)
                            ecb.AddComponent<ComNeedActivate>(geEntity);
                        else
                            ecb.AddComponent<ComNeedDeactivate>(geEntity);
                    }
                }
            }

            ecb.Playback(state.EntityManager);
        }

        [BurstCompile]
        public void OnDestroy(ref SystemState state)
        {
        }

        private bool IsOngoingRequiredTagsValid(NativeArray<int> ongoingRequiredTags,
            DynamicBuffer<BuffElemFixedTag> fixedTags,
            DynamicBuffer<BuffElemTemporaryTag> tempTags,
            SingletonGameplayTagMap tagMap)
        {
            foreach (var tag in ongoingRequiredTags)
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

                if (!hasTag) return false;
            }

            return true;
        }
    }
}
using GAS.Runtime;
using Unity.Burst;
using Unity.Entities;

namespace GAS.RuntimeWithECS.System.GameplayEffect
{
    [UpdateInGroup(typeof(SysGroupTryApplyEffect))]
    [UpdateAfter(typeof(SCheckApplicationCondition))]
    public partial struct SCheckApplicationRequiredTag : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<SingletonGameplayTagMap>();
            state.RequireForUpdate<CApplicationRequiredTags>();
            state.RequireForUpdate<CEffectInUsage>();
            state.RequireForUpdate<CEffectApplied>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            // var tagMap = SystemAPI.GetSingleton<SingletonGameplayTagMap>();
            //
            // var ecb = new EntityCommandBuffer(Allocator.Temp);
            // foreach (var (requiredTags, comInUsage, _,ge) in SystemAPI
            //              .Query<RefRO<CApplicationRequiredTags>, RefRW<CInUsage>,RefRO<CValidEffect>>().WithEntityAccess())
            // {
            //     var asc = comInUsage.ValueRO.Target;
            //     var fixedTags = SystemAPI.GetBuffer<BFixedTag>(asc);
            //     var tempTags = SystemAPI.GetBuffer<BTemporaryTag>(asc);
            //
            //     foreach (var tag in requiredTags.ValueRO.tags)
            //     {
            //         var hasTag = false;
            //         // 遍历固有Tag
            //         foreach (var fixedTag in fixedTags)
            //             if (tagMap.IsTagAIncludeTagB(fixedTag.tag, tag))
            //             {
            //                 hasTag = true;
            //                 break;
            //             }
            //
            //         // 遍历临时Tag
            //         if (!hasTag)
            //             foreach (var tempTag in tempTags)
            //                 if (tagMap.IsTagAIncludeTagB(tempTag.tag, tag))
            //                 {
            //                     hasTag = true;
            //                     break;
            //                 }
            //
            //         if (!hasTag)
            //         {
            //             ecb.RemoveComponent<CValidEffect>(ge);
            //             ecb.AddComponent<CEffectDestroy>(ge);
            //             break;
            //         }
            //
            //     }
            // }
            // ecb.Playback(state.EntityManager);
            // ecb.Dispose();
        }

        [BurstCompile]
        public void OnDestroy(ref SystemState state)
        {
        }
    }
}
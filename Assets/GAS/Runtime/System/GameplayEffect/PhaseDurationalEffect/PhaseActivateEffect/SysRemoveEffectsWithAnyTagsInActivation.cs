using GAS.Runtime;
using Unity.Burst;
using Unity.Entities;

namespace GAS.Runtime
{
    [UpdateInGroup(typeof(SysGroupActivateEffect))]
    [UpdateBefore(typeof(SActivateEnd))]
    public partial struct SysRemoveEffectsWithAnyTagsInActivation : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<SingletonGameplayTagMap>();
            state.RequireForUpdate<CInActivationProgress>();
            state.RequireForUpdate<CRemoveEffectWithTags>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            // var ecb = new EntityCommandBuffer(Allocator.Temp);
            // var tagMap = SystemAPI.GetSingleton<SingletonGameplayTagMap>();
            // foreach (var (inUsage, _, removeEffectWithTags, ge) in SystemAPI
            //              .Query<RefRO<CInUsage>, RefRO<CInActivationProgress>, RefRO<CRemoveEffectWithTags>>()
            //              .WithEntityAccess())
            // {
            //     var owner = inUsage.ValueRO.Target;
            //     var tags = removeEffectWithTags.ValueRO.tags;
            //     var effects = state.EntityManager.GetBuffer<BEGameplayEffect>(owner);
            //     for (var i = effects.Length - 1; i >= 0; i--)
            //     {
            //         var effect = effects[i].GameplayEffect;
            //         if (effect.CheckEffectHasAnyTags(tagMap, state.EntityManager, tags))
            //         {
            //             ecb.RemoveComponent<CValidEffect>(effect);
            //             ecb.AddComponent<CEffectDestroy>(effect);
            //             effects.RemoveAt(i);
            //         }
            //     }
            // }
        }

        [BurstCompile]
        public void OnDestroy(ref SystemState state)
        {
        }
    }
}
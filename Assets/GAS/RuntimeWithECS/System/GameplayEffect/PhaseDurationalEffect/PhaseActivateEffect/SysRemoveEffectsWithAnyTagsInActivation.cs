using GAS.RuntimeWithECS.Common.Component;
using GAS.RuntimeWithECS.GameplayEffect;
using GAS.RuntimeWithECS.GameplayEffect.Component;
using GAS.RuntimeWithECS.System.SystemGroup;
using GAS.RuntimeWithECS.Tag.Component;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;

namespace GAS.RuntimeWithECS.System.GameplayEffect.PhaseDurationalEffect
{
    [UpdateInGroup(typeof(SysGroupActivateEffect))]
    [UpdateBefore(typeof(SysActivateEnd))]
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
            var ecb = new EntityCommandBuffer(Allocator.Temp);
            var tagMap = SystemAPI.GetSingleton<SingletonGameplayTagMap>();
            foreach (var (inUsage, _, removeEffectWithTags, ge) in SystemAPI
                         .Query<RefRO<CInUsage>, RefRO<CInActivationProgress>, RefRO<CRemoveEffectWithTags>>()
                         .WithEntityAccess())
            {
                var owner = inUsage.ValueRO.Target;
                var tags = removeEffectWithTags.ValueRO.tags;
                var effects = state.EntityManager.GetBuffer<BEGameplayEffect>(owner);
                for (var i = effects.Length - 1; i >= 0; i--)
                {
                    var effect = effects[i].GameplayEffect;
                    if (effect.CheckEffectHasAnyTags(tagMap, state.EntityManager, tags))
                    {
                        ecb.RemoveComponent<CValidEffect>(effect);
                        ecb.AddComponent<CDestroy>(effect);
                        effects.RemoveAt(i);
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
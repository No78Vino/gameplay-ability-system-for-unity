using Unity.Collections;
using Unity.Entities;
using NotImplementedException = System.NotImplementedException;

namespace GAS.RuntimeWithECS.GameplayEffect.Component
{
    public struct COngoingRequiredTags : IComponentData
    {
        public NativeArray<int> tags;
    }

    public sealed class ConfOngoingRequiredTags : GameplayEffectComponentConfig
    {
        public int[] tags;

        public override void LoadToGameplayEffectEntity(Entity ge)
        {
            _entityManager.AddComponentData(ge, new COngoingRequiredTags
            {
                tags = new NativeArray<int>(tags, Allocator.Persistent)
            });
        }

        public override void LoadToGameplayEffectEntity(Entity ge, EntityCommandBuffer ecb)
        {
            ecb.AddComponent(ge, new COngoingRequiredTags
            {
                tags = new NativeArray<int>(tags, Allocator.Persistent)
            });
        }
    }
}
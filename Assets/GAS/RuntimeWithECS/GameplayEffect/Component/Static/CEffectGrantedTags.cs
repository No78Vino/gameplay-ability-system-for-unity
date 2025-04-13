using Unity.Collections;
using Unity.Entities;
using NotImplementedException = System.NotImplementedException;

namespace GAS.RuntimeWithECS.GameplayEffect.Component
{
    public struct CEffectGrantedTags : IComponentData
    {
        public NativeArray<int> tags;
    }

    public sealed class ConfEffectGrantedTags : GameplayEffectComponentConfig
    {
        public int[] tags;

        public override void LoadToGameplayEffectEntity(Entity ge)
        {
            _entityManager.AddComponentData(ge, new CEffectGrantedTags
            {
                tags = new NativeArray<int>(tags, Allocator.Persistent)
            });
        }

        public override void LoadToGameplayEffectEntity(Entity ge, EntityCommandBuffer ecb)
        {
            ecb.AddComponent(ge, new CEffectGrantedTags
            {
                tags = new NativeArray<int>(tags, Allocator.Persistent)
            });
        }
    }
}
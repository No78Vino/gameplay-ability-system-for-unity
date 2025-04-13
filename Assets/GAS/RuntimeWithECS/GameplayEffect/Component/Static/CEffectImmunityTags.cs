using Unity.Collections;
using Unity.Entities;
using NotImplementedException = System.NotImplementedException;

namespace GAS.RuntimeWithECS.GameplayEffect.Component
{
    public struct CEffectImmunityTags : IComponentData
    {
        public NativeArray<int> tags;
    }
    
    public sealed class ConfEffectImmunityTags:GameplayEffectComponentConfig
    {
        public int[] tags;
        
        public override void LoadToGameplayEffectEntity(Entity ge)
        {
            _entityManager.AddComponentData(ge, new CEffectImmunityTags
            {
                tags = new NativeArray<int>(tags, Allocator.Persistent)
            });
        }

        public override void LoadToGameplayEffectEntity(Entity ge, EntityCommandBuffer ecb)
        {
            ecb.AddComponent(ge, new CEffectImmunityTags
            {
                tags = new NativeArray<int>(tags, Allocator.Persistent)
            });
        }
    }
}
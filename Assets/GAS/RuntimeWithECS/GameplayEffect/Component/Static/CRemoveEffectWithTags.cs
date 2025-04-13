using Unity.Collections;
using Unity.Entities;
using NotImplementedException = System.NotImplementedException;

namespace GAS.RuntimeWithECS.GameplayEffect.Component
{
    public struct CRemoveEffectWithTags : IComponentData
    {
        public NativeArray<int> tags;
    }
    
    public sealed class ConfRemoveEffectWithTags:GameplayEffectComponentConfig
    {
        public int[] tags;
        
        public override void LoadToGameplayEffectEntity(Entity ge)
        {
            _entityManager.AddComponentData(ge, new CRemoveEffectWithTags
            {
                tags = new NativeArray<int>(tags, Allocator.Persistent)
            });
        }

        public override void LoadToGameplayEffectEntity(Entity ge, EntityCommandBuffer ecb)
        {
            ecb.AddComponent(ge, new CRemoveEffectWithTags
            {
                tags = new NativeArray<int>(tags, Allocator.Persistent)
            });
        }
    }
}
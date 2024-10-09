using Unity.Collections;
using Unity.Entities;

namespace GAS.RuntimeWithECS.GameplayEffect.Component
{
    public struct ComAssetTags : IComponentData
    {
        public NativeArray<int> tags;
    }
    
    public sealed class ConfAssetTags:GameplayEffectComponentConfig
    {
        public int[] tags;
        
        public override void LoadToGameplayEffectEntity(Entity ge)
        {
            _entityManager.AddComponentData(ge, new ComAssetTags
            {
                tags = new NativeArray<int>(tags, Allocator.Persistent)
            });
        }
    }
}
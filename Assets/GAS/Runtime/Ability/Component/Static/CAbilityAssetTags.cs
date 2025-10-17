using Unity.Collections;
using Unity.Entities;

namespace GAS.Runtime
{
    public struct CAbilityAssetTags : IComponentData
    {
        public NativeArray<int> tags;
    }
    
    public sealed class ConfAbilityAssetTags:AbilityComponentConfig
    {
        public int[] tags;
        
        public override void LoadToGameplayAbilityEntity(Entity ability)
        {
            _entityManager.AddComponentData(ability, new CAbilityAssetTags
            {
                tags = new NativeArray<int>(tags, Allocator.Persistent)
            });
        }
    }
}
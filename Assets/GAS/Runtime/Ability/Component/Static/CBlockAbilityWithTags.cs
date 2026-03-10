using Unity.Collections;
using Unity.Entities;

namespace GAS.Runtime
{
    public struct CBlockAbilityWithTags : IComponentData
    {
        public NativeArray<int> tags;
    }
    
    public sealed class ConfBlockAbilityWithTags:AbilityComponentConfig
    {
        public int[] tags;

        public override void LoadToGameplayAbilityEntity(Entity ability)
        {
            _entityManager.AddComponentData(ability, new CBlockAbilityWithTags
            {
                tags = new NativeArray<int>(tags, Allocator.Persistent)
            });
        }
    }
}
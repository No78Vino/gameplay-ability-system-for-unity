using Unity.Collections;
using Unity.Entities;

namespace GAS.Runtime
{
    public struct CCancelAbilityWithTags : IComponentData
    {
        public NativeArray<int> tags;
    }
    
    public sealed class ConfCancelAbilityWithTags:AbilityComponentConfig
    {
        public int[] tags;

        public override void LoadToGameplayAbilityEntity(Entity ability)
        {
            _entityManager.AddComponentData(ability, new CCancelAbilityWithTags
            {
                tags = new NativeArray<int>(tags, Allocator.Persistent)
            });
        }
    }
}
using Unity.Collections;
using Unity.Entities;

namespace GAS.Runtime
{
    public struct CCancelAbilityTags : IComponentData
    {
        public NativeArray<int> tags;
    }
    
    public sealed class ConfCancelAbilityTags:AbilityComponentConfig
    {
        public int[] tags;

        public override void LoadToGameplayAbilityEntity(Entity ability)
        {
            _entityManager.AddComponentData(ability, new CCancelAbilityTags
            {
                tags = new NativeArray<int>(tags, Allocator.Persistent)
            });
        }
    }
}
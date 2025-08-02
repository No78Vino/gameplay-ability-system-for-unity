using GAS.RuntimeWithECS.ComponentConfig;
using Unity.Collections;
using Unity.Entities;

namespace GAS.RuntimeWithECS.Static
{
    public struct CCancelAbilityTags : IComponentData
    {
        public NativeArray<int> tags;
    }
    
    public sealed class ConfCancelAbilityTags:GameplayAbilityComponentConfig
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
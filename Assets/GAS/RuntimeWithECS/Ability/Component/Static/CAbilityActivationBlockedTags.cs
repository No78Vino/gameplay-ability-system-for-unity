using Unity.Collections;
using Unity.Entities;

namespace GAS.Runtime
{
    public struct CAbilityActivationBlockedTags : IComponentData
    {
        public NativeArray<int> tags;
    }
    
    public sealed class ConfAbilityActivationBlockedTags:GameplayAbilityComponentConfig
    {
        public int[] tags;

        public override void LoadToGameplayAbilityEntity(Entity ability)
        {
            _entityManager.AddComponentData(ability, new CAbilityActivationBlockedTags
            {
                tags = new NativeArray<int>(tags, Allocator.Persistent)
            });
        }
    }
}
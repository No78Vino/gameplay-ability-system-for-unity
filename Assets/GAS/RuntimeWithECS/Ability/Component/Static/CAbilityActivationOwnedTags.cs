using Unity.Collections;
using Unity.Entities;

namespace GAS.Runtime
{
    public struct CAbilityActivationOwnedTags : IComponentData
    {
        public NativeArray<int> tags;
    }

    public sealed class ConfAbilityActivationOwnedTags : GameplayAbilityComponentConfig
    {
        public int[] tags;

        public override void LoadToGameplayAbilityEntity(Entity ability)
        {
            _entityManager.AddComponentData(ability, new CAbilityActivationOwnedTags
            {
                tags = new NativeArray<int>(tags, Allocator.Persistent)
            });
        }
    }
}
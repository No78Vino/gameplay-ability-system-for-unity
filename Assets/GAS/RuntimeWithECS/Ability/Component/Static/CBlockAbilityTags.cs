using GAS.RuntimeWithECS.ComponentConfig;
using Unity.Collections;
using Unity.Entities;

namespace GAS.RuntimeWithECS.Static
{
    public struct CBlockAbilityTags : IComponentData
    {
        public NativeArray<int> tags;
    }
    
    public sealed class ConfBlockAbilityTags:GameplayAbilityComponentConfig
    {
        public int[] tags;

        public override void LoadToGameplayAbilityEntity(Entity ability)
        {
            _entityManager.AddComponentData(ability, new CBlockAbilityTags
            {
                tags = new NativeArray<int>(tags, Allocator.Persistent)
            });
        }
    }
}
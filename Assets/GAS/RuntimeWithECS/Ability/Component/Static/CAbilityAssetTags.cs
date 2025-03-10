using GAS.RuntimeWithECS.Ability.ComponentConfig;
using Unity.Collections;
using Unity.Entities;

namespace GAS.RuntimeWithECS.Ability.Component.Static
{
    public struct CAbilityAssetTags : IComponentData
    {
        public NativeArray<int> tags;
    }
    
    public sealed class ConfAbilityAssetTags:GameplayAbilityComponentConfig
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
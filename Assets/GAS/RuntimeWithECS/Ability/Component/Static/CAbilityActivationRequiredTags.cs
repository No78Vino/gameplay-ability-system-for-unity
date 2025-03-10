using GAS.RuntimeWithECS.Ability.ComponentConfig;
using Unity.Collections;
using Unity.Entities;

namespace GAS.RuntimeWithECS.Ability.Component.Static
{
    public struct CAbilityActivationRequiredTags : IComponentData
    {
        public NativeArray<int> tags;
    }
    
    public sealed class ConfAbilityActivationRequiredTags:GameplayAbilityComponentConfig
    {
        public int[] tags;
        
        public override void LoadToGameplayAbilityEntity(Entity ability)
        {
            _entityManager.AddComponentData(ability, new CAbilityActivationRequiredTags
            {
                tags = new NativeArray<int>(tags, Allocator.Persistent)
            });
        }
    }
}
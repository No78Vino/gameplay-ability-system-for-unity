using System;
using Unity.Collections;
using Unity.Entities;

namespace GAS.Runtime
{
    public struct CAbilityActivationRequiredTags : IComponentData
    {
        public TagRequirementData requirement;
    }
    
    public sealed class ConfAbilityActivationRequiredTags:AbilityComponentConfig
    {
        public int[] tags;
        public int[] all;
        public int[] any;
        public int[] none;
        
        public override void LoadToGameplayAbilityEntity(Entity ability)
        {
            _entityManager.AddComponentData(ability, new CAbilityActivationRequiredTags
            {
                requirement = new TagRequirementData
                {
                    all = new NativeArray<int>(all ?? tags ?? Array.Empty<int>(), Allocator.Persistent),
                    any = new NativeArray<int>(any ?? Array.Empty<int>(), Allocator.Persistent),
                    none = new NativeArray<int>(none ?? Array.Empty<int>(), Allocator.Persistent)
                }
            });
        }
    }
}

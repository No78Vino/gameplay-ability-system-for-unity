using System;
using Unity.Collections;
using Unity.Entities;

namespace GAS.Runtime
{
    public struct CAbilityActivationBlockedTags : IComponentData
    {
        public TagRequirementData requirement;
    }
    
    public sealed class ConfAbilityActivationBlockedTags:AbilityComponentConfig
    {
        public int[] tags;
        public int[] all;
        public int[] any;
        public int[] none;

        public override void LoadToGameplayAbilityEntity(Entity ability)
        {
            _entityManager.AddComponentData(ability, new CAbilityActivationBlockedTags
            {
                requirement = new TagRequirementData
                {
                    all = new NativeArray<int>(all ?? Array.Empty<int>(), Allocator.Persistent),
                    any = new NativeArray<int>(any ?? Array.Empty<int>(), Allocator.Persistent),
                    none = new NativeArray<int>(none ?? tags ?? Array.Empty<int>(), Allocator.Persistent)
                }
            });
        }
    }
}

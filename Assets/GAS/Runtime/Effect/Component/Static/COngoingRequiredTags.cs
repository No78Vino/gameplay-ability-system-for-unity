using System;
using Unity.Collections;
using Unity.Entities;

namespace GAS.Runtime
{
    public struct COngoingRequiredTags : IComponentData
    {
        public TagRequirementData requirement;
    }

    public sealed class ConfOngoingRequiredTags : GameplayEffectComponentConfig
    {
        public int[] tags;
        public int[] all;
        public int[] any;
        public int[] none;

        public override void LoadToGameplayEffectEntity(Entity ge)
        {
            EntityHelper.AddComponent<COngoingRequiredTags>(ge);
            EntityHelper.SetComponent(ge, new COngoingRequiredTags
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

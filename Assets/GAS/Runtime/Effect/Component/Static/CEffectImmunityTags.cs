using System;
using Unity.Collections;
using Unity.Entities;

namespace GAS.Runtime
{
    public struct CEffectImmunityTags : IComponentData
    {
        public TagRequirementData requirement;
    }
    
    public sealed class ConfEffectImmunityTags:GameplayEffectComponentConfig
    {
        public int[] tags;
        public int[] all;
        public int[] any;
        public int[] none;
        
        public override void LoadToGameplayEffectEntity(Entity ge)
        {
            EntityHelper.AddComponent<CEffectImmunityTags>(ge);
            EntityHelper.SetComponent(ge, new CEffectImmunityTags
            {
                requirement = new TagRequirementData
                {
                    all = new NativeArray<int>(all ?? Array.Empty<int>(), Allocator.Persistent),
                    any = new NativeArray<int>(any ?? tags ?? Array.Empty<int>(), Allocator.Persistent),
                    none = new NativeArray<int>(none ?? Array.Empty<int>(), Allocator.Persistent)
                }
            });
        }
    }
}

using System;
using Unity.Collections;
using Unity.Entities;

namespace GAS.Runtime
{
    public struct CApplicationRequiredTags : IComponentData
    {
        public TagRequirementData requirement;
    }
    
    public sealed class ConfApplicationRequiredTags:GameplayEffectComponentConfig
    {
        public int[] tags;
        public int[] all;
        public int[] any;
        public int[] none;
        
        public override void LoadToGameplayEffectEntity(Entity ge)
        {
            EntityHelper.AddComponent<CApplicationRequiredTags>(ge);
            EntityHelper.SetComponent(ge, new CApplicationRequiredTags
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

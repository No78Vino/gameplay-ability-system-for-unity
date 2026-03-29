using Unity.Collections;
using Unity.Entities;

namespace GAS.Runtime
{
    public struct CEffectImmunityTagRequirement : IComponentData
    {
        public TagRequirementData query;
    }

    public sealed class ConfEffectImmunityTagRequirement : GameplayEffectComponentConfig
    {
        public int[] all;
        public int[] any;
        public int[] none;

        public override void LoadToGameplayEffectEntity(Entity ge)
        {
            EntityHelper.AddComponent<CEffectImmunityTagRequirement>(ge);
            EntityHelper.SetComponent(ge, new CEffectImmunityTagRequirement
            {
                query = new TagRequirementData
                {
                    all = new NativeArray<int>(all ?? System.Array.Empty<int>(), Allocator.Persistent),
                    any = new NativeArray<int>(any ?? System.Array.Empty<int>(), Allocator.Persistent),
                    none = new NativeArray<int>(none ?? System.Array.Empty<int>(), Allocator.Persistent)
                }
            });
        }
    }
}

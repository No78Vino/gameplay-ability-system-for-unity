using Unity.Collections;
using Unity.Entities;

namespace GAS.Runtime
{
    public struct CRemoveEffectWithTagRequirement : IComponentData
    {
        public TagRequirementData query;
    }

    public sealed class ConfRemoveEffectWithTagRequirement : GameplayEffectComponentConfig
    {
        public int[] all;
        public int[] any;
        public int[] none;

        public override void LoadToGameplayEffectEntity(Entity ge)
        {
            EntityHelper.AddComponent<CRemoveEffectWithTagRequirement>(ge);
            EntityHelper.SetComponent(ge, new CRemoveEffectWithTagRequirement
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

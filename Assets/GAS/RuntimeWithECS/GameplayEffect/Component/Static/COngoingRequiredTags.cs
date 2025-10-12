using Unity.Collections;
using Unity.Entities;

namespace GAS.Runtime
{
    public struct COngoingRequiredTags : IComponentData
    {
        public NativeArray<int> tags;
    }

    public sealed class ConfOngoingRequiredTags : GameplayEffectComponentConfig
    {
        public int[] tags;

        public override void LoadToGameplayEffectEntity(Entity ge)
        {
            EntityHelper.AddComponent<COngoingRequiredTags>(ge);
            EntityHelper.SetComponent(ge, new COngoingRequiredTags
            {
                tags = new NativeArray<int>(tags, Allocator.Persistent)
            });
        }
    }
}
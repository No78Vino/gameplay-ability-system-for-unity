using Unity.Collections;
using Unity.Entities;

namespace GAS.Runtime
{
    public struct CEffectGrantedTags : IComponentData
    {
        public NativeArray<int> tags;
    }

    public sealed class ConfEffectGrantedTags : GameplayEffectComponentConfig
    {
        public int[] tags;

        public override void LoadToGameplayEffectEntity(Entity ge)
        {
            EntityHelper.AddComponent<CEffectGrantedTags>(ge);
            EntityHelper.SetComponent(ge, new CEffectGrantedTags
            {
                tags = new NativeArray<int>(tags, Allocator.Persistent)
            });
        }
    }
}
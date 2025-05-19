using GAS.Runtime;
using GAS.RuntimeWithECS.GameplayEffect;
using Unity.Collections;
using Unity.Entities;
using NotImplementedException = System.NotImplementedException;

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
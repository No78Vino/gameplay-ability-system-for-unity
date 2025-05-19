using GAS.Runtime;
using GAS.RuntimeWithECS.GameplayEffect;
using Unity.Collections;
using Unity.Entities;

namespace GAS.Runtime
{
    public struct CEffectImmunityTags : IComponentData
    {
        public NativeArray<int> tags;
    }
    
    public sealed class ConfEffectImmunityTags:GameplayEffectComponentConfig
    {
        public int[] tags;
        
        public override void LoadToGameplayEffectEntity(Entity ge)
        {
            EntityHelper.AddComponent<CEffectImmunityTags>(ge);
            EntityHelper.SetComponent(ge, new CEffectImmunityTags
            {
                tags = new NativeArray<int>(tags, Allocator.Persistent)
            });
        }
    }
}
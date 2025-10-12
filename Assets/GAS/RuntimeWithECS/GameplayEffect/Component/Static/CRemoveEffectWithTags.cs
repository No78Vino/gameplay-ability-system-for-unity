using Unity.Collections;
using Unity.Entities;

namespace GAS.Runtime
{
    public struct CRemoveEffectWithTags : IComponentData
    {
        public NativeArray<int> tags;
    }
    
    public sealed class ConfRemoveEffectWithTags:GameplayEffectComponentConfig
    {
        public int[] tags;
        
        public override void LoadToGameplayEffectEntity(Entity ge)
        {
            EntityHelper.AddComponent<CRemoveEffectWithTags>(ge);
            EntityHelper.SetComponent(ge, new CRemoveEffectWithTags
            {
                tags = new NativeArray<int>(tags, Allocator.Persistent)
            });
        }
    }
}
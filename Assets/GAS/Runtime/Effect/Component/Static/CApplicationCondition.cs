using Unity.Collections;
using Unity.Entities;

namespace GAS.Runtime
{
    public struct CApplicationCondition : IComponentData
    {
        public NativeArray<int> conditions;
    }
    
    public sealed class ConfApplicationCondition:GameplayEffectComponentConfig
    {
        public int[] tags;
        
        public override void LoadToGameplayEffectEntity(Entity ge)
        {
            EntityHelper.AddComponent<CEffectAssetTags>(ge);
            EntityHelper.SetComponent(ge, new CEffectAssetTags
            {
                tags = new NativeArray<int>(tags, Allocator.Persistent)
            });
        }
    }
}
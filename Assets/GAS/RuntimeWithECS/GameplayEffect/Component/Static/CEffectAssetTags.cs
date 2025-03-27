using Unity.Collections;
using Unity.Entities;

namespace GAS.RuntimeWithECS.GameplayEffect.Component
{
    public struct CEffectAssetTags : IComponentData
    {
        /// <summary>
        /// AssetTags,描述GE性质的Tag。用于Tag相关逻辑判断。
        /// </summary>
        public NativeArray<int> tags;
    }
    
    public sealed class ConfAssetTags:GameplayEffectComponentConfig
    {
        public int[] tags;
        
        public override void LoadToGameplayEffectEntity(Entity ge)
        {
            _entityManager.AddComponentData(ge, new CEffectAssetTags
            {
                tags = new NativeArray<int>(tags, Allocator.Persistent)
            });
        }
    }
}
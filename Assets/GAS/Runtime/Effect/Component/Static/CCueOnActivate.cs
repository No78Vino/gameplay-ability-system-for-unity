using Unity.Collections;
using Unity.Entities;

namespace GAS.Runtime
{
    public struct CCueOnActivate : IComponentData
    {
        public NativeArray<Entity> cues;
        
        /// <summary>
        /// 运行时的实际Cue实例
        /// </summary>
        public NativeArray<Entity> runtimeCues;
    }

    public sealed class ConfCueOnActivate : ConfCueBase
    {
        public override void LoadToGameplayEffectEntity(Entity ge)
        {
            var entities = CreateCueEntityArray(ge);
            EntityHelper.AddComponent<CCueOnActivate>(ge);
            EntityHelper.SetComponent(ge, new CCueOnActivate
            {
                cues = entities
            });
        }
    }
}
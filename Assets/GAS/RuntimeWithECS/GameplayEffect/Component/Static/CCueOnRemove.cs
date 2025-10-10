using GAS.RuntimeWithECS.Cue;
using Unity.Collections;
using Unity.Entities;

namespace GAS.Runtime
{
    public struct CCueOnRemove : IComponentData
    {
        /// <summary>
        ///     cue entity
        /// </summary>
        public NativeArray<Entity> cues;
        
        
        /// <summary>
        /// 运行时的实际Cue实例
        /// </summary>
        public NativeArray<Entity> runtimeCues;
    }

    public sealed class ConfCueOnRemove : ConfCueBase
    {
        public override void LoadToGameplayEffectEntity(Entity ge)
        {
            var entities = CreateCueEntityArray(ge);
            EntityHelper.AddComponent<CCueOnRemove>(ge);
            EntityHelper.SetComponent(ge, new CCueOnRemove
            {
                cues = entities
            });
        }
    }
}
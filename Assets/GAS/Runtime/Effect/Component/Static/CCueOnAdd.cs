using Unity.Collections;
using Unity.Entities;

namespace GAS.Runtime
{
    public struct CCueOnAdd : IComponentData
    {
        /// <summary>
        ///     cue entity
        /// </summary>
        public NativeArray<Entity> cues;
    }

    public sealed class ConfCueOnAdd : ConfCueBase
    {
        public override void LoadToGameplayEffectEntity(Entity ge)
        {
            var entities = CreateCueEntityArray(ge);
            EntityHelper.AddComponent<CCueOnAdd>(ge);
            EntityHelper.SetComponent(ge, new CCueOnAdd
            {
                cues = entities
            });
        }
    }
}
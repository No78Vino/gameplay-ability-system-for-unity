using Unity.Collections;
using Unity.Entities;

namespace GAS.Runtime
{
    public struct CCueOnDeactivate : IComponentData
    {
        public NativeArray<Entity> cues;
    }

    public sealed class ConfCueOnDeactivate : ConfCueBase
    {
        public override void LoadToGameplayEffectEntity(Entity ge)
        {
            var entities = CreateCueEntityArray(ge);
            EntityHelper.AddComponent<CCueOnDeactivate>(ge);
            EntityHelper.SetComponent(ge, new CCueOnDeactivate
            {
                cues = entities
            });
        }
    }
}
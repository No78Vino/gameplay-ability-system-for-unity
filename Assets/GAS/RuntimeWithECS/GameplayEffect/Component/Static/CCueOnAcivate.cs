using Unity.Collections;
using Unity.Entities;

namespace GAS.Runtime
{
    public struct CCueOnAcivate : IComponentData
    {
        public NativeArray<Entity> cues;
    }

    public sealed class ConfCueOnActivate : ConfCueBase
    {
        public override void LoadToGameplayEffectEntity(Entity ge)
        {
            var entities = CreateCueEntityArray(ge);
            EntityHelper.AddComponent<CCueOnAcivate>(ge);
            EntityHelper.SetComponent(ge, new CCueOnAcivate
            {
                cues = entities
            });
        }
    }
}
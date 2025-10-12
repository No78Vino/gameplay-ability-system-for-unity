using Unity.Collections;
using Unity.Entities;

namespace GAS.Runtime
{
    public struct CCueOnDeacivate : IComponentData
    {
        public NativeArray<Entity> cues;
    }

    public sealed class ConfCueOnDeactivate : ConfCueBase
    {
        public override void LoadToGameplayEffectEntity(Entity ge)
        {
            var entities = CreateCueEntityArray(ge);
            EntityHelper.AddComponent<CCueOnDeacivate>(ge);
            EntityHelper.SetComponent(ge, new CCueOnDeacivate
            {
                cues = entities
            });
        }
    }
}
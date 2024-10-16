using Unity.Collections;
using Unity.Entities;

namespace GAS.RuntimeWithECS.GameplayEffect.Component
{
    public struct ComCueOnAdd : IComponentData
    {
        public NativeArray<Entity> cues;
    }
}
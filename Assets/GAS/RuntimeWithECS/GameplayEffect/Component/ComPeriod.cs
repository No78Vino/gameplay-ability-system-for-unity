using Unity.Collections;
using Unity.Entities;

namespace GAS.RuntimeWithECS.GameplayEffect.Component
{
    public struct ComPeriod : IComponentData
    {
        public int period;
        public NativeArray<Entity> gameplayEffects;
    }
}
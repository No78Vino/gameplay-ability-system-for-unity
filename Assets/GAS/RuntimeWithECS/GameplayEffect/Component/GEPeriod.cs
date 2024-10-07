using Unity.Collections;
using Unity.Entities;

namespace GAS.RuntimeWithECS.GameplayEffect.Component
{
    public struct GEPeriod : IComponentData
    {
        public int period;
        public NativeArray<Entity> gameplayEffects;
    }
}
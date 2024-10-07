using Unity.Entities;

namespace GAS.RuntimeWithECS.GameplayEffect.Component
{
    [InternalBufferCapacity(500)]
    public struct GameplayEffectBufferElement : IBufferElementData
    {
        public Entity GameplayEffect;
    }
}
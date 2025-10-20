using Unity.Entities;

namespace GAS.Runtime
{
    [InternalBufferCapacity(GASParameterSetting.ASC_MAX_GAMEPLAY_EFFECT_COUNT)]
    public struct BGameplayEffect : IBufferElementData
    {
        public Entity GameplayEffect;
    }
}
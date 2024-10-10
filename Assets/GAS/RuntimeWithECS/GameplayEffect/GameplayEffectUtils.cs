using GAS.RuntimeWithECS.Core;
using GAS.RuntimeWithECS.GameplayEffect.Component;
using Unity.Entities;

namespace GAS.RuntimeWithECS.GameplayEffect
{
    public static class GameplayEffectUtils
    {
        private static EntityManager GasEntityManager => GASManager.EntityManager;

        public static DynamicBuffer<GameplayEffectBufferElement> GameplayEffectsOf(Entity asc)
        {
            return GasEntityManager.GetBuffer<GameplayEffectBufferElement>(asc);
        }
    }
}
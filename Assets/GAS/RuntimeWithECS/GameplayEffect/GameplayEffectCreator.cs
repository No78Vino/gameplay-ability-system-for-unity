using GAS.EditorForECS.GameplayEffect;
using GAS.RuntimeWithECS.Core;
using Unity.Entities;

namespace GAS.RuntimeWithECS.GameplayEffect
{
    public static class GameplayEffectCreator
    {
        private static EntityManager _entityManager => GASManager.EntityManager;

        public static Entity Create(NewGameplayEffectAsset asset)
        {
            return Create(asset.components);
        }

        public static Entity Create(GameplayEffectComponentConfig[] componentAssets)
        {
            var entity = _entityManager.CreateEntity();

            foreach (var config in componentAssets) 
                config.LoadToGameplayEffectEntity(entity);
            return entity;
        }
    }
}
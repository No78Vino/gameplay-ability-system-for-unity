using GAS.EditorForECS.GameplayEffect;
using GAS.RuntimeWithECS.Core;
using Unity.Entities;

namespace GAS.RuntimeWithECS.GameplayEffect
{
    public static class GameplayEffectCreator
    {
        private static EntityManager _entityManager => GASManager.EntityManager;

        public static Entity CreateGameplayEffectEntity(NewGameplayEffectAsset asset)
        {
            return CreateGameplayEffectEntity(asset.components);
        }

        public static Entity CreateGameplayEffectEntity(GameplayEffectComponentConfig[] componentAssets)
        {
            var entity = _entityManager.CreateEntity();

            foreach (var config in componentAssets) 
                config.LoadToGameplayEffectEntity(entity);
            return entity;
        }

        public static NewGameplayEffectSpec CreateGameplayEffectSpec(NewGameplayEffectAsset asset)
        {
            return CreateGameplayEffectSpec(asset.components);
        }
        
        public static NewGameplayEffectSpec CreateGameplayEffectSpec(GameplayEffectComponentConfig[] componentAssets)
        {
            return new NewGameplayEffectSpec(componentAssets);
        }
    }
}
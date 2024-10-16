using Unity.Collections;
using Unity.Entities;

namespace GAS.RuntimeWithECS.GameplayEffect.Component
{
    public struct ComPeriod : IComponentData
    {
        public int Period;
        public NativeArray<Entity> GameplayEffects;
    }
    
    public sealed class ConfPeriod:GameplayEffectComponentConfig
    {
        public int Period;
        public GameplayEffectComponentConfig[][] GameplayEffectSettings;

        public override void LoadToGameplayEffectEntity(Entity ge)
        {
            var geEntities = new NativeArray<Entity>(GameplayEffectSettings.Length, Allocator.Persistent);
            for (var i = 0; i < GameplayEffectSettings.Length; i++)
            {
                var comConfigs = GameplayEffectSettings[i];
                geEntities[i] = GameplayEffectCreator.CreateGameplayEffectEntity(comConfigs);
            }

            _entityManager.AddComponentData(ge, new ComPeriod
            {
                Period = Period,
                GameplayEffects = geEntities,
            });
        }
    }
}
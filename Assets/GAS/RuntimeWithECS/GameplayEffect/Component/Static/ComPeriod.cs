using Unity.Collections;
using Unity.Entities;

namespace GAS.RuntimeWithECS.GameplayEffect.Component
{
    public struct ComPeriod : IComponentData
    {
        public int Period;
        public bool ResetTimeCountWhenDeactivated;
        
        public NativeArray<Entity> GameplayEffects;
        
        // -------------------------------------以下是RUNTIME数据，不需要初始化---------------------------------------//
        public int startTime;
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
                geEntities[i] = GEUtil.CreateGameplayEffectEntity(comConfigs);
            }

            _entityManager.AddComponentData(ge, new ComPeriod
            {
                Period = Period,
                GameplayEffects = geEntities,
            });
        }
    }
}
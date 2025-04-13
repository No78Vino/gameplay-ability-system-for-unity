using Unity.Entities;

namespace GAS.RuntimeWithECS.GameplayEffect
{
    public class GameplayEffectConfig
    {
        private GameplayEffectComponentConfig[] _componentConfigs;

        public GameplayEffectComponentConfig[] ComponentConfigs => _componentConfigs;
        
        public GameplayEffectConfig(GameplayEffectComponentConfig[] configs)
        {
            _componentConfigs = configs;
        }

        public Entity CreateGameplayEffectEntity()
        {
            return GEUtil.CreateGameplayEffectEntity(_componentConfigs);
        }
        
        public Entity CreateGameplayEffectEntity(EntityCommandBuffer ecb)
        {
            return GEUtil.CreateGameplayEffectEntity(_componentConfigs,ecb);
        }
    }
}
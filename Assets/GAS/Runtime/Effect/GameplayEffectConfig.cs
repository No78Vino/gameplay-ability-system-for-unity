using Unity.Entities;

namespace GAS.Runtime
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
            return GameplayEffectHelper.CreateGameplayEffectEntity(_componentConfigs);
        }
    }
}
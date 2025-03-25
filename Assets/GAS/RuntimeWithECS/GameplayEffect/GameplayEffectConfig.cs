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
    }
}
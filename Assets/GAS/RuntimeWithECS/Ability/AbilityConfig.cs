using GAS.RuntimeWithECS.ComponentConfig;

namespace GAS.RuntimeWithECS
{
    public class AbilityConfig
    {
        private GameplayAbilityComponentConfig[] _componentConfigs;

        public GameplayAbilityComponentConfig[] ComponentConfigs => _componentConfigs;
        
        public AbilityConfig(GameplayAbilityComponentConfig[] configs)
        {
            _componentConfigs = configs;
        }
    }
}
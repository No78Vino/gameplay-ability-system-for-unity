using GAS.RuntimeWithECS.Ability.ComponentConfig;

namespace GAS.RuntimeWithECS.Ability
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
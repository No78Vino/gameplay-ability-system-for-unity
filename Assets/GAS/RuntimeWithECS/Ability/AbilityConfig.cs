using GAS.RuntimeWithECS.ComponentConfig;

namespace GAS.Runtime
{
    public class AbilityConfig
    {
        public GameplayAbilityComponentConfig[] ComponentConfigs { get; }

        public AbilityConfig(GameplayAbilityComponentConfig[] configs)
        {
            ComponentConfigs = configs;
        }
    }
}
using GAS.RuntimeWithECS.GameplayEffect;

namespace GAS.RuntimeWithECS.Ability.Component
{
    public class AbilityParamArrayGameplayEffect: AbilityParamBase
    {
        private GameplayEffectConfig[] _value;
        public GameplayEffectConfig[] Value => _value;
        
        public void SetValue(GameplayEffectConfig[] value)
        {
            _value = value;
        }
        
        public AbilityParamArrayGameplayEffect(GameplayEffectConfig[] value)
        {
            _value = value;
        }
        
        public AbilityParamArrayGameplayEffect()
        {
        }
    }
}
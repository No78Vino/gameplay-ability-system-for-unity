using GAS.RuntimeWithECS.GameplayEffect;

namespace GAS.RuntimeWithECS.Ability.Component
{
    public class AbilityParamArrayGameplayEffect: AbilityParamBase
    {
        private string[] _value;
        public string[] Value => _value;
        
        public void SetValue(string[] value)
        {
            _value = value;
        }
        
        public AbilityParamArrayGameplayEffect(string[] value)
        {
            _value = value;
        }
        
        public AbilityParamArrayGameplayEffect()
        {
        }
    }
}
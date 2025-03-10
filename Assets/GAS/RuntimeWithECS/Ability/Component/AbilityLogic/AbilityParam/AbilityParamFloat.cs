namespace GAS.RuntimeWithECS.Ability.Component
{
    public class AbilityParamFloat: AbilityParamBase
    {
        private float _value;
        public float Value => _value;
        
        public void SetValue(float value)
        {
            _value = value;
        }
    }
}
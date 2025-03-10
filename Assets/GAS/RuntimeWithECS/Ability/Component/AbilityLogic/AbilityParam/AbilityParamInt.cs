namespace GAS.RuntimeWithECS.Ability.Component
{
    public class AbilityParamInt: AbilityParamBase
    {
        private int _value;
        public int Value => _value;
        
        public void SetValue(int value)
        {
            _value = value;
        }
    }
}
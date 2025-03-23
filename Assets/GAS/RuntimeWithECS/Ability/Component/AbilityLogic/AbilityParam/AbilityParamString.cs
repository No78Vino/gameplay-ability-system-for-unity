namespace GAS.RuntimeWithECS.Ability.Component
{
    public class AbilityParamString: AbilityParamBase
    {
        private string _value;
        public string Value => _value;
        
        public AbilityParamString(string value)
        {
            SetValue(value);
        }
        
        public void SetValue(string value)
        {
            _value = value;
        }
        
        public override string ToString()
        {
            return _value;
        }
    }
}
namespace GAS.RuntimeWithECS.Ability.Component
{
    public class AbilityParamArrayInt: AbilityParamBase
    {
        private int[] _value;
        public int[] Value => _value;
        
        public void SetValue(int[] value)
        {
            _value = value;
        }
        
        public AbilityParamArrayInt(int[] value)
        {
            _value = value;
        }
    }
}
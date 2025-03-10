namespace GAS.RuntimeWithECS.Ability.Component
{
    public class AbilityParamVector2: AbilityParamBase
    {
        private UnityEngine.Vector2 _value;
        public UnityEngine.Vector2 Value => _value;
        
        public void SetValue(UnityEngine.Vector2 value)
        {
            _value = value;
        }
    }
}
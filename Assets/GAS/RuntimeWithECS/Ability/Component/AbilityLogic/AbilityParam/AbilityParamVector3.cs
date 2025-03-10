namespace GAS.RuntimeWithECS.Ability.Component
{
    public class AbilityParamVector3: AbilityParamBase
    {
        private UnityEngine.Vector3 _value;
        public UnityEngine.Vector3 Value => _value;
        
        public void SetValue(UnityEngine.Vector3 value)
        {
            _value = value;
        }
    }
}
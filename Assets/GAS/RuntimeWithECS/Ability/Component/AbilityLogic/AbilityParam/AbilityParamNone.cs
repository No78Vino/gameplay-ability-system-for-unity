namespace GAS.RuntimeWithECS.Ability.Component
{
    public class AbilityParamNone: AbilityParamBase
    {
        private static AbilityParamNone _instance;
        
        public static AbilityParamNone None => _instance ??= new AbilityParamNone();
    }
}
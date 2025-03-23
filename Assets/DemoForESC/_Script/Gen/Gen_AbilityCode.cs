using GAS.RuntimeWithECS.Ability;

namespace DemoForESC._Script.Gen
{
    public static class Gen_AbilityCode
    {
        public const int DebugLog = 0;
        public const int Move = 1;
        public const int Jump = 2;
        public const int Attack = 3;
        public const int Attack_Monster = 4;
        
        public static void LoadAbilityCode()
        {
            AbilityHelper.RegisterAbilityLogic(DebugLog, typeof(GAS.RuntimeWithECS.Ability.Component.CommonAbilityLogic.ALDebugLog));
            // AbilityHelper.RegisterAbilityLogic(Move, new AbilityLogic_Move());
            // AbilityHelper.RegisterAbilityLogic(Jump, new AbilityLogic_Jump());
            // AbilityHelper.RegisterAbilityLogic(Attack, new AbilityLogic_Attack());
            // AbilityHelper.RegisterAbilityLogic(Attack_Monster, new AbilityLogic_Attack());
        }
    }
}
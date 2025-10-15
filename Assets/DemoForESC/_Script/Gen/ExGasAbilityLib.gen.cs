///////////////////////////////////
//// This is a generated file. ////
////     Do not modify it.     ////
///////////////////////////////////

using System;
using System.Collections.Generic;
namespace GAS.Runtime
{
    public static class XAbility
    {
        public const int ABILITY_move = 10001;
        public const int ABILITY_RunSpeedUp = 10002;
        public const int ABILITY_debug_ge_ability = 1003;
        public const int ABILITY_debug_ge_2 = 1005;

        public static void LoadAbilityCode()
        {
            var ALMove = typeof(DemoForESC._Script.Gas.Ability.ALMove);
            GAS.Runtime.AbilityHelper.RegisterAbilityLogic(ALMove.Name, ALMove,typeof(DemoForESC._Script.Gas.Ability.AbilityParamMove));
            var ALApplyEffect = typeof(GAS.Runtime.ALApplyEffect);
            GAS.Runtime.AbilityHelper.RegisterAbilityLogic(ALApplyEffect.Name, ALApplyEffect,typeof(GAS.Runtime.AbilityParamArrayInt));
            var ALDebugLog = typeof(GAS.Runtime.ALDebugLog);
            GAS.Runtime.AbilityHelper.RegisterAbilityLogic(ALDebugLog.Name, ALDebugLog,typeof(GAS.Runtime.AbilityParamString));
            var ALTimeline = typeof(GAS.Runtime.ALTimeline);
            GAS.Runtime.AbilityHelper.RegisterAbilityLogic(ALTimeline.Name, ALTimeline,typeof(GAS.Runtime.AbilityParamTimeline));
        }
    }
}

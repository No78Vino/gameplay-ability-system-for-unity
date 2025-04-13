///////////////////////////////////
//// This is a generated file. ////
////     Do not modify it.     ////
///////////////////////////////////

using System;
using System.Collections.Generic;
namespace GAS.Runtime
{
    public static class GEN_AbilityCode
    {
        public const int ABILITY_move = 1414245001;
        public const int ABILITY_RunSpeedUp = -203323501;

        public static void LoadAbilityCode()
        {
            var ALMove = typeof(DemoForESC._Script.Gas.Ability.ALMove);
            GAS.RuntimeWithECS.Ability.AbilityHelper.RegisterAbilityLogic(ALMove.FullName, ALMove);
            var ALApplyEffect = typeof(GAS.RuntimeWithECS.Ability.Component.CommonAbilityLogic.ALApplyEffect);
            GAS.RuntimeWithECS.Ability.AbilityHelper.RegisterAbilityLogic(ALApplyEffect.FullName, ALApplyEffect);
            var ALDebugLog = typeof(GAS.RuntimeWithECS.Ability.Component.CommonAbilityLogic.ALDebugLog);
            GAS.RuntimeWithECS.Ability.AbilityHelper.RegisterAbilityLogic(ALDebugLog.FullName, ALDebugLog);
        }
    }
    }

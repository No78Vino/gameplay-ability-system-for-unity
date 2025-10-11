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

        public static void LoadAbilityCode()
        {
            var ALApplyEffect = typeof(GAS.RuntimeWithECS.CommonAbilityLogic.ALApplyEffect);
            GAS.RuntimeWithECS.AbilityHelper.RegisterAbilityLogic(ALApplyEffect.Name, ALApplyEffect,typeof(GAS.RuntimeWithECS.AbilityParamArrayInt));
            var ALDebugLog = typeof(GAS.RuntimeWithECS.CommonAbilityLogic.ALDebugLog);
            GAS.RuntimeWithECS.AbilityHelper.RegisterAbilityLogic(ALDebugLog.Name, ALDebugLog,typeof(GAS.RuntimeWithECS.AbilityParamString));
            var ALTimeline = typeof(GAS.Runtime.ALTimeline);
            GAS.RuntimeWithECS.AbilityHelper.RegisterAbilityLogic(ALTimeline.Name, ALTimeline,typeof(GAS.Runtime.AbilityParamTimeline));
            var ALMove = typeof(DemoForESC._Script.Gas.Ability.ALMove);
            GAS.RuntimeWithECS.AbilityHelper.RegisterAbilityLogic(ALMove.Name, ALMove,typeof(DemoForESC._Script.Gas.Ability.AbilityParamMove));
        }
    }
}

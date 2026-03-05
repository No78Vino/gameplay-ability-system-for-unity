///////////////////////////////////
//// This is a generated file. ////
////     Do not modify it.     ////
///////////////////////////////////

namespace GAS.Runtime
{
    public static class XAbility
    {
        public const int ABILITY_move = 10001;
        public const int ABILITY_RunSpeedUp = 10002;
        public const int ABILITY_debug_ge_ability = 1003;
        public const int ABILITY_debug_ge_2 = 1005;
        public const int ABILITY_Attack = 20001;

        public static void LoadAbilityCode()
        {
            ///  AbilityLogic
            var ALDeath = typeof(DemoForESC._Script.Gas.Ability.ALDeath);
            GAS.Runtime.AbilityHelper.RegisterAbilityLogic(ALDeath.Name, ALDeath,typeof(GAS.Runtime.XParamEffectIDs));
            var ALMove = typeof(DemoForESC._Script.Gas.Ability.ALMove);
            GAS.Runtime.AbilityHelper.RegisterAbilityLogic(ALMove.Name, ALMove,typeof(DemoForESC._Script.Gas.Ability.XParamMove));
            var ALApplyEffect = typeof(GAS.Runtime.ALApplyEffect);
            GAS.Runtime.AbilityHelper.RegisterAbilityLogic(ALApplyEffect.Name, ALApplyEffect,typeof(GAS.Runtime.XParamEffectIDs));
            var ALDebugLog = typeof(GAS.Runtime.ALDebugLog);
            GAS.Runtime.AbilityHelper.RegisterAbilityLogic(ALDebugLog.Name, ALDebugLog,typeof(GAS.Runtime.XParamString));
            var ALTimeline = typeof(GAS.Runtime.ALTimeline);
            GAS.Runtime.AbilityHelper.RegisterAbilityLogic(ALTimeline.Name, ALTimeline,typeof(GAS.Runtime.XParamALTimelineID));

            ///  AbilityTask
            var TaskPlayCuePreset = typeof(GAS.Runtime.TaskPlayCuePreset);
            GAS.Runtime.AbilityHelper.RegisterAbilityTask(TaskPlayCuePreset.Name, TaskPlayCuePreset,typeof(GAS.Runtime.XParamCueList));
            var TaskApplyEffects = typeof(GAS.Runtime.TaskApplyEffects);
            GAS.Runtime.AbilityHelper.RegisterAbilityTask(TaskApplyEffects.Name, TaskApplyEffects,typeof(GAS.Runtime.XParamApplyEffects));
            var TaskDebug = typeof(GAS.Runtime.TaskDebug);
            GAS.Runtime.AbilityHelper.RegisterAbilityTask(TaskDebug.Name, TaskDebug,typeof(GAS.Runtime.XParamString));
            var TaskDoCost = typeof(GAS.Runtime.TaskDoCost);
            GAS.Runtime.AbilityHelper.RegisterAbilityTask(TaskDoCost.Name, TaskDoCost,typeof(GAS.Runtime.XParamNone));
            var TaskDoNothing = typeof(GAS.Runtime.TaskDoNothing);
            GAS.Runtime.AbilityHelper.RegisterAbilityTask(TaskDoNothing.Name, TaskDoNothing,typeof(GAS.Runtime.XParamNone));
            var TaskPlayCue = typeof(GAS.Runtime.TaskPlayCue);
            GAS.Runtime.AbilityHelper.RegisterAbilityTask(TaskPlayCue.Name, TaskPlayCue,typeof(GAS.Runtime.XParamCue));

            ///  TargetCatcher
            var CatchAreaBox3D = typeof(GAS.Runtime.CatchAreaBox3D);
            GAS.Runtime.TargetCatcherHelper.RegisterTargetCatcher(CatchAreaBox3D.Name, CatchAreaBox3D,typeof(GAS.Runtime.XParamCatchAreaBox3D));
            var CatchSelf = typeof(GAS.Runtime.CatchSelf);
            GAS.Runtime.TargetCatcherHelper.RegisterTargetCatcher(CatchSelf.Name, CatchSelf,typeof(GAS.Runtime.XParamNone));
            var CatchTarget = typeof(GAS.Runtime.CatchTarget);
            GAS.Runtime.TargetCatcherHelper.RegisterTargetCatcher(CatchTarget.Name, CatchTarget,typeof(GAS.Runtime.XParamNone));
        }
    }
}

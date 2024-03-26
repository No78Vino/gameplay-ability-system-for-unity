///////////////////////////////////
//// This is a generated file. ////
////     Do not modify it.     ////
///////////////////////////////////

using System;
using System.Collections.Generic;

namespace GAS.Runtime
{
    public static class GAbilityLib
    {
        public struct AbilityInfo
        {
            public string Name;
            public string AssetPath;
            public Type AbilityClassType;
        }

        public static AbilityInfo BossAttack01 = new AbilityInfo { Name = "BossAttack01", AssetPath = "Assets/Demo/Resources/GAS_Setting/Config/GameplayAbilityLib/Boss/BossAttack01.asset",AbilityClassType = typeof(GAS.Runtime.TimelineAbility) };

        public static AbilityInfo BossAttack02 = new AbilityInfo { Name = "BossAttack02", AssetPath = "Assets/Demo/Resources/GAS_Setting/Config/GameplayAbilityLib/Boss/BossAttack02.asset",AbilityClassType = typeof(GAS.Runtime.TimelineAbility) };

        public static AbilityInfo BossAttack03 = new AbilityInfo { Name = "BossAttack03", AssetPath = "Assets/Demo/Resources/GAS_Setting/Config/GameplayAbilityLib/Boss/BossAttack03.asset",AbilityClassType = typeof(GAS.Runtime.TimelineAbility) };

        public static AbilityInfo BossAttack04 = new AbilityInfo { Name = "BossAttack04", AssetPath = "Assets/Demo/Resources/GAS_Setting/Config/GameplayAbilityLib/Boss/BossAttack04.asset",AbilityClassType = typeof(GAS.Runtime.TimelineAbility) };

        public static AbilityInfo BossDie = new AbilityInfo { Name = "BossDie", AssetPath = "Assets/Demo/Resources/GAS_Setting/Config/GameplayAbilityLib/Boss/BossDie.asset",AbilityClassType = typeof(GAS.Runtime.TimelineAbility) };

        public static AbilityInfo Die = new AbilityInfo { Name = "Die", AssetPath = "Assets/Demo/Resources/GAS_Setting/Config/GameplayAbilityLib/Die.asset",AbilityClassType = typeof(GAS.Runtime.TimelineAbility) };

        public static AbilityInfo Jump = new AbilityInfo { Name = "Jump", AssetPath = "Assets/Demo/Resources/GAS_Setting/Config/GameplayAbilityLib/Jump.asset",AbilityClassType = typeof(GAS.Runtime.Jump) };

        public static AbilityInfo Move = new AbilityInfo { Name = "Move", AssetPath = "Assets/Demo/Resources/GAS_Setting/Config/GameplayAbilityLib/Move.asset",AbilityClassType = typeof(GAS.Runtime.Move) };

        public static AbilityInfo Attack = new AbilityInfo { Name = "Attack", AssetPath = "Assets/Demo/Resources/GAS_Setting/Config/GameplayAbilityLib/Player/Attack.asset",AbilityClassType = typeof(GAS.Runtime.TimelineAbility) };

        public static AbilityInfo Defend = new AbilityInfo { Name = "Defend", AssetPath = "Assets/Demo/Resources/GAS_Setting/Config/GameplayAbilityLib/Player/Defend.asset",AbilityClassType = typeof(GAS.Runtime.TimelineAbility) };

        public static AbilityInfo DodgeStep = new AbilityInfo { Name = "DodgeStep", AssetPath = "Assets/Demo/Resources/GAS_Setting/Config/GameplayAbilityLib/Player/DodgeStep.asset",AbilityClassType = typeof(GAS.Runtime.TimelineAbility) };


        public static Dictionary<string, AbilityInfo> AbilityMap = new Dictionary<string, AbilityInfo>
        {
            ["BossAttack01"] = BossAttack01,
            ["BossAttack02"] = BossAttack02,
            ["BossAttack03"] = BossAttack03,
            ["BossAttack04"] = BossAttack04,
            ["BossDie"] = BossDie,
            ["Die"] = Die,
            ["Jump"] = Jump,
            ["Move"] = Move,
            ["Attack"] = Attack,
            ["Defend"] = Defend,
            ["DodgeStep"] = DodgeStep,
        };
    }
}
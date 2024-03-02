///////////////////////////////////
//// This is a generated file. ////
////     Do not modify it.     ////
///////////////////////////////////
using System;
using System.Collections.Generic;

namespace GAS.Runtime.Ability
{
  public static class AbilityCollection
  {
      public struct AbilityInfo
      {
          public string Name;
          public string AssetPath;
          public Type AbilityClassType;
      }

    public static AbilityInfo Jump_Info = new AbilityInfo { Name = "Jump", AssetPath = "Assets/Demo/Resources/GAS_Setting/Config/GameplayAbilityLib/Jump.asset",AbilityClassType = typeof(GAS.Runtime.Ability.Jump) };

    public static AbilityInfo Move_Info = new AbilityInfo { Name = "Move", AssetPath = "Assets/Demo/Resources/GAS_Setting/Config/GameplayAbilityLib/Move.asset",AbilityClassType = typeof(GAS.Runtime.Ability.Move) };

    public static AbilityInfo DodgeStep_Info = new AbilityInfo { Name = "DodgeStep", AssetPath = "Assets/Demo/Resources/GAS_Setting/Config/GameplayAbilityLib/Player/DodgeStep.asset",AbilityClassType = typeof(GAS.Runtime.Ability.TimelineAbility.TimelineAbility) };

    public static AbilityInfo PlayerAttack_Info = new AbilityInfo { Name = "PlayerAttack", AssetPath = "Assets/Demo/Resources/GAS_Setting/Config/GameplayAbilityLib/Player/PlayerAttack.asset",AbilityClassType = typeof(GAS.Runtime.Ability.Attack) };

    public static AbilityInfo PlayerDefend_Info = new AbilityInfo { Name = "PlayerDefend", AssetPath = "Assets/Demo/Resources/GAS_Setting/Config/GameplayAbilityLib/Player/PlayerDefend.asset",AbilityClassType = typeof(GAS.Runtime.Ability.Defend) };

    public static AbilityInfo PlayerDodge_Info = new AbilityInfo { Name = "PlayerDodge", AssetPath = "Assets/Demo/Resources/GAS_Setting/Config/GameplayAbilityLib/Player/PlayerDodge.asset",AbilityClassType = typeof(GAS.Runtime.Ability.Dodge) };

    public static AbilityInfo TESTAbility_Info = new AbilityInfo { Name = "TESTAbility", AssetPath = "Assets/Demo/Resources/GAS_Setting/Config/GameplayAbilityLib/TestTimelineAbility.asset",AbilityClassType = typeof(GAS.Runtime.Ability.TimelineAbility.TimelineAbility) };

  public static Dictionary<string, AbilityInfo> AbilityMap = new Dictionary<string, AbilityInfo>
  {
      ["Jump"] = Jump_Info,
      ["Move"] = Move_Info,
      ["DodgeStep"] = DodgeStep_Info,
      ["PlayerAttack"] = PlayerAttack_Info,
      ["PlayerDefend"] = PlayerDefend_Info,
      ["PlayerDodge"] = PlayerDodge_Info,
      ["TESTAbility"] = TESTAbility_Info,
  };
  }
}

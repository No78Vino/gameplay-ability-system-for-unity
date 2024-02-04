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

  public static Dictionary<string, AbilityInfo> AbilityMap = new Dictionary<string, AbilityInfo>
  {
      ["Jump"] = Jump_Info,
      ["Move"] = Move_Info,
  };
  }
}

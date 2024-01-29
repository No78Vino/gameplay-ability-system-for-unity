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

    public static AbilityInfo Attack_Info = new AbilityInfo { Name = "Attack", AssetPath = "Assets/Demo/Resources/GAS_Setting/Config/GameplayAbilityLib/Player/Attack.asset",AbilityClassType = typeof(Demo.Script.MyAbilitySystem.Ability.AbilityMove) };

    public static AbilityInfo Test_Info = new AbilityInfo { Name = "Test", AssetPath = "Assets/Demo/Resources/GAS_Setting/Config/GameplayAbilityLib/Test.asset",AbilityClassType = typeof(Demo.Script.MyAbilitySystem.Ability.AbilityMove) };

  public static Dictionary<string, AbilityInfo> AbilityMap = new Dictionary<string, AbilityInfo>
  {
      ["Attack"] = Attack_Info,
      ["Test"] = Test_Info,
  };
  }
}

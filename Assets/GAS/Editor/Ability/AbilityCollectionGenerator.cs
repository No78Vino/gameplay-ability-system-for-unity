#if UNITY_EDITOR
namespace GAS.Editor.Ability
{
    using GAS.Runtime.Ability;
    using UnityEditor;
    using System;
    using System.IO;
    using GAS.Core;
    using GAS.Editor.GameplayAbilitySystem;
    using UnityEngine;
    
    public class AbilityCollectionGenerator
    {
        public static void Gen()
        {
            string pathWithoutAssets = Application.dataPath.Substring(0, Application.dataPath.Length - 6);
            var filePath = $"{pathWithoutAssets}/{GASSettingAsset.CodeGenPath}/{GasDefine.GAS_ABILITY_COLLECTION_CSHARP_SCRIPT_NAME}";
            string loadMethodCodeString = GASSettingAsset.Setting.StringCodeOfLoadAbilityAsset;
            GenerateAbilityCollection(loadMethodCodeString,filePath);
        }

        private static void GenerateAbilityCollection(string loadMethodCodeString,string filePath)
        {
            using var writer = new StreamWriter(filePath);
            writer.WriteLine("///////////////////////////////////");
            writer.WriteLine("//// This is a generated file. ////");
            writer.WriteLine("////     Do not modify it.     ////");
            writer.WriteLine("///////////////////////////////////");
            writer.WriteLine("namespace GAS.Runtime.Ability");
            writer.WriteLine("{");
            writer.WriteLine("  public static class AbilityCollection");
            writer.WriteLine("  {");
            
            writer.WriteLine("      public struct AbilityInfo");
            writer.WriteLine("      {");
            writer.WriteLine("          public string Name;");
            writer.WriteLine("          public AbilityAsset Asset;");
            writer.WriteLine("      }");
            writer.WriteLine("");
            
            var abilityAssets = EditorUtil.FindAssetsByType<AbilityAsset>(GASSettingAsset.GameplayAbilityLibPath);
            
            foreach (var ability in abilityAssets)
            {
                // public static AbilityInfo Attack_Info = new AbilityInfo { Name = "Attack", Asset = Framework.Utilities.AssetUtil.LoadAsset<AbilityAsset>("ssss") };
                // public static GAS.Ability.Attack Attack = new GAS.Ability.Attack(Attack_Info.Asset);
                var validName = ability.UniqueName;
                var path = AssetDatabase.GetAssetPath(ability);
                string loadAbilityAssetCode = string.Format(loadMethodCodeString, path);
                writer.WriteLine(
                    $"    public static AbilityInfo {validName}_Info = new AbilityInfo {{ Name = \"{validName}\", Asset = {loadAbilityAssetCode}}};");
                writer.WriteLine($"    public static {ability.InstanceAbilityClassFullName} {validName} = new {ability.InstanceAbilityClassFullName}({validName}_Info.Asset);");
                writer.WriteLine("");
            }
            
            writer.WriteLine("  }");
            writer.WriteLine("}");
            
            Console.WriteLine($"Generated GameplayTagSumCollection at path: {filePath}");
        }
    }
}
#endif
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
            writer.WriteLine("          public string AssetPath;");
            // writer.WriteLine("          public AbilityAsset Asset()");
            // writer.WriteLine("          {");
            // string loadAbilityAssetCode = string.Format(loadMethodCodeString, "AssetPath");
            // writer.WriteLine($"              return {loadAbilityAssetCode};");
            // writer.WriteLine("          }");
            writer.WriteLine("      }");
            writer.WriteLine("");
            
            var abilityAssets = EditorUtil.FindAssetsByType<AbilityAsset>(GASSettingAsset.GameplayAbilityLibPath);
            
            foreach (var ability in abilityAssets)
            {
                var validName = ability.UniqueName;
                var path = AssetDatabase.GetAssetPath(ability);
                writer.WriteLine(
                    $"    public static AbilityInfo {validName}_Info = new AbilityInfo {{ Name = \"{validName}\", AssetPath = \"{path}\"}};");
                // writer.WriteLine($"    private static {ability.InstanceAbilityClassFullName} _{validName};");
                // writer.WriteLine($"    public static {ability.InstanceAbilityClassFullName} {validName}()");
                // writer.WriteLine("    {");
                // writer.WriteLine($"        if (_{validName} == null) _{validName} = new {ability.InstanceAbilityClassFullName}({validName}_Info.Asset());");
                // writer.WriteLine($"        return _{validName};");
                // writer.WriteLine("    }");
                writer.WriteLine("");
            }
            
            writer.WriteLine("  }");
            writer.WriteLine("}");
            
            Console.WriteLine($"Generated GameplayTagSumCollection at path: {filePath}");
        }
    }
}
#endif
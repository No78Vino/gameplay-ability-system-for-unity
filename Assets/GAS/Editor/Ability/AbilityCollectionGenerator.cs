using GAS.Runtime.Ability;

#if UNITY_EDITOR
namespace GAS.Editor.Ability
{
    using System.Collections.Generic;
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
            GenerateAbilityCollection(filePath);
        }

        private static void GenerateAbilityCollection(string filePath)
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
            
            var abilityAssets = EditorUtil.FindAssetsByType<AbilityAsset>(GASSettingAsset.GameplayAbilityLibPath);
            
            foreach (var ability in abilityAssets)
            {
                var validName = EditorUtil.MakeValidIdentifier(ability.UniqueName);
                writer.WriteLine(
                    $"    public const string {validName} = \"{validName}\";");
            }
            
            writer.WriteLine("  }");
            writer.WriteLine("}");
            
            //Console.WriteLine($"Generated GameplayTagSumCollection at path: {filePath}");
        }
    }
}
#endif
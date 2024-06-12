using System;
using System.IO;
using System.Linq;
using GAS.Runtime;
using UnityEditor;
using UnityEngine;

namespace GAS.Editor
{
    public class AbilityCollectionGenerator
    {
        public static void Gen()
        {
            string pathWithoutAssets = Application.dataPath.Substring(0, Application.dataPath.Length - 6);
            var filePath =
                $"{pathWithoutAssets}/{GASSettingAsset.CodeGenPath}/{GasDefine.GAS_ABILITY_LIB_CSHARP_SCRIPT_NAME}";
            GenerateAbilityCollection(filePath);
        }

        private static void GenerateAbilityCollection(string filePath)
        {
            using var writer = new IndentedWriter(new StreamWriter(filePath));
            writer.WriteLine("///////////////////////////////////");
            writer.WriteLine("//// This is a generated file. ////");
            writer.WriteLine("////     Do not modify it.     ////");
            writer.WriteLine("///////////////////////////////////");

            writer.WriteLine("");

            writer.WriteLine("using System;");
            writer.WriteLine("using System.Collections.Generic;");

            writer.WriteLine("");

            writer.WriteLine("namespace GAS.Runtime");
            writer.WriteLine("{");
            writer.Indent++;
            {
                writer.WriteLine("public static class GAbilityLib");
                writer.WriteLine("{");
                writer.Indent++;
                {
                    writer.WriteLine("public struct AbilityInfo");
                    writer.WriteLine("{");
                    writer.Indent++;
                    {
                        writer.WriteLine("public string Name;");
                        writer.WriteLine("public string AssetPath;");
                        writer.WriteLine("public Type AbilityClassType;");
                        //writer.WriteLine("public AbilityAsset Asset()");
                        // writer.WriteLine("{");
                        // writer.Indent++;
                        // {
                        //     string loadAbilityAssetCode = string.Format(loadMethodCodeString, "AssetPath");
                        //     writer.WriteLine($"return {loadAbilityAssetCode};");
                        // }
                        // writer.Indent--;
                        // writer.WriteLine("}");
                    }
                    writer.Indent--;
                    writer.WriteLine("}");

                    writer.WriteLine("");

                    var abilityAssets = EditorUtil
                        .FindAssetsByType<AbilityAsset>(GASSettingAsset.GameplayAbilityLibPath)
                        .OrderBy(x => x.UniqueName)
                        .ThenBy(x => x.name)
                        .ToArray();

                    foreach (var ability in abilityAssets)
                    {
                        var path = AssetDatabase.GetAssetPath(ability);
#if true
                        writer.WriteLine(
                            $"public static AbilityInfo {ability.UniqueName} = " +
                            $"new AbilityInfo {{ " +
                            $"Name = \"{ability.UniqueName}\", " +
                            $"AssetPath = \"{path}\"," +
                            $"AbilityClassType = typeof({ability.InstanceAbilityClassFullName}) }};");
#else
                          writer.WriteLine($"public static AbilityInfo {ability.UniqueName} = new AbilityInfo");
                        writer.WriteLine("{");
                        writer.Indent++;
                        {
                            writer.WriteLine($"Name = \"{ability.UniqueName}\",");
                            writer.WriteLine($"AssetPath = \"{path}\",");
                            writer.WriteLine($"AbilityClassType = typeof({ability.InstanceAbilityClassFullName})");
                        }
                        writer.Indent--;
                        writer.WriteLine("};");
#endif
                        // writer.WriteLine($"private static {ability.InstanceAbilityClassFullName} _{validName};");
                        // writer.WriteLine($"public static {ability.InstanceAbilityClassFullName} {validName}()");
                        // writer.WriteLine("{");
                        // writer.Indent++;
                        // {
                        //     writer.WriteLine($"if (_{validName} == null) _{validName} = new {ability.InstanceAbilityClassFullName}({validName}_Info.Asset());");
                        //     writer.Indent++;
                        //     {
                        //         writer.WriteLine($"return _{validName};");
                        //     }
                        //     writer.Indent--;
                        // }
                        // writer.Indent--;
                        // writer.WriteLine("}");

                        writer.WriteLine("");
                    }

                    writer.WriteLine("");

                    writer.WriteLine(
                        "public static Dictionary<string, AbilityInfo> AbilityMap = new Dictionary<string, AbilityInfo>");
                    writer.WriteLine("{");
                    writer.Indent++;
                    {
                        foreach (var ability in abilityAssets)
                        {
                            writer.WriteLine($"[\"{ability.UniqueName}\"] = {ability.UniqueName},");
                        }
                    }
                    writer.Indent--;
                    writer.WriteLine("};");
                }
                writer.Indent--;
                writer.WriteLine("}");
            }
            writer.Indent--;
            writer.Write("}");

            Console.WriteLine($"Generated GTagLib at path: {filePath}");
        }
    }
}
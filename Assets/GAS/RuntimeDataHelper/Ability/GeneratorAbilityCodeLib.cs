using System.IO;
using GAS.Editor;
using GAS.RuntimeDataHelper.Helper;
using UnityEditor;
using UnityEngine;

namespace GAS.RuntimeDataHelper.Ability
{
    /// <summary>
    /// 能力代码库脚本生成器
    /// </summary>
    public static class GeneratorAbilityCodeLib
    {
        [MenuItem("EX-GAS/CodeGenerate/AbilityCodeLib")]
        public static void Gen()
        {
            string pathWithoutAssets = Application.dataPath.Substring(0, Application.dataPath.Length - 6);
            var filePath =
                $"{pathWithoutAssets}/{GASSettingAsset.CodeGenPath}/{GasDefine.GAS_ABILITY_CODE_LIB_CSHARP_SCRIPT_NAME}";
            GenerateAbilityCodeLib(filePath);
        }

        public static void GenerateAbilityCodeLib(string filePath)
        {
            // public static class GEN_AbilityCode
            // {
            //     public const int DebugLog = 0;
            //     public const int Move = 1;
            //     public const int Jump = 2;
            //     public const int Attack = 3;
            //     public const int Attack_Monster = 4;
            //
            //     public static void LoadAbilityCode()
            //     {
            //         var AlDebugLog = typeof(GAS.RuntimeWithECS.Ability.Component.CommonAbilityLogic.ALDebugLog);
            //         GAS.RuntimeWithECS.Ability.AbilityHelper.RegisterAbilityLogic(AlDebugLog.FullName, AlDebugLog);
            //     }
            // }

            using var writer = new IndentedWriter(new StreamWriter(filePath));
            writer.WriteLine("///////////////////////////////////");
            writer.WriteLine("//// This is a generated file. ////");
            writer.WriteLine("////     Do not modify it.     ////");
            writer.WriteLine("///////////////////////////////////");

            writer.WriteLine("");
            
            writer.WriteLine("using System;");
            writer.WriteLine("using System.Collections.Generic;");
            
            writer.WriteLine("namespace GAS.Runtime");
            writer.WriteLine("{");
            
            writer.Indent++;
            {
                writer.WriteLine("public static class GEN_AbilityCode");
                writer.WriteLine("{");
                writer.Indent++;
                {
                    var allAbilityAsset = EXEditorHelper.GetAllAbilityConfigAssets();
                    foreach (var abilityConfigAsset in allAbilityAsset)
                    {
                        var abilityName = abilityConfigAsset.ConfAssetAbilityBaseInfo.name;
                        var hashCode = abilityConfigAsset.ConfAssetAbilityBaseInfo.Code;
                        writer.WriteLine($"public const int ABILITY_{abilityName} = {hashCode};");
                    }

                    writer.WriteLine("");
                    writer.WriteLine("public static void LoadAbilityCode()");
                    writer.WriteLine("{");
                    writer.Indent++;
                    {
                        var subTypes = EXEditorHelper.GetCachedAbilityLogicTypes();
                        foreach (var subType in subTypes)
                        {
                            var typeFullName = subType.FullName;
                            var shortTypeName = subType.Name;
                            writer.WriteLine($"var {shortTypeName} = typeof({typeFullName});");
                            writer.WriteLine($"GAS.RuntimeWithECS.Ability.AbilityHelper.RegisterAbilityLogic({shortTypeName}.FullName, {shortTypeName});");
                        }
                    }
                    writer.Indent--;
                    writer.WriteLine("}");
                }
                writer.Indent--;
                writer.WriteLine("}");
            }
            
            writer.WriteLine("}");
        }
    }
}
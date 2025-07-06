using System;
using System.IO;
using System.Linq;
using GAS.RuntimeDataHelper.Helper;
using UnityEditor;
using UnityEngine;

namespace GAS.Editor
{
    public static class GeneratorMmcType
    {
        [MenuItem("EXTool/EX-GAS/CodeGenerate/MmcType")]
        public static void Gen()
        {
            string pathWithoutAssets = Application.dataPath.Substring(0, Application.dataPath.Length - 6);
            var filePath =
                $"{pathWithoutAssets}/{GASSettingAsset.CodeGenPath}/{GasDefine.GAS_MMC_CODE_CSHARP_SCRIPT_NAME}";
            GenerateMmcCodeLib(filePath);
        }

        public static void GenerateMmcCodeLib(string filePath)
        {
            //         namespace GAS.Runtime
            //         {
            //     public static class GEN_MmcCode
            //     {
            //         public const string MMC_None = "2";
            //         public const string MMC_FloatScale = "2";
            //
            //         public static void LoadMmcType()
            //         {
            //             var none = typeof(RuntimeWithECS.Modifier.CommonUsage.MMCNone);
            //             MmcHelper.RegisterMmc(MMC_FloatScale, none);
            //
            //             var scalableFloat = typeof(RuntimeWithECS.Modifier.CommonUsage.MMCScalableFloat);
            //             MmcHelper.RegisterMmc(MMC_FloatScale, scalableFloat);
            //         }
            //     }
            // }
            using var writer = new IndentedWriter(new StreamWriter(filePath));
            writer.WriteLine("///////////////////////////////////");
            writer.WriteLine("//// This is a generated file. ////");
            writer.WriteLine("////     Do not modify it.     ////");
            writer.WriteLine("///////////////////////////////////");

            writer.WriteLine("");
            
            writer.WriteLine("namespace GAS.Runtime");
            writer.WriteLine("{");
            writer.Indent++;
            {
                writer.WriteLine("public static class GEN_MmcCode");
                writer.WriteLine("{");
                writer.Indent++;
                {
                    var allMmc = EditGameplayEffectHelper.GetCachedMmcTypes();
                    var mmcTypes = allMmc as Type[] ?? allMmc.ToArray();
                    foreach (var mmcType in mmcTypes)
                    {
                        var mmcName = mmcType.Name;
                        var fullName = mmcType.FullName;
                        writer.WriteLine($"public const string MMC_{mmcName} = \"{fullName}\";");
                    }

                    writer.WriteLine("");
                    writer.WriteLine("public static void LoadMmcType()");
                    writer.WriteLine("{");
                    writer.Indent++;
                    {
                        foreach (var mmcType in mmcTypes)
                        {
                            var mmcName = mmcType.Name;
                            var typeFullName = mmcType.FullName;
                            writer.WriteLine($"var {mmcName} = typeof({typeFullName});");
                            writer.WriteLine($"MmcHelper.RegisterMmc(MMC_{mmcName}, {mmcName});");
                        }
                    }
                    writer.Indent--;
                    writer.WriteLine("}");
                }
                writer.Indent--;
                writer.WriteLine("}");
            }
            writer.Indent--;
            writer.WriteLine("}");
            
            Console.WriteLine($"Generated MmcCode at path: {filePath}");
            AssetDatabase.Refresh();
        }
    }
}
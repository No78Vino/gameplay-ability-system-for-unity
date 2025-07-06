using System;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace GAS.Editor
{
    public static class GeneratorCueType
    {
        [MenuItem("EXTool/EX-GAS/CodeGenerate/CueType")]
        public static void Gen()
        {
            string pathWithoutAssets = Application.dataPath.Substring(0, Application.dataPath.Length - 6);
            var filePath =
                $"{pathWithoutAssets}/{GASSettingAsset.CodeGenPath}/{GasDefine.GAS_CUE_CODE_CSHARP_SCRIPT_NAME}";
            GenerateCueCodeLib(filePath);
        }
        
        public static void GenerateCueCodeLib(string filePath)
        {
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
                writer.WriteLine("public static class GEN_CueCode");
                writer.WriteLine("{");
                writer.Indent++;
                {
                    var allCue = EditCueHelper.GetCachedCueTypes();
                    var cueTypes = allCue as Type[] ?? allCue.ToArray();
                    foreach (var cueType in cueTypes)
                    {
                        var cueName = cueType.Name;
                        var fullName = cueType.FullName;
                        writer.WriteLine($"public const string CUE_{cueName} = \"{fullName}\";");
                    }

                    writer.WriteLine("");
                    writer.WriteLine("public static void LoadCueType()");
                    writer.WriteLine("{");
                    writer.Indent++;
                    {
                        foreach (var cueType in cueTypes)
                        {
                            var cueName = cueType.Name;
                            var typeFullName = cueType.FullName;
                            writer.WriteLine($"var {cueName} = typeof({typeFullName});");
                            writer.WriteLine($"CueHelper.RegisterCue(CUE_{cueName}, {cueName});");
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
            
            Console.WriteLine($"Generated CueCode at path: {filePath}");
            AssetDatabase.Refresh();
        }
    }
}
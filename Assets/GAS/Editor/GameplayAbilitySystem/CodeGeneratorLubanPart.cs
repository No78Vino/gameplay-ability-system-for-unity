using System.IO;

namespace GAS.Editor
{
    public static class CodeGeneratorLubanPart
    {
        // TODO
        public static void GenerateLubanExtension()
        {
            var setting = GASSettingAsset.LoadOrCreate();
            var filePath = setting.PathOfCodeLubanExtesion;
            using var writer = new IndentedWriter(new StreamWriter(filePath));
            writer.WriteLine("///////////////////////////////////");
            writer.WriteLine("//// This is a generated file. ////");
            writer.WriteLine("////     Do not modify it.     ////");
            writer.WriteLine("///////////////////////////////////");

            writer.WriteLine("");

            writer.WriteLine("using System;");
            writer.WriteLine("using System.Collections.Generic;");
            writer.WriteLine("using cfg;");

            writer.WriteLine("namespace GAS.Runtime");
            writer.WriteLine("{");
            writer.Indent++;
            {
                writer.WriteLine("public static class XLuban");
                writer.WriteLine("{");
                writer.Indent++;
                {
                    writer.WriteLine(
                        $"public const string GAME_CONF_DIR = \"{GASSettingAsset.Instance.TableOutpuPath}\";");
                    writer.WriteLine("private static Tables _tables;");
                    writer.WriteLine("public static Tables Tables");
                    writer.WriteLine("{");
                    writer.Indent++;
                    {
                        writer.WriteLine("get");
                        writer.WriteLine("{");
                        writer.Indent++;
                        writer.WriteLine("if (_tables == null) LoadTables();");
                        writer.WriteLine("return _tables;");
                        writer.Indent--;
                        writer.WriteLine("}");
                    }
                    writer.Indent--;
                    writer.WriteLine("}");

                    // public static void LoadTables()
                    // {
                    //     if (_tables != null) return; // Already loaded
                    //     _tables = new Tables(file => JSON.Parse(File.ReadAllText($"{GAME_CONF_DIR}/{file}.json")));
                    // }




                    // var allAbilityNames = GetAbilityNames();
                    // foreach (var kv in allAbilityNames)
                    // {
                    //     var abilityName = kv.Value;
                    //     var code = kv.Key;
                    //     writer.WriteLine($"public const int ABILITY_{abilityName} = {code};");
                    // }
                    //
                    // writer.WriteLine("");
                    // writer.WriteLine("public static void LoadAbilityCode()");
                    // writer.WriteLine("{");
                    // writer.Indent++;
                    // {
                    //     var subTypes = EXEditorHelper.GetCachedAbilityLogicTypes();
                    //     foreach (var subType in subTypes)
                    //     {
                    //         var typeFullName = subType.FullName;
                    //         var shortTypeName = subType.Name;
                    //         var abilityParamType =
                    //             EXEditorHelper.GetCachedAbilityLogicToAbilityParamTypeMap()[typeFullName];
                    //         var abilityParamTypeFullName = abilityParamType.FullName;
                    //         writer.WriteLine($"var {shortTypeName} = typeof({typeFullName});");
                    //         writer.WriteLine(
                    //             $"GAS.RuntimeWithECS.AbilityHelper.RegisterAbilityLogic({shortTypeName}.FullName, {shortTypeName},typeof({abilityParamTypeFullName}));");
                    //     }
                    // }
                    // writer.Indent--;
                    // writer.WriteLine("}");
                }
                writer.Indent--;
                writer.WriteLine("}");
            }
            writer.Indent--;
            writer.WriteLine("}");
        }
    }
}
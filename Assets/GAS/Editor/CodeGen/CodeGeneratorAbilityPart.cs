using System.Collections.Generic;
using System.IO;
using GAS.Editor;
using OfficeOpenXml;

namespace GAS.Editor
{
    public static class CodeGeneratorAbilityPart
    {
        public static void GenerateAbilityCode()
        {
            var setting = GASSettingAsset.LoadOrCreate();
            var filePath = setting.PathOfCodeAbility;
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
                writer.WriteLine("public static class XAbility");
                writer.WriteLine("{");
                writer.Indent++;
                {
                    var allAbilityNames = GetAbilityNames();
                    foreach (var kv in allAbilityNames)
                    {
                        var abilityName = kv.Value;
                        var code = kv.Key;
                        writer.WriteLine($"public const int ABILITY_{abilityName} = {code};");
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
                            var abilityParamType =
                                EXEditorHelper.GetCachedAbilityLogicToAbilityParamTypeMap()[typeFullName];
                            var abilityParamTypeFullName = abilityParamType.FullName;
                            writer.WriteLine($"var {shortTypeName} = typeof({typeFullName});");
                            writer.WriteLine(
                                $"GAS.RuntimeWithECS.AbilityHelper.RegisterAbilityLogic({shortTypeName}.FullName, {shortTypeName},typeof({abilityParamTypeFullName}));");
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
        }
        
        
        private static Dictionary<int,string> GetAbilityNames()
        {
            Dictionary<int, string> result = new Dictionary<int, string>();
            var excelFilePath = GASSettingAsset.Instance.PathOfExcelAbility;
            var xlsxFileInfo = new FileInfo(excelFilePath);
            using (var package = new ExcelPackage(xlsxFileInfo))
            {
                var worksheet = package.Workbook.Worksheets[1];
               
                var safeCnt = 99999;
                var row = 4;
                while (safeCnt > 0 && worksheet.Cells[row, 2].Value != null)
                {
                    safeCnt--;
                   
                    var id = int.Parse(worksheet.Cells[row, 2].Value.ToString());
                    result.Add(id,worksheet.Cells[row, 3].Value.ToString());
                   
                    row++;
                }
            }

            return result;
        }
    }
}
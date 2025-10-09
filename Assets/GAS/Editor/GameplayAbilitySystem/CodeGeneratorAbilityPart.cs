using System.Collections.Generic;
using System.IO;
using GAS.RuntimeDataHelper.Helper;
using OfficeOpenXml;
using SimpleJSON;
using Sirenix.OdinInspector;
using Unity.Entities;

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
        
        #region 可视化读写编辑 Ability 配置xlsx文件
        
        private static Dictionary<int,string> GetAbilityNames()
        {
            Dictionary<int, string> result = new Dictionary<int, string>();
            var excelFilePath = GASSettingAsset.Instance.PathOfExcelAbility;
            var xlsxFileInfo = new FileInfo(excelFilePath);
            using (var package = new ExcelPackage(xlsxFileInfo))
            {
                var worksheet = package.Workbook.Worksheets[1];
                // 注册表头
                //var headerMap = new Dictionary<string, int>();
                // for (var i = 0; i < 500; i++)
                // {
                //     if (worksheet.Cells[1, i + 1].Value == null) continue;
                //     //var header = worksheet.Cells[1, i + 1].Value.ToString();
                //     // 去除格式后缀（即#之后的内容）
                //     //header = header.Split('#')[0];
                //     //if (string.IsNullOrEmpty(header)) continue;
                //     //headerMap[header] = i + 1; // Excel列从1开始
                // }

                // 读取数据行,从第4行开始，第二列为key，即id。
                // 以第2列是否有值为结束标志
                // _data = new Dictionary<int, Dictionary<int, object>>();
                // _abilityLogicParameter = new Dictionary<int, List<object>>();
                // _idToRowMap = new Dictionary<int, int>();
                var safeCnt = 99999;
                var row = 4;
                while (safeCnt > 0 && worksheet.Cells[row, 2].Value != null)
                {
                    safeCnt--;
                    //var rowData = new Dictionary<int, object>();
                    var id = int.Parse(worksheet.Cells[row, 2].Value.ToString());
                    result.Add(id,worksheet.Cells[row, 3].Value.ToString());
                    // foreach (var colIndex in _headerMap.Values)
                    //     rowData.Add(colIndex, worksheet.Cells[row, colIndex].Value);
                    //
                    // var parameterCol = _headerMap["abilityLogic"];
                    // var abilityParams = new List<object>();
                    // for (var i = parameterCol + 1; i < 51 + parameterCol; i++)
                    // {
                    //     var v = worksheet.Cells[row, i].Value;
                    //     rowData.Add(i, v);
                    //     abilityParams.Add(v);
                    // }
                    //
                    // _data.Add(id, rowData);
                    // _abilityLogicParameter.Add(id, abilityParams);
                    // _idToRowMap.Add(id, row);
                    row++;
                }
            }

            return result;
        }

        #endregion
    }
}
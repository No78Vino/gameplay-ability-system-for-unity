using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using GAS.Editor;
using UnityEditor;
using UnityEngine;

namespace GAS.RuntimeDataHelper.Attribute
{
    public static class GeneratorAttributeAndAttrSetCode
    {
        [MenuItem("EX-GAS/CodeGenerate/AttributeSetCode")]
        public static void Gen()
        {
            string pathWithoutAssets = Application.dataPath.Substring(0, Application.dataPath.Length - 6);
            var filePath =
                $"{pathWithoutAssets}/{GASSettingAsset.CodeGenPath}/{GasDefine.GAS_ATTRIBUTE_AND_ATTR_SET_CODE_CSHARP_SCRIPT_NAME}";

            GenerateCode(filePath);
        }

        private static void GenerateCode(string filePath)
        {
            using var writer = new IndentedWriter(new StreamWriter(filePath));

            writer.WriteLine("///////////////////////////////////");
            writer.WriteLine("//// This is a generated file. ////");
            writer.WriteLine("////     Do not modify it.     ////");
            writer.WriteLine("///////////////////////////////////");

            writer.WriteLine("");
            writer.WriteLine("using System.Collections.Generic;");
            writer.WriteLine("using GAS.RuntimeWithECS.Attribute;");
            writer.WriteLine("");

            writer.WriteLine("namespace GAS.Runtime");
            writer.WriteLine("{");
            writer.Indent++;
            {
                // 先生成Attribute的Code
                var attributeAsset = AttributeAsset.LoadOrCreate();
                var attributeInfos = (from t in attributeAsset.attributes
                    where !string.IsNullOrWhiteSpace(t.Name)
                    select new Tuple<int, string>(t.GetCode(), t.Name)).ToList();
                Dictionary<string,int> _attrCodeMap = new();
                foreach (var attr in attributeInfos) _attrCodeMap.Add(attr.Item2, attr.Item1);
                

                writer.WriteLine("public static class GEN_AttributeCode");
                writer.WriteLine("{");
                writer.Indent++;
                {
                    foreach (var attr in attributeInfos)
                    {
                        writer.WriteLine($"public const int {attr.Item2} = {attr.Item1};");
                    }
                }
                writer.Indent--;
                writer.WriteLine("}");
                
                // 再生成AttributeSet的Code
                var attributeSetAsset = AttributeSetAsset.LoadOrCreate();
                var attributeSetInfos = attributeSetAsset.AttributeSetConfigs;
                
                writer.WriteLine("public static class GEN_AttrSetCode");
                writer.WriteLine("{");
                writer.Indent++;
                {
                    foreach (var attrSet in attributeSetInfos)
                    {
                        writer.WriteLine($"public const int {attrSet.Name} = {attrSet.GetCode()};");
                    }
                    
                    foreach (var attrSet in attributeSetInfos)
                    {
                        writer.WriteLine(
                            $"public static NewAttributeSetConfig AS_{attrSet.Name} = new({attrSet.Name}, new AttributeBaseSetting[]");
                        writer.WriteLine("{");
                        writer.Indent++;
                        {
                            foreach (var attr in attrSet.Attributes)
                            {
                                // 初始值，钳制最小值，钳制最大值，最小值，最大值
                                writer.WriteLine(
                                    $"new(GEN_AttributeCode.{attr.attrName}," +
                                    $"{attr.ValueDefaultInit}," +
                                    $"{attr.ClampMin.ToString().ToLower()}," +
                                    $"{attr.ClampMax.ToString().ToLower()}," +
                                    $"{attr.ValueMin}f," +
                                    $"{attr.ValueMax}f),");
                            }
                        }
                        writer.Indent--;
                        writer.WriteLine("});");
                       
                        
                    }
                    writer.WriteLine("public static Dictionary<int,NewAttributeSetConfig> AttributeSetMap = new()");
                    writer.WriteLine("{");
                    writer.Indent++;
                    {
                        foreach (var attrSet in attributeSetInfos)
                        {
                            writer.WriteLine(
                                $"{{{attrSet.Name},AS_{attrSet.Name}}},");
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
using System;
using System.Collections.Generic;
using System.IO;
using GAS.Core;
using GAS.Runtime.AttributeSet;
using UnityEditor;
using UnityEngine;

namespace GAS.Editor.AttributeSet
{
    public static class AttributeSetClassGen
    {
        public static void Gen()
        {
            var asset = AssetDatabase.LoadAssetAtPath<AttributeSetAsset>(GasDefine.GAS_ATTRIBUTESET_ASSET_PATH);
            var filePath = $"{Application.dataPath}/{asset.AttributeSetClassGenPath}/{GasDefine.GAS_ATTRIBUTESET_CLASS_CSHARP_SCRIPT_NAME}";
            GenerateAttributeCollection(asset.AttributeSetConfigs, filePath);
        }

        private static void GenerateAttributeCollection(List<AttributeSetConfig> attributeSetConfigs, string filePath)
        {
            using var writer = new StreamWriter(filePath);
            writer.WriteLine("///////////////////////////////////");
            writer.WriteLine("//// This is a generated file. ////");
            writer.WriteLine("////     Do not modify it.     ////");
            writer.WriteLine("///////////////////////////////////");
            writer.WriteLine("");
            writer.WriteLine("using GAS.Runtime.Attribute;");
            writer.WriteLine("namespace GAS.Runtime.AttributeSet");
            writer.WriteLine("{");

            foreach (var attributeSet in attributeSetConfigs)
            {
                var validName = EditorUtil.MakeValidIdentifier(attributeSet.Name);
                writer.WriteLine($"public class AttrSet_{validName}:AttributeSet");
                writer.WriteLine("{");

                foreach (var attributeName in attributeSet.AttributeNames)
                {
                    string validAttrName = EditorUtil.MakeValidIdentifier(attributeName);
                    writer.WriteLine($"    private AttributeBase {validAttrName} = new AttributeBase(\"{attributeName}\");");
                    writer.WriteLine($"    public AttributeBase Get{validAttrName} => {validAttrName};");
                    writer.WriteLine($"    public void Init{validAttrName}(float value)");
                    writer.WriteLine("    {");
                    writer.WriteLine($"        {validAttrName}.SetBaseValue(value);");
                    writer.WriteLine($"        {validAttrName}.SetCurrentValue(value);");
                    writer.WriteLine("    }");
                    writer.WriteLine($"      public void SetCurrent{validAttrName}(float value)");
                    writer.WriteLine("    {");
                    writer.WriteLine($"        {validAttrName}.SetCurrentValue(value);");
                    writer.WriteLine("    }");
                    writer.WriteLine($"      public void SetBase{validAttrName}(float value)");
                    writer.WriteLine("    {");
                    writer.WriteLine($"        {validAttrName}.SetBaseValue(value);");
                    writer.WriteLine("    }");
                }
                
                writer.WriteLine("");
                
                writer.WriteLine("      public override AttributeBase this[string key]");
                writer.WriteLine("      {");
                writer.WriteLine("          get");
                writer.WriteLine("          {");
                writer.WriteLine("              switch (key)");
                writer.WriteLine("              {");
                foreach (var attributeName in attributeSet.AttributeNames)
                {
                    string validAttrName = EditorUtil.MakeValidIdentifier(attributeName);
                    writer.WriteLine($"                 case \"{validAttrName}\":");
                    writer.WriteLine($"                    return {validAttrName};");
                }
                writer.WriteLine("              }");
                writer.WriteLine("              return null;");
                writer.WriteLine("          }");
                writer.WriteLine("      }");
                
                writer.WriteLine("}");
            }
            
            writer.WriteLine("}");

            Console.WriteLine($"Generated Code Script at path: {filePath}");
        }
    }
}
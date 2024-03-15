
#if UNITY_EDITOR
namespace GAS.Editor
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using GAS;
    using Runtime;
    using UnityEditor;
    using UnityEngine;
    using Editor;

    
    public static class AttributeSetClassGen
    {
        public static void Gen()
        {
            var asset = AssetDatabase.LoadAssetAtPath<AttributeSetAsset>(GASSettingAsset.GAS_ATTRIBUTESET_ASSET_PATH);
            string pathWithoutAssets = Application.dataPath.Substring(0, Application.dataPath.Length - 6);
            var filePath = $"{pathWithoutAssets}/{GASSettingAsset.CodeGenPath}/{GasDefine.GAS_ATTRIBUTESET_LIB_CSHARP_SCRIPT_NAME}";
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
            writer.WriteLine("using System;");
            writer.WriteLine("using System.Collections.Generic;");
            writer.WriteLine("namespace GAS.Runtime");
            writer.WriteLine("{");

            foreach (var attributeSet in attributeSetConfigs)
            {
                var validName = EditorUtil.MakeValidIdentifier(attributeSet.Name);
                writer.WriteLine($"public class AS_{validName}:AttributeSet");
                writer.WriteLine("{");

                foreach (var attributeName in attributeSet.AttributeNames)
                {
                    string validAttrName = EditorUtil.MakeValidIdentifier(attributeName);
                    writer.WriteLine($"    private AttributeBase _{validAttrName} = new AttributeBase(\"AS_{validName}\",\"{attributeName}\");");
                    writer.WriteLine($"    public AttributeBase {validAttrName} => _{validAttrName};");
                    writer.WriteLine($"    public void Init{validAttrName}(float value)");
                    writer.WriteLine("    {");
                    writer.WriteLine($"        _{validAttrName}.SetBaseValue(value);");
                    writer.WriteLine($"        _{validAttrName}.SetCurrentValue(value);");
                    writer.WriteLine("    }");
                    writer.WriteLine($"      public void SetCurrent{validAttrName}(float value)");
                    writer.WriteLine("    {");
                    writer.WriteLine($"        _{validAttrName}.SetCurrentValue(value);");
                    writer.WriteLine("    }");
                    writer.WriteLine($"      public void SetBase{validAttrName}(float value)");
                    writer.WriteLine("    {");
                    writer.WriteLine($"        _{validAttrName}.SetBaseValue(value);");
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
                    writer.WriteLine($"                    return _{validAttrName};");
                }
                writer.WriteLine("              }");
                writer.WriteLine("              return null;");
                writer.WriteLine("          }");
                writer.WriteLine("      }");
                
                writer.WriteLine("");
                writer.WriteLine("      public override string[] AttributeNames { get; } =");
                writer.WriteLine("      {");
                foreach (var attributeName in attributeSet.AttributeNames)
                {
                    string validAttrName = EditorUtil.MakeValidIdentifier(attributeName);
                    writer.WriteLine($"          \"{validAttrName}\",");
                }
                writer.WriteLine("      };");
        
                writer.WriteLine("}");
            }
            
            writer.WriteLine($"public static class GAttrSetLib");
            writer.WriteLine("{");
            writer.WriteLine("    public static readonly Dictionary<string,Type> AttrSetTypeDict = new Dictionary<string, Type>()");
            writer.WriteLine("    {");
            foreach (var attributeSet in attributeSetConfigs)
            {
                var validName = EditorUtil.MakeValidIdentifier(attributeSet.Name);
                writer.WriteLine($"        {{\"{attributeSet.Name}\",typeof(AS_{validName})}},");
            }
            writer.WriteLine("    };");
            
            writer.WriteLine("    public static List<string> AttributeFullNames=new List<string>()");
            writer.WriteLine("    {");
            foreach (var attributeSet in attributeSetConfigs)
            {
                var validName = EditorUtil.MakeValidIdentifier(attributeSet.Name);
                foreach (var attributeName in attributeSet.AttributeNames)
                {
                    string validAttrName = EditorUtil.MakeValidIdentifier(attributeName);
                    writer.WriteLine($"        \"AS_{validName}.{validAttrName}\",");
                }
            }
            writer.WriteLine("      };");
            
            writer.WriteLine("}");
            
        
            writer.WriteLine("}");

            Console.WriteLine($"Generated Code Script at path: {filePath}");
        }
    }
}
#endif
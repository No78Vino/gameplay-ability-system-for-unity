#if UNITY_EDITOR
namespace GAS.Editor
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using UnityEditor;
    using UnityEngine;

    public static class AttributeSetClassGen
    {
        public static void Gen()
        {
            var attributeSetAsset = AttributeSetAsset.LoadOrCreate();

            var attributeAsset = AttributeAsset.LoadOrCreate();
            var attributeNames = (from t in attributeAsset.attributes where !string.IsNullOrWhiteSpace(t.Name) select t.Name)
                .ToList();

            // Check if AttributeSet contains attribute that is not defined in AttributeAsset
            foreach (var attributeSetConfig in attributeSetAsset.AttributeSetConfigs)
            {
                foreach (var attributeName in attributeSetConfig.AttributeNames)
                {
                    if (!attributeNames.Contains(attributeName))
                    {
                        var msg =
                            $"Invalid Attribute(\"{attributeName}\") in AttributeSet(\"{attributeSetConfig.Name}\"), \"{attributeName}\" is not defined in AttributeAsset!";
                        Debug.LogError(msg);
                        EditorUtility.DisplayDialog("Error", msg, "OK");
                        return;
                    }
                }
            }

            string pathWithoutAssets = Application.dataPath.Substring(0, Application.dataPath.Length - 6);
            var filePath =
                $"{pathWithoutAssets}/{GASSettingAsset.CodeGenPath}/{GasDefine.GAS_ATTRIBUTESET_LIB_CSHARP_SCRIPT_NAME}";
            GenerateAttributeCollection(attributeSetAsset.AttributeSetConfigs, filePath);
        }

        private static void GenerateAttributeCollection(List<AttributeSetConfig> attributeSetConfigs, string filePath)
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
                foreach (var attributeSet in attributeSetConfigs)
                {
                    var validName = EditorUtil.MakeValidIdentifier(attributeSet.Name);
                    writer.WriteLine($"public class AS_{validName} : AttributeSet");
                    writer.WriteLine("{");
                    writer.Indent++;
                    {
                        bool skippedFirst = false;
                        foreach (var attributeName in attributeSet.AttributeNames)
                        {
                            if (!skippedFirst) skippedFirst = true;
                            else writer.WriteLine("");

                            string validAttrName = EditorUtil.MakeValidIdentifier(attributeName);
                            writer.WriteLine($"#region {attributeName}");
                            writer.WriteLine("");
                            {
                                writer.WriteLine(
                                    $"private AttributeBase _{validAttrName} = new AttributeBase(\"AS_{validName}\", \"{attributeName}\");");

                                writer.WriteLine("");

                                writer.WriteLine($"public AttributeBase {validAttrName} => _{validAttrName};");

                                writer.WriteLine("");

                                writer.WriteLine($"public void Init{validAttrName}(float value)");
                                writer.WriteLine("{");
                                writer.Indent++;
                                {
                                    writer.WriteLine($"_{validAttrName}.SetBaseValue(value);");
                                    writer.WriteLine($"_{validAttrName}.SetCurrentValue(value);");
                                }
                                writer.Indent--;
                                writer.WriteLine("}");

                                writer.WriteLine("");

                                writer.WriteLine($"public void SetCurrent{validAttrName}(float value)");
                                writer.WriteLine("{");
                                writer.Indent++;
                                {
                                    writer.WriteLine($"_{validAttrName}.SetCurrentValue(value);");
                                }
                                writer.Indent--;
                                writer.WriteLine("}");

                                writer.WriteLine("");

                                writer.WriteLine($"public void SetBase{validAttrName}(float value)");
                                writer.WriteLine("{");
                                writer.Indent++;
                                {
                                    writer.WriteLine($"_{validAttrName}.SetBaseValue(value);");
                                }
                                writer.Indent--;
                                writer.WriteLine("}");
                            }
                            writer.WriteLine("");
                            writer.WriteLine($"#endregion {attributeName}");
                        }

                        writer.WriteLine("");

                        writer.WriteLine("public override AttributeBase this[string key]");
                        writer.WriteLine("{");
                        writer.Indent++;
                        {
                            writer.WriteLine("get");
                            writer.WriteLine("{");
                            writer.Indent++;
                            {
                                writer.WriteLine("switch (key)");
                                writer.WriteLine("{");
                                writer.Indent++;
                                {
                                    foreach (var attributeName in attributeSet.AttributeNames)
                                    {
                                        string validAttrName = EditorUtil.MakeValidIdentifier(attributeName);
                                        writer.WriteLine($"case \"{validAttrName}\":");
                                        writer.Indent++;
                                        {
                                            writer.WriteLine($"return _{validAttrName};");
                                        }
                                        writer.Indent--;
                                    }
                                }
                                writer.Indent--;
                                writer.WriteLine("}");
                                writer.WriteLine("");
                                writer.WriteLine("return null;");
                            }
                            writer.Indent--;
                            writer.WriteLine("}");
                        }
                        writer.Indent--;
                        writer.WriteLine("}");

                        writer.WriteLine("");

                        writer.WriteLine("public override string[] AttributeNames { get; } =");
                        writer.WriteLine("{");
                        writer.Indent++;
                        {
                            foreach (var attributeName in attributeSet.AttributeNames)
                            {
                                string validAttrName = EditorUtil.MakeValidIdentifier(attributeName);
                                writer.WriteLine($"\"{validAttrName}\",");
                            }
                        }
                        writer.Indent--;
                        writer.WriteLine("};");

                        writer.WriteLine("");
                        writer.WriteLine("public override void SetOwner(AbilitySystemComponent owner)");
                        writer.WriteLine("{");
                        writer.Indent++;
                        {
                            writer.WriteLine("_owner = owner;");
                            foreach (var attributeName in attributeSet.AttributeNames)
                            {
                                string validAttrName = EditorUtil.MakeValidIdentifier(attributeName);
                                writer.WriteLine($"_{validAttrName}.SetOwner(owner);");
                            }
                        }
                        writer.Indent--;
                        writer.WriteLine("}");
                    }
                    writer.Indent--;
                    writer.WriteLine("}");
                }

                writer.WriteLine("");

                writer.WriteLine($"public static class GAttrSetLib");
                writer.WriteLine("{");
                writer.Indent++;
                {
                    writer.WriteLine(
                        "public static readonly Dictionary<string, Type> AttrSetTypeDict = new Dictionary<string, Type>()");
                    writer.WriteLine("{");
                    writer.Indent++;
                    {
                        foreach (var attributeSet in attributeSetConfigs)
                        {
                            var validName = EditorUtil.MakeValidIdentifier(attributeSet.Name);
                            writer.WriteLine($"{{ \"{attributeSet.Name}\", typeof(AS_{validName}) }},");
                        }
                    }
                    writer.Indent--;
                    writer.WriteLine("};");
        
                    writer.WriteLine(
                        "public static readonly Dictionary<Type,string> TypeToName = new Dictionary<Type,string>");
                    writer.WriteLine("{");
                    writer.Indent++;
                    {
                        foreach (var attributeSet in attributeSetConfigs)
                        {
                            var validName = EditorUtil.MakeValidIdentifier(attributeSet.Name);
                            writer.WriteLine($"{{  typeof(AS_{validName}),nameof(AS_{validName}) }},");
                        }
                    }
                    writer.Indent--;
                    writer.WriteLine("};");
                    
                    writer.WriteLine("");

                    writer.WriteLine("public static List<string> AttributeFullNames = new List<string>()");
                    writer.WriteLine("{");
                    writer.Indent++;
                    {
                        foreach (var attributeSet in attributeSetConfigs)
                        {
                            var validName = EditorUtil.MakeValidIdentifier(attributeSet.Name);
                            foreach (var attributeName in attributeSet.AttributeNames)
                            {
                                string validAttrName = EditorUtil.MakeValidIdentifier(attributeName);
                                writer.WriteLine($"\"AS_{validName}.{validAttrName}\",");
                            }
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

            Console.WriteLine($"Generated Code Script at path: {filePath}");
        }
    }
}
#endif
using GAS.Runtime;

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
            var attributeNames =
                (from t in attributeAsset.attributes where !string.IsNullOrWhiteSpace(t.Name) select t.Name)
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

            var invalidAttributes = attributeAsset.attributes.Where(x =>
                x.CalculateMode is CalculateMode.MinValueOnly or CalculateMode.MaxValueOnly &&
                x.SupportedOperation != SupportedOperation.Override).ToArray();
            if (invalidAttributes.Length > 0)
            {
                var msg =
                    $"计算模式为\"取最小值\"或\"取最大值\"的属性只能支持\"替换\"操作：\n{string.Join("\n", invalidAttributes.Select(x => $"\"{x.Name}\""))}";
                Debug.LogError(msg.Replace("\n", ", "));
                EditorUtility.DisplayDialog("Error", msg, "OK");
                return;
            }


            string pathWithoutAssets = Application.dataPath.Substring(0, Application.dataPath.Length - 6);
            var filePath =
                $"{pathWithoutAssets}/{GASSettingAsset.CodeGenPath}/{GasDefine.GAS_ATTRIBUTESET_LIB_CSHARP_SCRIPT_NAME}";
            GenerateAttributeCollection(attributeSetAsset.AttributeSetConfigs, attributeAsset, filePath);
        }

        private static void GenerateAttributeCollection(List<AttributeSetConfig> attributeSetConfigs,
            AttributeAsset attributeAsset, string filePath)
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
                foreach (var attributeSetConfig in attributeSetConfigs.OrderBy(x => x.Name))
                {
                    var validName = EditorUtil.MakeValidIdentifier(attributeSetConfig.Name);
                    writer.WriteLine($"public class AS_{validName} : AttributeSet");
                    writer.WriteLine("{");
                    writer.Indent++;
                    {
                        bool skippedFirst = false;
                        foreach (var attributeName in attributeSetConfig.AttributeNames.OrderBy(x => x))
                        {
                            var attributeAccessor = attributeAsset.attributes.Find(x => x.Name == attributeName);

                            if (!skippedFirst) skippedFirst = true;
                            else writer.WriteLine("");

                            string validAttrName = EditorUtil.MakeValidIdentifier(attributeName);
                            writer.WriteLine($"#region {attributeName}");
                            writer.WriteLine("");
                            {
                                writer.WriteLine($"/// <summary>");
                                writer.WriteLine($"/// {attributeAccessor.Comment}");
                                writer.WriteLine($"/// </summary>");
                                writer.WriteLine(
                                    $"public AttributeBase {validAttrName} {{ get; }} = new (\"AS_{validName}\", \"{attributeName}\", {attributeAccessor.DefaultValue}f, CalculateMode.{attributeAccessor.CalculateMode}, (SupportedOperation){(byte)attributeAccessor.SupportedOperation}, {(attributeAccessor.LimitMinValue ? attributeAccessor.MinValue : "float.MinValue")}, {(attributeAccessor.LimitMaxValue ? attributeAccessor.MaxValue : "float.MaxValue")});");

                                writer.WriteLine("");

                                writer.WriteLine($"public void Init{validAttrName}(float value)");
                                writer.WriteLine("{");
                                writer.Indent++;
                                {
                                    writer.WriteLine($"{validAttrName}.SetBaseValue(value);");
                                    writer.WriteLine($"{validAttrName}.SetCurrentValue(value);");
                                }
                                writer.Indent--;
                                writer.WriteLine("}");

                                writer.WriteLine("");

                                writer.WriteLine($"public void SetCurrent{validAttrName}(float value)");
                                writer.WriteLine("{");
                                writer.Indent++;
                                {
                                    writer.WriteLine($"{validAttrName}.SetCurrentValue(value);");
                                }
                                writer.Indent--;
                                writer.WriteLine("}");

                                writer.WriteLine("");

                                writer.WriteLine($"public void SetBase{validAttrName}(float value)");
                                writer.WriteLine("{");
                                writer.Indent++;
                                {
                                    writer.WriteLine($"{validAttrName}.SetBaseValue(value);");
                                }
                                writer.Indent--;
                                writer.WriteLine("}");

                                writer.WriteLine("");

                                writer.WriteLine($"public void SetMin{validAttrName}(float value)");
                                writer.WriteLine("{");
                                writer.Indent++;
                                {
                                    writer.WriteLine($"{validAttrName}.SetMinValue(value);");
                                }
                                writer.Indent--;
                                writer.WriteLine("}");

                                writer.WriteLine("");

                                writer.WriteLine($"public void SetMax{validAttrName}(float value)");
                                writer.WriteLine("{");
                                writer.Indent++;
                                {
                                    writer.WriteLine($"{validAttrName}.SetMaxValue(value);");
                                }
                                writer.Indent--;
                                writer.WriteLine("}");

                                writer.WriteLine("");

                                writer.WriteLine($"public void SetMinMax{validAttrName}(float min, float max)");
                                writer.WriteLine("{");
                                writer.Indent++;
                                {
                                    writer.WriteLine($"{validAttrName}.SetMinMaxValue(min, max);");
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
                                    foreach (var attributeName in attributeSetConfig.AttributeNames)
                                    {
                                        string validAttrName = EditorUtil.MakeValidIdentifier(attributeName);
                                        writer.WriteLine($"case \"{validAttrName}\":");
                                        writer.Indent++;
                                        {
                                            writer.WriteLine($"return {validAttrName};");
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
                            foreach (var attributeName in attributeSetConfig.AttributeNames)
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
                            foreach (var attributeName in attributeSetConfig.AttributeNames)
                            {
                                string validAttrName = EditorUtil.MakeValidIdentifier(attributeName);
                                writer.WriteLine($"{validAttrName}.SetOwner(owner);");
                            }
                        }
                        writer.Indent--;
                        writer.WriteLine("}");

                        writer.WriteLine("");
                        writer.WriteLine("public static class Lookup");
                        writer.WriteLine("{");
                        writer.Indent++;
                        {
                            foreach (var attributeName in attributeSetConfig.AttributeNames)
                            {
                                string validAttrName = EditorUtil.MakeValidIdentifier(attributeName);
                                writer.WriteLine(
                                    $"public const string {attributeName} = \"AS_{validName}.{validAttrName}\";");
                            }
                        }
                        writer.Indent--;
                        writer.WriteLine("}");
                    }
                    writer.Indent--;
                    writer.WriteLine("}");
                    writer.WriteLine("");
                }

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

                    writer.WriteLine("");

                    writer.WriteLine(
                        "public static readonly Dictionary<Type, string> TypeToName = new Dictionary<Type, string>");
                    writer.WriteLine("{");
                    writer.Indent++;
                    {
                        foreach (var attributeSet in attributeSetConfigs)
                        {
                            var validName = EditorUtil.MakeValidIdentifier(attributeSet.Name);
                            writer.WriteLine($"{{ typeof(AS_{validName}), nameof(AS_{validName}) }},");
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
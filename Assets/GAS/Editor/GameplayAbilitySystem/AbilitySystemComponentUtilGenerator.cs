#if UNITY_EDITOR
namespace GAS.Editor
{
    using System;
    using System.IO;
    using GAS;
    using UnityEngine;


    public static class AbilitySystemComponentUtilGenerator
    {
        public static void Gen()
        {
            string pathWithoutAssets = Application.dataPath.Substring(0, Application.dataPath.Length - 6);
            var filePath =
                $"{pathWithoutAssets}/{GASSettingAsset.CodeGenPath}/{GasDefine.GAS_ASCUTIL_CSHARP_SCRIPT_NAME}";
            GenerateASCUtil(filePath);
        }

        private static void GenerateASCUtil(string filePath)
        {
            using var writer = new IndentedWriter(new StreamWriter(filePath));

            writer.WriteLine("///////////////////////////////////");
            writer.WriteLine("//// This is a generated file. ////");
            writer.WriteLine("////     Do not modify it.     ////");
            writer.WriteLine("///////////////////////////////////");

            writer.WriteLine("");

            writer.WriteLine("using System;");
            writer.WriteLine("using System.Linq;");
            writer.WriteLine("using UnityEngine;");

            writer.WriteLine("");

            writer.WriteLine("namespace GAS.Runtime");
            writer.WriteLine("{");
            writer.Indent++;
            {
                writer.WriteLine("public static class AbilitySystemComponentExtension");
                writer.WriteLine("{");
                writer.Indent++;
                {
                    writer.WriteLine("public static Type[] PresetAttributeSetTypes(this AbilitySystemComponent asc)");
                    writer.WriteLine("{");
                    writer.Indent++;
                    {
                        writer.WriteLine("if (asc.Preset == null) return null;");
                        writer.WriteLine("var attrSetTypes = new Type[asc.Preset.AttributeSets.Length];");
                        writer.WriteLine("for (var i = 0; i < asc.Preset.AttributeSets.Length; i++)");
                        writer.Indent++;
                        {
                            writer.WriteLine(
                                "attrSetTypes[i] = GAttrSetLib.AttrSetTypeDict[asc.Preset.AttributeSets[i]];");
                        }
                        writer.Indent--;
                        writer.WriteLine("return attrSetTypes;");
                    }
                    writer.Indent--;
                    writer.WriteLine("}");

                    writer.WriteLine("");

                    writer.WriteLine(
                        "public static GameplayTag[] PresetBaseTags(this AbilitySystemComponent asc)");
                    writer.WriteLine("{");
                    writer.Indent++;
                    {
                        writer.WriteLine("if (asc.Preset == null) return null;");
                        writer.WriteLine("return asc.Preset.BaseTags;");
                    }
                    writer.Indent--;
                    writer.WriteLine("}");

                    writer.WriteLine("");

                    writer.WriteLine(
                        "public static void InitWithPreset(this AbilitySystemComponent asc, int level, AbilitySystemComponentPreset preset = null)");
                    writer.WriteLine("{");
                    writer.Indent++;
                    {
                        writer.WriteLine("if (preset != null) asc.SetPreset(preset);");
                        writer.WriteLine("if (asc.Preset == null) return;");

                        writer.WriteLine("");

                        writer.WriteLine("#if UNITY_EDITOR", true);
                        writer.WriteLine("if (asc.Preset.BaseAbilities != null && asc.Preset.BaseAbilities.Any(x => x == null))");
                        writer.WriteLine("{");
                        writer.Indent++;
                        {
                            writer.WriteLine(
                                "Debug.LogWarning($\"BaseAbilities contains null in preset: {asc.Preset.name}\");");
                        }
                        writer.Indent--;
                        writer.WriteLine("}");
                        writer.WriteLine("#endif", true);

                        writer.WriteLine("");

                        writer.WriteLine(
                            "asc.Init(asc.PresetBaseTags(), asc.PresetAttributeSetTypes(), asc.Preset.BaseAbilities, level);");
                    }
                    writer.Indent--;
                    writer.WriteLine("}");
                }

                writer.Indent--;
                writer.WriteLine("}");
            }

            writer.Indent--;
            writer.Write("}");

            Console.WriteLine($"Generated AbilitySystemComponentExtension at path: {filePath}");
        }
    }
}
#endif
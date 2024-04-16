#if UNITY_EDITOR
namespace GAS.Editor
{
    using GAS.Runtime;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using GAS;
    using UnityEditor;
    using UnityEngine;
    using Editor;

    public static class GTagLibGenerator
    {
        public static void Gen()
        {
            var asset = GameplayTagsAsset.LoadOrCreate();
            string pathWithoutAssets = Application.dataPath.Substring(0, Application.dataPath.Length - 6);
            var filePath =
                $"{pathWithoutAssets}/{GASSettingAsset.CodeGenPath}/{GasDefine.GAS_TAG_LIB_CSHARP_SCRIPT_NAME}";
            var tags = asset.Tags;
            GenerateGTagLib(tags, filePath);
        }

        private static void GenerateGTagLib(List<GameplayTag> tags, string filePath)
        {
            using var writer = new IndentedWriter(new StreamWriter(filePath));
            writer.WriteLine("///////////////////////////////////");
            writer.WriteLine("//// This is a generated file. ////");
            writer.WriteLine("////     Do not modify it.     ////");
            writer.WriteLine("///////////////////////////////////");

            writer.WriteLine("");

            writer.WriteLine("using System.Collections.Generic;");

            writer.WriteLine("");

            writer.WriteLine("namespace GAS.Runtime");
            writer.WriteLine("{");
            writer.Indent++;
            {
                writer.WriteLine("public static class GTagLib");
                writer.WriteLine("{");
                writer.Indent++;
                {
                    // Generate members for each tag
                    foreach (var tag in tags)
                    {
                        var validName = MakeValidIdentifier(tag.Name);
                        writer.WriteLine(
                            $"public static GameplayTag {validName} {{ get; }} = new GameplayTag(\"{tag.Name}\");");
                    }

                    writer.WriteLine("");

                    writer.WriteLine(
                        "public static Dictionary<string, GameplayTag> TagMap = new Dictionary<string, GameplayTag>");
                    writer.WriteLine("{");
                    writer.Indent++;
                    {
                        foreach (var tag in tags)
                        {
                            var validName = MakeValidIdentifier(tag.Name);
                            writer.WriteLine($"[\"{tag.Name}\"] = {validName},");
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

        private static string MakeValidIdentifier(string name)
        {
            // Replace '.' with '_'
            name = name.Replace('.', '_');

            // If starts with a digit, add '_' at the beginning
            if (char.IsDigit(name[0])) name = "_" + name;

            // Ensure the identifier is valid
            return string.Join("",
                name.Split(
                    new[]
                    {
                        ' ', '-', '.', ':', ',', '!', '?', '#', '$', '%', '^', '&', '*', '(', ')', '[', ']', '{', '}',
                        '/', '\\', '|'
                    }, StringSplitOptions.RemoveEmptyEntries));
        }
    }
}
#endif
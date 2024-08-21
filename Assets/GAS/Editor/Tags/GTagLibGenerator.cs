using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using GAS.Runtime;
using UnityEngine;

namespace GAS.Editor
{
    public static class GTagLibGenerator
    {
        public static void Gen(string filePath = null)
        {
            var asset = GameplayTagsAsset.LoadOrCreate();
            var tags = asset.Tags;
            GenerateGTagLib(tags, filePath);

            Console.WriteLine($"Generated GTagLib at path: {filePath}");
        }

        public static string DefaultFilePath
        {
            get
            {
                string pathWithoutAssets = Application.dataPath.Substring(0, Application.dataPath.Length - 6);
                var filePath = $"{pathWithoutAssets}/{GASSettingAsset.CodeGenPath}/{GasDefine.GAS_TAG_LIB_CSHARP_SCRIPT_NAME}";
                return filePath;
            }
        }

        private record TagInfo(string Name, string Identifier, string Comment);

        // We expose the method as public to facilitate support for extension tools in generating…
        public static void GenerateGTagLib(IEnumerable<GameplayTag> gameplayTags, string filePath = null)
        {
            var gameplayTagInfos = gameplayTags
                .OrderBy(x => x.Name)
                .Select(x => new TagInfo(x.Name, MakeValidIdentifier(x.Name), x.Name))
                .ToArray();

            filePath ??= DefaultFilePath;

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
                    bool skippedFirst = false;
                    foreach (var tagInfo in gameplayTagInfos)
                    {
                        if (!skippedFirst) skippedFirst = true;
                        else writer.WriteLine();

                        writer.WriteLine($"/// <summary>{tagInfo.Comment}</summary>");
                        writer.WriteLine($"public static GameplayTag {tagInfo.Identifier} {{ get; }} = new(\"{tagInfo.Name}\");");
                    }

                    writer.WriteLine("");

                    writer.WriteLine("public static readonly IReadOnlyDictionary<string, GameplayTag> TagMap = new Dictionary<string, GameplayTag>");
                    writer.WriteLine("{");
                    writer.Indent++;
                    {
                        foreach (var tagInfo in gameplayTagInfos)
                        {
                            writer.WriteLine($"[\"{tagInfo.Name}\"] = {tagInfo.Identifier},");
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
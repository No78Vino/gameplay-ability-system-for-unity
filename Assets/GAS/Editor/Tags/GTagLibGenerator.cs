using System.Linq;

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
        public static void Gen(string filePath = null)
        {
            var asset = GameplayTagsAsset.LoadOrCreate();
            var tags = asset.Tags;
            GenerateGTagLib(tags, filePath);
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

        // We expose the method as public to facilitate support for extension tools in generating…
        public static void GenerateGTagLib(IEnumerable<GameplayTag> gameplayTags, string filePath = null)
        {
            var gameplayTagNamesWithIdentifier = gameplayTags
                .OrderBy(x => x.Name)
                .Select(x => new Tuple<string, string>(x.Name, MakeValidIdentifier(x.Name)))
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
                    // Generate members for each tag
                    foreach (var tuple in gameplayTagNamesWithIdentifier)
                    {
                        writer.WriteLine(
                            $"public static GameplayTag {tuple.Item2} {{ get; }} = new GameplayTag(\"{tuple.Item1}\");");
                    }

                    writer.WriteLine("");

                    writer.WriteLine(
                        "public static Dictionary<string, GameplayTag> TagMap = new Dictionary<string, GameplayTag>");
                    writer.WriteLine("{");
                    writer.Indent++;
                    {
                        foreach (var tuple in gameplayTagNamesWithIdentifier)
                        {
                            writer.WriteLine($"[\"{tuple.Item1}\"] = {tuple.Item2},");
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
using System;
using System.IO;
using GAS.Editor;
using UnityEditor;

namespace _ProjectCodeGenerate
{
    public static class _Generator_AbilityConfigSO
    {
        [MenuItem("EX-GAS/_ProjectGenerator/_AbilityConfigSO", priority = 0)]
        public static void Gen()
        {
            // var asset = GameplayTagsAsset.LoadOrCreate();
            // var tags = asset.Tags;
            // GenerateAbilityConfigtSO(tags, filePath);
        }
        
         public static void GenerateAbilityConfigSO(string filePath)
        {
            // var gameplayTagNamesWithIdentifier = gameplayTags
            //     .OrderBy(x => x.Name)
            //     .Select(x => new Tuple<string, string>(x.Name, MakeValidIdentifier(x.Name)))
            //     .ToArray();

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
                    // foreach (var tuple in gameplayTagNamesWithIdentifier)
                    // {
                    //     writer.WriteLine(
                    //         $"public static GameplayTag {tuple.Item2} {{ get; }} = new GameplayTag(\"{tuple.Item1}\");");
                    // }

                    writer.WriteLine("");

                    writer.WriteLine(
                        "public static Dictionary<string, GameplayTag> TagMap = new Dictionary<string, GameplayTag>");
                    writer.WriteLine("{");
                    writer.Indent++;
                    // {
                    //     foreach (var tuple in gameplayTagNamesWithIdentifier)
                    //     {
                    //         writer.WriteLine($"[\"{tuple.Item1}\"] = {tuple.Item2},");
                    //     }
                    // }
                    writer.Indent--;
                    writer.WriteLine("};");
                }
                writer.Indent--;
                writer.WriteLine("}");
            }
            writer.Indent--;
            writer.Write("}");

            Console.WriteLine($"Generated GEN_AbilityConfigSO at path: {filePath}");
        }
    }
}
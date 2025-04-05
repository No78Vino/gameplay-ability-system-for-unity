using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using GAS.Editor;
using GAS.Runtime;
using GAS.RuntimeDataHelper.Helper;
using UnityEditor;
using UnityEngine;

namespace GAS.RuntimeDataHelper.Tag
{
    /// <summary>
    /// 标签代码库脚本生成器
    /// </summary>
    public static class GeneratorGameplayTagCode
    {
        [MenuItem("EX-GAS/CodeGenerate/GameplayTagCode")]
        public static void Gen()
        {
            string pathWithoutAssets = Application.dataPath.Substring(0, Application.dataPath.Length - 6);
            var filePath =
                $"{pathWithoutAssets}/{GASSettingAsset.CodeGenPath}/{GasDefine.GAS_GAMEPLAY_TAG_CODE_LIB_CSHARP_SCRIPT_NAME}";
            GenerateGameplayTagCodeLib(filePath);
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
        
        private static void GenerateGameplayTagCodeLib(string filePath)
        {
            var asset = GameplayTagsAsset.LoadOrCreate();
            var gameplayTagNamesWithIdentifier = asset.Tags
                .OrderBy(x => x.Name)
                .Select(x => new Tuple<GameplayTag, string>(x, MakeValidIdentifier(x.Name)))
                .ToArray();
            
            using var writer = new IndentedWriter(new StreamWriter(filePath));
            writer.WriteLine("///////////////////////////////////");
            writer.WriteLine("//// This is a generated file. ////");
            writer.WriteLine("////     Do not modify it.     ////");
            writer.WriteLine("///////////////////////////////////");

            writer.WriteLine("");
            writer.WriteLine("using System.Collections.Generic;");
            writer.WriteLine("using GAS.RuntimeWithECS.Tag;");
            writer.WriteLine("");
            writer.WriteLine("namespace GAS.Runtime");
            writer.WriteLine("{");
            writer.Indent++;
            {
                writer.WriteLine("public static class GEN_GameplayTagCode");
                writer.WriteLine("{");
                writer.Indent++;
                {
                    var allTagAsset = gameplayTagNamesWithIdentifier;
                    foreach (var tuple in gameplayTagNamesWithIdentifier)
                    {
                        writer.WriteLine(
                            $"public const int {tuple.Item2} = {tuple.Item1.HashCode};");
                    }

                    writer.WriteLine("");
                    writer.WriteLine("public static void InitTagList()");
                    writer.WriteLine("{");
                    writer.Indent++;
                    {
                        writer.WriteLine("GTagUtil.InitTagMap(new Dictionary<int, GASTag>()");
                        writer.WriteLine("{");
                        writer.Indent++;
                        {
                            foreach (var tagAsset in allTagAsset)
                            {
                                var tag = tagAsset.Item1;
                                var tagName = tagAsset.Item2;
                                var parentTagNames = new List<string>();
                                string[] parentTagNamesArray = tagName.Split('_');
                                string tempParentName = "";
                                for (var i = 0; i < parentTagNamesArray.Length - 1; i++)
                                {
                                    var parentName = parentTagNamesArray[i];
                                    tempParentName += parentName;
                                    parentTagNames.Add(tempParentName);
                                    tempParentName += "_";
                                }
                                var childTagNames = new string[tag.AncestorNames.Length];
                                for (var i = 0; i < tag.AncestorNames.Length; i++)
                                {
                                    var childName = tag.AncestorNames[i];
                                    childTagNames[i] = MakeValidIdentifier(childName);
                                }


                                writer.WriteLine(
                                    $"{{ {tagName}, " +
                                    $"new GASTag({tagName}, " +
                                    $"new int[] {{ {string.Join(", ", parentTagNames)} }}, " +
                                    $"new int[] {{ {string.Join(", ", childTagNames)} }}) }},");
                            }
                        }

                        writer.Indent--;
                        writer.WriteLine("});");
                    }

                    writer.Indent--;
                    writer.WriteLine("}");
                }

                writer.Indent--;
                writer.WriteLine("}");
            }
            writer.Indent--;
            writer.WriteLine("}");
        }
    }
}
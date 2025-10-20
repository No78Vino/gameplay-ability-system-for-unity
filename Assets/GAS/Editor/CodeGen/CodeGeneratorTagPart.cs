using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using UnityEditor;
using Debug = UnityEngine.Debug;

namespace GAS.Editor
{
    public static class CodeGeneratorTagPart
    {
        public static void GenerateTag()
        {
            var setting = GASSettingAsset.LoadOrCreate();
            var tagJsonFilePath = setting.PathOfJsonTag;
            // 检查文件是否存在
            if (!File.Exists(tagJsonFilePath))
            {
                Debug.LogError($"JSON file not found at {tagJsonFilePath}");
                return;
            }
            var tagJsonText = File.ReadAllText(tagJsonFilePath);
            var tags = GasJsonReader.ReadTags(tagJsonText);

            var tagNameToIdMap = tags.ToDictionary(t => CodeGenerator.MakeTagValidIdentifier(t.name), t => t.id);
            var tagIdToNameMap = tags.ToDictionary(t => t.id, t => CodeGenerator.MakeTagValidIdentifier(t.name));

            // 预处理，获取所有Tag的父子关系
            // 父节点
            Dictionary<int, List<string>> parentMap = new Dictionary<int, List<string>>();
            foreach (var tag in tags)
            {
                var tagName = CodeGenerator.MakeTagValidIdentifier(tag.name);
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
                
                parentMap.Add(tag.id, parentTagNames);
            }
            // 子节点
            Dictionary<int, List<string>> childMap = new Dictionary<int, List<string>>();
            foreach (var tag in tags) childMap.Add(tag.id,new List<string>());

            foreach (var kp in parentMap)
            {
                var parents = kp.Value;
                foreach (var pName in parents)
                {
                    var pID = tagNameToIdMap[pName];
                    foreach (var tag in tags)
                    {
                        if (CodeGenerator.MakeTagValidIdentifier(tag.name) == pName)
                        {
                            childMap[pID].Add(tagIdToNameMap[kp.Key]);
                        }
                    }
                }
            }

            var filePath = setting.PathOfCodeTag;

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
                writer.WriteLine("public static class XTag");
                writer.WriteLine("{");
                writer.Indent++;
                {
                    var allTagAsset = tags;
                    foreach (var tag in tags)
                    {
                        writer.WriteLine(
                            $"public const int {CodeGenerator.MakeTagValidIdentifier(tag.name)} = {tag.id};");
                    }

                    writer.WriteLine("");
                    writer.WriteLine("public static void InitTagList()");
                    writer.WriteLine("{");
                    writer.Indent++;
                    {
                        writer.WriteLine("TagHelper.InitTagMap(new Dictionary<int, GameplayTag>()");
                        writer.WriteLine("{");
                        writer.Indent++;
                        {
                            foreach (var tagInEditor in allTagAsset)
                            {
                                var tag = tagInEditor.id;
                                var tagName = CodeGenerator.MakeTagValidIdentifier(tagInEditor.name);
                                writer.WriteLine(
                                    $"{{ {tagName}, " +
                                    $"new GameplayTag({tagName}, " +
                                    $"new int[] {{ {string.Join(", ", parentMap[tag])} }}, " +
                                    $"new int[] {{ {string.Join(", ", childMap[tag])} }}) }},");
                            }
                        }

                        writer.Indent--;
                        writer.WriteLine("},");

                        writer.WriteLine("new Dictionary<int, string>()");
                        writer.WriteLine("{");
                        writer.Indent++;
                        {
                            foreach (var tagAsset in allTagAsset)
                            {
                                var tagName = CodeGenerator.MakeTagValidIdentifier(tagAsset.name);
                                writer.WriteLine(
                                    $"{{ {tagName}, \"{tagAsset.name}\" }},");
                            }
                        }
                        writer.Indent--;
                        writer.WriteLine("}");
                        writer.WriteLine(");");
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
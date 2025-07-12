using System;
using System.IO;
using UnityEditor;

namespace GAS.Editor
{
    public static class CodeGenerator
    {
        private static string MakeTagValidIdentifier(string name)
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
        
        /// <summary>
        ///  生成Tag代码
        /// </summary>
        [MenuItem("EXTool/EX-GAS/生成脚本/GameplayTag")]
        public static void GenerateTagCode()
        {
            var setting = GASSettingAsset.LoadOrCreate();
            var tagJsonFilePath = setting.PathOfJsonTag;
            var tagJsonText = File.ReadAllText(tagJsonFilePath);
            var tags = GasJsonReader.ReadTags(tagJsonText);

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
                writer.WriteLine("public static class GTagLib");
                writer.WriteLine("{");
                writer.Indent++;
                {
                    // Generate members for each tag
                    foreach (var tag in tags)
                    {
                        writer.WriteLine(
                            $"public static GameplayTag {MakeTagValidIdentifier(tag.name)} {{ get; }} = new GameplayTag(\"{tag.name}\");");
                    }
            
                    writer.WriteLine("");
            
                    writer.WriteLine(
                        "public static Dictionary<string, GameplayTag> TagMap = new Dictionary<string, GameplayTag>");
                    writer.WriteLine("{");
                    writer.Indent++;
                    {
                        foreach (var tag in tags)
                        {
                            writer.WriteLine($"[\"{tag.name}\"] = {MakeTagValidIdentifier(tag.name)},");
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
    }
}
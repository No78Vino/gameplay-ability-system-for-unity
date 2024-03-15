﻿#if UNITY_EDITOR
namespace GAS.Editor
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using GAS.Core;
    using UnityEditor;
    using UnityEngine;
    using Editor;
    public static class GTagLibGenerator
    {
        public static void Gen()
        {
            var asset = AssetDatabase.LoadAssetAtPath<GameplayTagsAsset>(GASSettingAsset.GAS_TAG_ASSET_PATH);
            string pathWithoutAssets = Application.dataPath.Substring(0, Application.dataPath.Length - 6);
            var filePath = $"{pathWithoutAssets}/{GASSettingAsset.CodeGenPath}/{GasDefine.GAS_TAG_LIB_CSHARP_SCRIPT_NAME}";
            var tags = asset.Tags;
            GenerateGTagLib(tags, filePath);
        }

        private static void GenerateGTagLib(List<GameplayTag> tags, string filePath)
        {
            using var writer = new StreamWriter(filePath);
            writer.WriteLine("///////////////////////////////////");
            writer.WriteLine("//// This is a generated file. ////");
            writer.WriteLine("////     Do not modify it.     ////");
            writer.WriteLine("///////////////////////////////////");
            writer.WriteLine("using System.Collections.Generic;");
            writer.WriteLine("namespace GAS.Runtime.Tags");
            writer.WriteLine("{");
            writer.WriteLine("public static class GTagLib");
            writer.WriteLine("{");

            // Generate members for each tag
            foreach (var tag in tags)
            {
                var validName = MakeValidIdentifier(tag.Name);
                writer.WriteLine($"    public static GameplayTag {validName} {{ get;}} = new GameplayTag(\"{tag.Name}\");");
            }
            
            writer.WriteLine("");
            
            writer.WriteLine("      public static Dictionary<string, GameplayTag> TagMap = new Dictionary<string, GameplayTag>");
            writer.WriteLine("      {");
            foreach (var tag in tags)
            {
                var validName = MakeValidIdentifier(tag.Name);
                writer.WriteLine($"         [\"{tag.Name}\"] = {validName},");   
            }
            writer.WriteLine("      };");
            
            writer.WriteLine("}");
            writer.WriteLine("}");

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
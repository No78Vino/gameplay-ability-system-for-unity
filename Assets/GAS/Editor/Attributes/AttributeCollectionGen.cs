﻿#if UNITY_EDITOR

namespace GAS.Editor
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using UnityEngine;
    
    public static class AttributeCollectionGen
    {
        private sealed class AttributeInfo
        {
            public string Name;
            public string Comment;
        }

        public static void Gen()
        {
            var asset = AttributeAsset.LoadOrCreate();
            string pathWithoutAssets = Application.dataPath.Substring(0, Application.dataPath.Length - 6);
            var filePath =
                $"{pathWithoutAssets}/{GASSettingAsset.CodeGenPath}/{GasDefine.GAS_ATTRIBUTE_LIB_CSHARP_SCRIPT_NAME}";


            var attributeInfos = (from t in asset.attributes
                where !string.IsNullOrWhiteSpace(t.Name)
                select new AttributeInfo { Name = t.Name, Comment = t.Comment }).ToList();


            GenerateAttributeCollection(attributeInfos, filePath);
        }

        private static void GenerateAttributeCollection(IEnumerable<AttributeInfo> attributes, string filePath)
        {
            using var writer = new IndentedWriter(new StreamWriter(filePath));

            writer.WriteLine("///////////////////////////////////");
            writer.WriteLine("//// This is a generated file. ////");
            writer.WriteLine("////     Do not modify it.     ////");
            writer.WriteLine("///////////////////////////////////");

            writer.WriteLine("");

            writer.WriteLine("namespace GAS.Runtime");
            writer.WriteLine("{");
            writer.Indent++;
            {
                writer.WriteLine("public static class GAttrLib");
                writer.WriteLine("{");
                writer.Indent++;
                {
                    bool skippedFirst = false;
                    // Generate members for each ATTRIBUTE
                    foreach (var attr in attributes)
                    {
                        if (!skippedFirst) skippedFirst = true;
                        else writer.WriteLine();

                        var validName = EditorUtil.MakeValidIdentifier(attr.Name);
                        writer.WriteLine("/// <summary>");
                        writer.WriteLine($"/// {attr.Comment}");
                        writer.WriteLine("/// </summary>");
                        writer.WriteLine($"public const string {validName} = \"{attr.Name}\";");
                    }
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
#endif
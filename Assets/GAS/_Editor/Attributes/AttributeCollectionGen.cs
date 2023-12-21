using System;
using System.Collections.Generic;
using System.IO;
using GAS.Core;
using GAS.Runtime.Attribute;
using UnityEditor;
using UnityEngine;

namespace GAS.Editor.Attributes
{
    public static class AttributeCollectionGen
    {
        public static void Gen()
        {
            var asset = AssetDatabase.LoadAssetAtPath<AttributeAsset>(GasDefine.GAS_ATTRIBUTE_ASSET_PATH);
            var filePath = $"{Application.dataPath}/{asset.AttributeCollectionGenPath}/{GasDefine.GAS_ATTRIBUTE_COLLECTION_CSHARP_SCRIPT_NAME}";
            var attributeNames = asset.AttributeNames;
            GenerateAttributeCollection(attributeNames, filePath);
        }

        private static void GenerateAttributeCollection(List<string> attributes, string filePath)
        {
            using var writer = new StreamWriter(filePath);
            writer.WriteLine("///////////////////////////////////");
            writer.WriteLine("//// This is a generated file. ////");
            writer.WriteLine("////     Do not modify it.     ////");
            writer.WriteLine("///////////////////////////////////");
            writer.WriteLine("namespace GAS.Runtime.Attribute");
            writer.WriteLine("{");
            writer.WriteLine("public static class AttributeCollection");
            writer.WriteLine("{");

            // Generate members for each ATTRIBUTE
            foreach (var attr in attributes)
            {
                var validName = EditorUtil.MakeValidIdentifier(attr);
                writer.WriteLine(
                    $"    public const string {validName} = \"{attr}\";");
            }

            writer.WriteLine("}");
            writer.WriteLine("}");

            Console.WriteLine($"Generated GameplayTagSumCollection at path: {filePath}");
        }
    }
}
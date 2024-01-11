
#if UNITY_EDITOR
namespace GAS.Editor.Attributes
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using GAS.Core;
    using GAS.Runtime.Attribute;
    using UnityEditor;
    using UnityEngine;
    using GAS.Editor.GameplayAbilitySystem;
    
    public static class AttributeCollectionGen
    {
        public static void Gen()
        {
            var asset = AssetDatabase.LoadAssetAtPath<AttributeAsset>(GASSettingAsset.GAS_ATTRIBUTE_ASSET_PATH);
            string pathWithoutAssets = Application.dataPath.Substring(0, Application.dataPath.Length - 6);
            var filePath = $"{pathWithoutAssets}/{GASSettingAsset.CodeGenPath}/{GasDefine.GAS_ATTRIBUTE_COLLECTION_CSHARP_SCRIPT_NAME}";
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
#endif
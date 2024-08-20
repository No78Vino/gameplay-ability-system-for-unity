using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using GAS.General;
using UnityEngine;

namespace GAS.Runtime
{
    public static class ReflectionHelper
    {
        #region AttributeSetNames

        private static string[] _attributeSetNames;

        public static IEnumerable<string> AttributeSetNames
        {
            get
            {
                _attributeSetNames ??= LoadAttributeSetNames();
                return _attributeSetNames;
            }
        }

        private static string[] LoadAttributeSetNames()
        {
            var libType = TypeUtil.FindTypeInAllAssemblies("GAS.Runtime.GAttrSetLib");
            if (libType == null)
            {
                Debug.LogError("[EX] Type 'GAttrSetLib' not found. Please generate the GAttrSetLib CODE first!");
                return Array.Empty<string>();
            }

            const string fieldName = "AttrSetTypeDict";
            var field = libType.GetField(fieldName, BindingFlags.Public | BindingFlags.Static);
            if (field == null)
            {
                Debug.LogError($"[EX] Field \"{fieldName}\" not found in GAttrSetLib!");
                return Array.Empty<string>();
            }

            var value = field.GetValue(null);
            if (value is not Dictionary<string, Type> dict)
            {
                Debug.LogError($"[EX] Field \"{fieldName}\" is not a Dictionary<string, Type> in GAttrSetLib!");
                return Array.Empty<string>();
            }

            return dict.Keys.ToArray();
        }

        #endregion AttributeSetNames

        #region AttributeNames

        private static string[] _attributeNames;

        public static IEnumerable<string> AttributeNames
        {
            get
            {
                _attributeNames ??= LoadAttributeNames();
                return _attributeNames;
            }
        }

        private static string[] LoadAttributeNames()
        {
            var libType = TypeUtil.FindTypeInAllAssemblies("GAS.Runtime.GAttrSetLib");
            if (libType == null)
            {
                Debug.LogError("[EX] Type 'GAttrSetLib' not found. Please generate the GAttrSetLib CODE first!");
                return Array.Empty<string>();
            }

            const string fieldName = "AttributeFullNames";
            var field = libType.GetField(fieldName, BindingFlags.Public | BindingFlags.Static);
            if (field == null)
            {
                Debug.LogError($"[EX] Field \"{fieldName}\" not found in GAttrSetLib!");
                return Array.Empty<string>();
            }

            var value = field.GetValue(null);
            if (value is not List<string> list)
            {
                Debug.LogError($"[EX] Field \"{fieldName}\" is not a List<string> in GAttrSetLib!");
                return Array.Empty<string>();
            }

            return list.ToArray();
        }

        #endregion AttributeNames

        #region Attributes

        private static Dictionary<string, AttributeSet> _attributeSetDict;
        private static Dictionary<string, AttributeBase> _attributeDict;
        private static AttributeBase[] _attributes;

        public static IEnumerable<AttributeBase> Attributes
        {
            get
            {
                if (_attributes == null)
                {
                    LoadAttributes();
                }

                return _attributes;
            }
        }

        private static void LoadAttributes()
        {
            _attributes = AttributeNames.Select(x =>
            {
                var strings = x.Split('.');
                return LoadAttribute(strings[0], strings[1]);
            }).Where(x => x != null).ToArray();
        }


        public static AttributeBase GetAttribute(string attributeFullName)
        {
            if (_attributes == null)
            {
                LoadAttributes();
            }

            _attributeDict.TryGetValue(attributeFullName, out var attr);
            return attr;
        }

        public static AttributeBase GetAttribute(string attrSetName, string attrName)
        {
            return GetAttribute(attrSetName + "." + attrName);
        }

        private static AttributeBase LoadAttribute(string attrSetName, string attrName)
        {
            _attributeSetDict ??= new();
            _attributeDict ??= new();
            if (!_attributeSetDict.TryGetValue(attrSetName, out var attrSet))
            {
                var fullName = $"GAS.Runtime.{attrSetName}";
                var type = TypeUtil.FindTypeInAllAssemblies(fullName);
                if (type == null)
                {
                    Debug.LogError($"[EX] attr set Type '{fullName}' not found. Please generate the GAttrSetLib CODE first!");
                    return null;
                }

                attrSet = Activator.CreateInstance(type) as AttributeSet;
                _attributeSetDict[attrSetName] = attrSet;
            }

            if (attrSet == null)
            {
                Debug.LogError($"[EX] attr set '{attrSetName}' is null. Please generate the GAttrSetLib CODE first!");
                return null;
            }

            var attr = attrSet[attrName];
            if (attr == null)
            {
                Debug.LogError($"[EX] attr '{attrSetName}.{attrName}' is null. Please generate the GAttrSetLib CODE first!");
                return null;
            }

            _attributeDict[attr.Name] = attr;
            return attr;
        }

        #endregion Attributes

        #region GameplayTags

        private static GameplayTag[] _gameplayTags;

        public static IEnumerable<GameplayTag> GameplayTags
        {
            get
            {
                _gameplayTags ??= LoadTags();
                return _gameplayTags;
            }
        }

        private static GameplayTag[] LoadTags()
        {
            var tagLibType = TypeUtil.FindTypeInAllAssemblies("GAS.Runtime.GTagLib");
            if (tagLibType == null)
            {
                Debug.LogError("[EX] Type 'GTagLib' not found. Please generate the TAGS CODE first!");
                return Array.Empty<GameplayTag>();
            }

            const string fieldName = "TagMap";
            var field = tagLibType.GetField("TagMap", BindingFlags.Public | BindingFlags.Static);
            if (field == null)
            {
                Debug.LogError($"[EX] Field \"{fieldName}\" not found in GTagLib!");
                return Array.Empty<GameplayTag>();
            }

            var value = field.GetValue(null);
            if (value is not Dictionary<string, GameplayTag> tagMap)
            {
                Debug.LogError($"[EX] Field \"{fieldName}\" is not a Dictionary<string, GameplayTag> in GTagLib!");
                return Array.Empty<GameplayTag>();
            }

            return tagMap.Values.ToArray();
        }

        #endregion GameplayTags
    }
}
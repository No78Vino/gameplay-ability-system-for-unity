using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using GAS.General;
using GAS.Runtime;
using GAS.RuntimeDataHelper.Ability;
using GAS.RuntimeDataHelper.Ability.AbilityParam;
using GAS.RuntimeDataHelper.GameplayEffect;
using GAS.RuntimeWithECS;
using Sirenix.OdinInspector;
using Sirenix.Utilities.Editor;
using UnityEditor;
using UnityEngine;

namespace GAS.RuntimeDataHelper.Helper
{
    public static class EXEditorHelper
    {
        public static void ShowNotification(string message, float duration = 2f)
        {
            // 获取当前激活的编辑器窗口（通常是Inspector或场景视图）
            var targetWindow = EditorWindow.mouseOverWindow ? EditorWindow.mouseOverWindow : EditorWindow.focusedWindow;

            if (targetWindow != null)
                targetWindow.ShowNotification(new GUIContent(message));
            else
                Debug.LogWarning(message);
        }


        /// <summary>
        ///     获取工程中所有指定类型的 ScriptableObject 资源 (Odin优化版)
        /// </summary>
        /// <typeparam name="T">继承自 ScriptableObject 的类型</typeparam>
        /// <param name="includeSubAssets">是否包含子资源（如嵌套在 Prefab 中的资源）</param>
        public static List<T> FindAll<T>(bool includeSubAssets = false) where T : ScriptableObject
        {
            // Odin 的缓存式查找（性能优化关键）
            var assets = AssetUtilities.GetAllAssetsOfType<T>();

            var results = new List<T>();

            foreach (var asset in assets)
                // // 直接通过 GUID 获取强类型对象
                // var asset = AssetDatabase.LoadAssetAtPath<T>(
                //     AssetDatabase.GUIDToAssetPath(guid));
                results.Add(asset);

            return results;
        }

        /// <summary>
        ///     快速版本（不加载资源本体）
        /// </summary>
        public static List<string> FindAllPaths<T>() where T : ScriptableObject
        {
            var assets = AssetUtilities.GetAllAssetsOfType<T>().ToList();
            List<string> paths = new();
            foreach (var a in assets)
            {
                var p = AssetDatabase.GetAssetPath(a);
                if (!string.IsNullOrEmpty(p))
                    paths.Add(p);
            }

            return paths;
        }

        public static int GetFrameRate()
        {
            return (int)Math.Round(1 / Time.fixedDeltaTime);
        }
        
        #region GameplayTag

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
                Debug.LogError($"[EX] Field {fieldName} not found in GTagLib!");
                return Array.Empty<GameplayTag>();
            }

            var value = field.GetValue(null);
            if (value is not Dictionary<string, GameplayTag> tagMap)
            {
                Debug.LogError($"[EX] Field {fieldName} is not a Dictionary<string, GameplayTag> in GTagLib!");
                return Array.Empty<GameplayTag>();
            }

            return tagMap.Values.ToArray();
        }
        
        private static ValueDropdownItem[] _gameplayTagChoices;
        
        public static IEnumerable<ValueDropdownItem> GameplayTagCodeChoices
        {
            get
            {
                _gameplayTagChoices ??= ReflectionHelper.GameplayTags
                    .Select(gameplayTag => new ValueDropdownItem(gameplayTag.Name, gameplayTag.Code))
                    .ToArray();
                return _gameplayTagChoices;
            }
        }

        #endregion
        
        #region Ability

        private static IEnumerable<Type> _cachedAbilityComponentSubTypes;

        public static IEnumerable<Type> GetCachedAbilityComponentSubTypes()
        {
            if (_cachedAbilityComponentSubTypes != null) return _cachedAbilityComponentSubTypes;
            var assemblies = AppDomain.CurrentDomain.GetAssemblies();
            _cachedAbilityComponentSubTypes = assemblies
                .SelectMany(asm => asm.GetTypes())
                .Where(type =>
                    type.IsSubclassOf(typeof(BaseGameplayAbilityComponentConfigAsset)) &&
                    !type.IsAbstract &&
                    type.IsDefined(typeof(SerializableAttribute), false)
                )
                .ToList();

            return _cachedAbilityComponentSubTypes;
        }

        private static ValueDropdownItem[] _abilityComponentTypeChoices;
        
        public static IEnumerable<ValueDropdownItem> AbilityComponentTypeChoices
        {
            get
            {
                _abilityComponentTypeChoices ??= GetCachedAbilityComponentSubTypes()
                    .Select(type => new ValueDropdownItem(type.Name, type.FullName))
                    .ToArray();
                return _abilityComponentTypeChoices;
            }
        }
        
        private static IEnumerable<Type> _cachedAbilityLogicTypes;

        public static IEnumerable<Type> GetCachedAbilityLogicTypes()
        {
            if (_cachedAbilityLogicTypes != null) return _cachedAbilityLogicTypes;
            var assemblies = AppDomain.CurrentDomain.GetAssemblies();
            _cachedAbilityLogicTypes = assemblies
                .SelectMany(asm => asm.GetTypes())
                .Where(type =>
                    type.IsSubclassOf(typeof(AbilityLogicBase)) &&
                    !type.IsAbstract
                )
                .ToList();

            return _cachedAbilityLogicTypes;
        }

        private static ValueDropdownItem[] _abilityLogicChoices;

        public static IEnumerable<ValueDropdownItem> AbilityLogicChoices
        {
            get
            {
                if (_abilityLogicChoices == null || _abilityLogicChoices.Length == 0)
                {
                    var types = GetCachedAbilityLogicTypes();
                    _abilityLogicChoices = types
                        .Select(type => new ValueDropdownItem(type.Name, type.FullName))
                        .ToArray();
                }

                return _abilityLogicChoices;
            }
        }

        private static IEnumerable<Type> _cachedAbilityParamConfigTypes;

        public static IEnumerable<Type> GetCachedAbilityParamConfigTypes()
        {
            if (_cachedAbilityParamConfigTypes != null) return _cachedAbilityParamConfigTypes;
            var assemblies = AppDomain.CurrentDomain.GetAssemblies();
            _cachedAbilityParamConfigTypes = assemblies
                .SelectMany(asm => asm.GetTypes())
                .Where(type =>
                    type.IsSubclassOf(typeof(AbilityParamConfigBase)) &&
                    !type.IsAbstract &&
                    type.IsDefined(typeof(SerializableAttribute), false)
                )
                .ToList();

            return _cachedAbilityParamConfigTypes;
        }


        private static Dictionary<Type, Type> _cachedAbilityParamTypeToAbilityParamConfigTypeMap;

        public static Dictionary<Type, Type> GetCachedAbilityParamTypeToAbilityParamConfigTypeMap()
        {
            if (_cachedAbilityParamTypeToAbilityParamConfigTypeMap != null)
                return _cachedAbilityParamTypeToAbilityParamConfigTypeMap;
            var types = GetCachedAbilityParamConfigTypes();
            _cachedAbilityParamTypeToAbilityParamConfigTypeMap = new Dictionary<Type, Type>();
            foreach (var derivedType in types)
            {
                var baseType = derivedType.BaseType; // 获取基类类型

                if (baseType != null && baseType.IsGenericType)
                {
                    // 获取泛型类型定义（如 AbilityParamConfigBase<>）
                    var genericBaseDef = baseType.GetGenericTypeDefinition();

                    // 确认是否是所需的基类泛型定义
                    if (genericBaseDef == typeof(AbilityParamConfigBase<>))
                    {
                        // 获取实际使用的泛型参数（如 AbilityParamString）
                        var genericArgs = baseType.GetGenericArguments();
                        var paramType = genericArgs[0];

                        _cachedAbilityParamTypeToAbilityParamConfigTypeMap[paramType] = derivedType;
                    }
                }
            }

            return _cachedAbilityParamTypeToAbilityParamConfigTypeMap;
        }


        private static Dictionary<string, Type> _cachedAbilityLogicToAbilityParamConfigTypeMap;

        public static Dictionary<string, Type> GetCachedAbilityLogicToAbilityParamConfigTypeMap()
        {
            if (_cachedAbilityLogicToAbilityParamConfigTypeMap != null)
                return _cachedAbilityLogicToAbilityParamConfigTypeMap;
            var types = GetCachedAbilityLogicTypes();
            _cachedAbilityLogicToAbilityParamConfigTypeMap = new Dictionary<string, Type>();
            foreach (var derivedType in types)
            {
                var baseType = derivedType.BaseType; // 获取基类类型

                if (baseType != null && baseType.IsGenericType)
                {
                    // 获取泛型类型定义（如 AbilityLogicBase<>）
                    var genericBaseDef = baseType.GetGenericTypeDefinition();

                    // 确认是否是所需的基类泛型定义
                    if (genericBaseDef == typeof(AbilityLogicBase<>))
                    {
                        // 获取实际使用的泛型参数（如 AbilityParamString）
                        var genericArgs = baseType.GetGenericArguments();
                        var paramType = genericArgs[0];

                        if (derivedType.FullName != null)
                        {
                            var param2ParamConfigMap = GetCachedAbilityParamTypeToAbilityParamConfigTypeMap();
                            if (!param2ParamConfigMap.ContainsKey(paramType))
                                ShowNotification(
                                    $"未找到对应的能力参数配置，请检查类【{derivedType.FullName}】是否继承自AbilityParamConfigBase<T>");
                            else
                                _cachedAbilityLogicToAbilityParamConfigTypeMap[derivedType.FullName] =
                                    param2ParamConfigMap[paramType];
                        }
                    }
                }
            }

            return _cachedAbilityLogicToAbilityParamConfigTypeMap;
        }

        private static Dictionary<string, Type> _cachedAbilityLogicToAbilityParamTypeMap;

        public static Dictionary<string, Type> GetCachedAbilityLogicToAbilityParamTypeMap()
        {
            if (_cachedAbilityLogicToAbilityParamTypeMap != null)
                return _cachedAbilityLogicToAbilityParamTypeMap;
            var types = GetCachedAbilityLogicTypes();
            _cachedAbilityLogicToAbilityParamTypeMap = new Dictionary<string, Type>();
            foreach (var derivedType in types)
            {
                var baseType = derivedType.BaseType; // 获取基类类型

                if (baseType != null && baseType.IsGenericType)
                {
                    // 获取泛型类型定义（如 AbilityLogicBase<>）
                    var genericBaseDef = baseType.GetGenericTypeDefinition();

                    // 确认是否是所需的基类泛型定义
                    if (genericBaseDef == typeof(AbilityLogicBase<>))
                    {
                        // 获取实际使用的泛型参数（如 AbilityParamString）
                        var genericArgs = baseType.GetGenericArguments();
                        var paramType = genericArgs[0];

                        if (derivedType.FullName != null)
                        {
                            _cachedAbilityLogicToAbilityParamTypeMap[derivedType.FullName] = paramType;
                        }
                    }
                }
            }

            return _cachedAbilityLogicToAbilityParamTypeMap;
        }
        
        public static List<AbilityConfigAsset> GetAllAbilityConfigAssets()
        {
            var abilityConfigAssets = FindAll<AbilityConfigAsset>();
            return abilityConfigAssets;
        }
        #endregion
    }
}
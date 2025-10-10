using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using GAS.General;
using GAS.Runtime;
using GAS.RuntimeWithECS;
using Sirenix.OdinInspector;
using Sirenix.Utilities.Editor;
using UnityEditor;
using UnityEngine;

namespace GAS.Editor
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
        
        public static List<string> GetAllReadOnlyFieldNames(Type targetType)
        {
            // 绑定标志：获取实例字段（包括public和非public），但不包括静态字段
            BindingFlags bindingFlags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;
        
            // 获取所有字段，并通过IsInitOnly识别readonly字段
            FieldInfo[] allFields = targetType.GetFields(bindingFlags);
            List<string> readonlyFieldNames = new List<string>();

            foreach (FieldInfo field in allFields)
            {
                // 关键：IsInitOnly为true表示readonly字段
                if (field.IsInitOnly)
                {
                    readonlyFieldNames.Add(field.Name);
                }
            }

            return readonlyFieldNames;
        }

        public static int GetFrameRate()
        {
            return (int)Math.Round(1 / Time.fixedDeltaTime);
        }
        
        #region Ability

        private static IEnumerable<Type> _cachedAbilityComponentSubTypes;

        public static IEnumerable<Type> GetCachedAbilityComponentSubTypes()
        {
            if (_cachedAbilityComponentSubTypes != null) return _cachedAbilityComponentSubTypes;
            // var assemblies = AppDomain.CurrentDomain.GetAssemblies();
            // _cachedAbilityComponentSubTypes = assemblies
            //     .SelectMany(asm => asm.GetTypes())
            //     .Where(type =>
            //         type.IsSubclassOf(typeof(BaseGameplayAbilityComponentConfigAsset)) &&
            //         !type.IsAbstract &&
            //         type.IsDefined(typeof(SerializableAttribute), false)
            //     )
            //     .ToList();

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


        private static Dictionary<Type, Type> _cachedAbilityParamTypeToAbilityParamConfigTypeMap;
        

        private static Dictionary<string, Type> _cachedAbilityLogicToAbilityParamConfigTypeMap;
        
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
        #endregion
    }
}
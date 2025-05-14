using System;
using System.Collections.Generic;
using System.Linq;
using GAS.Runtime;
using GAS.RuntimeDataHelper.Helper;
using GAS.RuntimeWithECS.Cue;
using Sirenix.OdinInspector;

namespace GAS.Editor
{
    public static class EditCueHelper
    {
        private static Dictionary<Type, Type> _cachedCueParamTypeToCueParamConfigTypeMap;
        private static Dictionary<string, Type> _cachedCueToCueParamConfigTypeMap;
        private static IEnumerable<Type> _cachedCueParamConfigTypes;
        private static IEnumerable<Type> _cachedCueTypes;
        
        public static IEnumerable<Type> GetCachedCueParamConfigTypes()
        {
            if (_cachedCueParamConfigTypes != null) return _cachedCueParamConfigTypes;
            var assemblies = AppDomain.CurrentDomain.GetAssemblies();
            _cachedCueParamConfigTypes = assemblies
                .SelectMany(asm => asm.GetTypes())
                .Where(type =>
                    type.IsSubclassOf(typeof(CueParamConfigBase)) &&
                    !type.IsAbstract &&
                    type.IsDefined(typeof(SerializableAttribute), false)
                )
                .ToList();
            return _cachedCueParamConfigTypes;
        }
        
        public static Dictionary<Type, Type> GetCachedCueParamTypeToCueParamConfigTypeMap()
        {
            if (_cachedCueParamTypeToCueParamConfigTypeMap != null)
                return _cachedCueParamTypeToCueParamConfigTypeMap;
            var types = GetCachedCueParamConfigTypes();
            _cachedCueParamTypeToCueParamConfigTypeMap = new Dictionary<Type, Type>();
            foreach (var derivedType in types)
            {
                var baseType = derivedType.BaseType; // 获取基类类型

                if (baseType != null && baseType.IsGenericType)
                {
                    // 获取泛型类型定义（如 CueParamConfigBase<>）
                    var genericBaseDef = baseType.GetGenericTypeDefinition();

                    // 确认是否是所需的基类泛型定义
                    if (genericBaseDef == typeof(CueParamConfigBase<>))
                    {
                        var genericArgs = baseType.GetGenericArguments();
                        var paramType = genericArgs[0];

                        _cachedCueParamTypeToCueParamConfigTypeMap[paramType] = derivedType;
                    }
                }
            }

            return _cachedCueParamTypeToCueParamConfigTypeMap;
        }
        
        public static Dictionary<string, Type> GetCachedCueToCueParamConfigTypeMap()
        {
            if (_cachedCueToCueParamConfigTypeMap != null)
                return _cachedCueToCueParamConfigTypeMap;
            var cueTypes = GetCachedInstantCueTypes() ;
            _cachedCueToCueParamConfigTypeMap = new Dictionary<string, Type>();
            foreach (var derivedType in cueTypes)
            {
                var baseType = derivedType.BaseType; // 获取基类类型

                if (baseType != null && baseType.IsGenericType)
                {
                    // 获取泛型类型定义（如 ModMagnitudeCalculationBase<>）
                    var genericBaseDef = baseType.GetGenericTypeDefinition();

                    // 确认是否是所需的基类泛型定义
                    if (genericBaseDef == typeof(GameplayCueBase<>))
                    {
                        // 获取实际使用的泛型参数（如 MmcParamString）
                        var genericArgs = baseType.GetGenericArguments();
                        var paramType = genericArgs[0];

                        if (derivedType.FullName != null)
                        {
                            var param2ParamConfigMap = GetCachedCueParamTypeToCueParamConfigTypeMap();
                            if (!param2ParamConfigMap.TryGetValue(paramType, out var value))
                                EXEditorHelper.ShowNotification(
                                    $"未找到对应的能力参数配置，请检查类【{derivedType.FullName}】是否继承自CueParamConfigBase<T>");
                            else
                                _cachedCueToCueParamConfigTypeMap[derivedType.FullName] = value;
                        }
                    }
                }
            }

            return _cachedCueToCueParamConfigTypeMap;
        }
        
        public static IEnumerable<Type> GetCachedCueTypes()
        {
            if (_cachedCueTypes != null) return _cachedCueTypes;
            var assemblies = AppDomain.CurrentDomain.GetAssemblies();
            _cachedCueTypes = assemblies
                .SelectMany(asm => asm.GetTypes())
                .Where(type =>
                    type.IsSubclassOf(typeof(GameplayCueBase)) &&
                    !type.IsAbstract
                )
                .ToList();

            return _cachedCueTypes;
        }
        
        #region Instant Cue
        
        private static ValueDropdownItem[] _instantCueChoices;
        private static IEnumerable<Type> _cachedInstantCueTypes;

        public static IEnumerable<ValueDropdownItem> InstantCueChoices
        {
            get
            {
                if (_instantCueChoices == null || _instantCueChoices.Length == 0)
                {
                    var types = GetCachedInstantCueTypes();
                    _instantCueChoices = types
                        .Select(type => new ValueDropdownItem(type.Name, type.FullName))
                        .ToArray();
                }

                return _instantCueChoices;
            }
        }
        
        public static IEnumerable<Type> GetCachedInstantCueTypes()
        {
            if (_cachedInstantCueTypes != null) return _cachedInstantCueTypes;
            var assemblies = AppDomain.CurrentDomain.GetAssemblies();
            _cachedInstantCueTypes = assemblies
                .SelectMany(asm => asm.GetTypes())
                .Where(type =>
                    type.IsSubclassOf(typeof(GameplayCueBase)) &&
                    !type.IsAbstract
                )
                .ToList();

            return _cachedInstantCueTypes;
        }
        
        #endregion
    }
}
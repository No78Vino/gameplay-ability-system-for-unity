using System;
using System.Collections.Generic;
using System.Linq;
using GAS.RuntimeDataHelper.GameplayEffect;
using GAS.RuntimeDataHelper.GameplayEffect.MmcParam;
using GAS.RuntimeWithECS.Modifier;
using Sirenix.OdinInspector;

namespace GAS.RuntimeDataHelper.Helper
{
    public static class EditGameplayEffectHelper
    {
        private static ValueDropdownItem[] _effectComponentTypeChoices;


        private static IEnumerable<Type> _cachedEffectComponentSubTypes;


        private static IEnumerable<Type> _cachedMmcTypes;


        private static ValueDropdownItem[] _mmcChoices;


        private static IEnumerable<Type> _cachedMmcParamConfigTypes;

        private static Dictionary<Type, Type> _cachedMmcParamTypeToMmcParamConfigTypeMap;


        private static Dictionary<string, Type> _cachedMmcToMmcParamConfigTypeMap;

        public static IEnumerable<ValueDropdownItem> EffectComponentTypeChoices
        {
            get
            {
                _effectComponentTypeChoices ??= GetCachedEffectComponentSubTypes()
                    .Select(type => new ValueDropdownItem(type.Name, type.FullName))
                    .ToArray();
                return _effectComponentTypeChoices;
            }
        }

        public static IEnumerable<ValueDropdownItem> MmcChoices
        {
            get
            {
                if (_mmcChoices == null || _mmcChoices.Length == 0)
                {
                    var types = GetCachedMmcTypes();
                    _mmcChoices = types
                        .Select(type => new ValueDropdownItem(type.Name, type.FullName))
                        .ToArray();
                }

                return _mmcChoices;
            }
        }

        public static IEnumerable<Type> GetCachedEffectComponentSubTypes()
        {
            if (_cachedEffectComponentSubTypes != null) return _cachedEffectComponentSubTypes;
            var assemblies = AppDomain.CurrentDomain.GetAssemblies();
            _cachedEffectComponentSubTypes = assemblies
                .SelectMany(asm => asm.GetTypes())
                .Where(type =>
                    type.IsSubclassOf(typeof(BaseGameplayEffectComponentConfigAsset)) &&
                    !type.IsAbstract &&
                    type.IsDefined(typeof(SerializableAttribute), false)
                )
                .ToList();

            return _cachedEffectComponentSubTypes;
        }

        public static IEnumerable<Type> GetCachedMmcTypes()
        {
            if (_cachedMmcTypes != null) return _cachedMmcTypes;
            var assemblies = AppDomain.CurrentDomain.GetAssemblies();
            _cachedMmcTypes = assemblies
                .SelectMany(asm => asm.GetTypes())
                .Where(type =>
                    type.IsSubclassOf(typeof(ModMagnitudeCalculationBase)) &&
                    !type.IsAbstract
                )
                .ToList();

            return _cachedMmcTypes;
        }

        public static IEnumerable<Type> GetCachedMmcParamConfigTypes()
        {
            if (_cachedMmcParamConfigTypes != null) return _cachedMmcParamConfigTypes;
            var assemblies = AppDomain.CurrentDomain.GetAssemblies();
            _cachedMmcParamConfigTypes = assemblies
                .SelectMany(asm => asm.GetTypes())
                .Where(type =>
                    type.IsSubclassOf(typeof(MmcParamConfigBase)) &&
                    !type.IsAbstract &&
                    type.IsDefined(typeof(SerializableAttribute), false)
                )
                .ToList();
            return _cachedMmcParamConfigTypes;
        }

        public static Dictionary<Type, Type> GetCachedMmcParamTypeToMmcParamConfigTypeMap()
        {
            if (_cachedMmcParamTypeToMmcParamConfigTypeMap != null)
                return _cachedMmcParamTypeToMmcParamConfigTypeMap;
            var types = GetCachedMmcParamConfigTypes();
            _cachedMmcParamTypeToMmcParamConfigTypeMap = new Dictionary<Type, Type>();
            foreach (var derivedType in types)
            {
                var baseType = derivedType.BaseType; // 获取基类类型

                if (baseType != null && baseType.IsGenericType)
                {
                    // 获取泛型类型定义（如 MmcParamConfigBase<>）
                    var genericBaseDef = baseType.GetGenericTypeDefinition();

                    // 确认是否是所需的基类泛型定义
                    if (genericBaseDef == typeof(MmcParamConfigBase<>))
                    {
                        // 获取实际使用的泛型参数（如 MmcParamString）
                        var genericArgs = baseType.GetGenericArguments();
                        var paramType = genericArgs[0];

                        _cachedMmcParamTypeToMmcParamConfigTypeMap[paramType] = derivedType;
                    }
                }
            }

            return _cachedMmcParamTypeToMmcParamConfigTypeMap;
        }

        public static Dictionary<string, Type> GetCachedMmcToMmcParamConfigTypeMap()
        {
            if (_cachedMmcToMmcParamConfigTypeMap != null)
                return _cachedMmcToMmcParamConfigTypeMap;
            var types = GetCachedMmcTypes();
            _cachedMmcToMmcParamConfigTypeMap = new Dictionary<string, Type>();
            foreach (var derivedType in types)
            {
                var baseType = derivedType.BaseType; // 获取基类类型

                if (baseType != null && baseType.IsGenericType)
                {
                    // 获取泛型类型定义（如 ModMagnitudeCalculationBase<>）
                    var genericBaseDef = baseType.GetGenericTypeDefinition();

                    // 确认是否是所需的基类泛型定义
                    if (genericBaseDef == typeof(ModMagnitudeCalculationBase<>))
                    {
                        // 获取实际使用的泛型参数（如 MmcParamString）
                        var genericArgs = baseType.GetGenericArguments();
                        var paramType = genericArgs[0];

                        if (derivedType.FullName != null)
                        {
                            var param2ParamConfigMap = GetCachedMmcParamTypeToMmcParamConfigTypeMap();
                            if (!param2ParamConfigMap.ContainsKey(paramType))
                                EXEditorHelper.ShowNotification(
                                    $"未找到对应的能力参数配置，请检查类【{derivedType.FullName}】是否继承自MmcParamConfigBase<T>");
                            else
                                _cachedMmcToMmcParamConfigTypeMap[derivedType.FullName] =
                                    param2ParamConfigMap[paramType];
                        }
                    }
                }
            }

            return _cachedMmcToMmcParamConfigTypeMap;
        }
    }
}
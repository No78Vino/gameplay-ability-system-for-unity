using System;
using System.Collections.Generic;
using System.Linq;
using GAS.RuntimeWithECS.Cue;
using Sirenix.OdinInspector;

namespace GAS.Editor
{
    public static class EditCueHelper
    {
        private static Dictionary<Type, Type> _cachedMmcParamTypeToMmcParamConfigTypeMap;
        private static Dictionary<string, Type> _cachedCueToCueParamConfigTypeMap;
        
        public static Dictionary<string, Type> GetCachedCueToCueParamConfigTypeMap()
        {
            if (_cachedCueToCueParamConfigTypeMap != null)
                return _cachedCueToCueParamConfigTypeMap;
            var instantTypes = GetCachedInstantCueTypes();
            var durationalTypes = GetCachedDurationalCueTypes();
            _cachedCueToCueParamConfigTypeMap = new Dictionary<string, Type>();
            
            foreach (var derivedType in instantTypes)
            {
                var baseType = derivedType.BaseType; // 获取基类类型

                if (baseType != null && baseType.IsGenericType)
                {
                    // 获取泛型类型定义（如 ModMagnitudeCalculationBase<>）
                    var genericBaseDef = baseType.GetGenericTypeDefinition();

                    // 确认是否是所需的基类泛型定义
                    if (genericBaseDef == typeof(CueInstant<>))
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
                                _cachedCueToCueParamConfigTypeMap[derivedType.FullName] =
                                    param2ParamConfigMap[paramType];
                        }
                    }
                }
            }

            return _cachedCueToCueParamConfigTypeMap;
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
                    type.IsSubclassOf(typeof(CueInstant)) &&
                    !type.IsAbstract
                )
                .ToList();

            return _cachedInstantCueTypes;
        }
        
        #endregion
    }
}
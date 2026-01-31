using System;
using System.Collections.Generic;
using System.Linq;
using GAS.Runtime;

namespace GAS.Editor
{
    public static class EditorCueHelper
    {
        private static Dictionary<string, Type> _cachedCueToParamTypeMap;
        
        public static Dictionary<string, Type> CueToCueParamTypeMap()
        {
            if (_cachedCueToParamTypeMap != null)
                return _cachedCueToParamTypeMap;
            var cueTypes = GetCachedInstantCueTypes() ;
            _cachedCueToParamTypeMap = new Dictionary<string, Type>();
            foreach (var derivedType in cueTypes)
            {
                var baseType = derivedType.BaseType; // 获取基类类型

                if (baseType != null && baseType.IsGenericType)
                {
                    var genericBaseDef = baseType.GetGenericTypeDefinition();

                    // 确认是否是所需的基类泛型定义
                    if (genericBaseDef == typeof(GameplayCueBase<>))
                    {
                        // 获取实际使用的泛型参数（如 MmcParamString）
                        var genericArgs = baseType.GetGenericArguments();
                        var paramType = genericArgs[0];

                        if (derivedType.FullName != null) _cachedCueToParamTypeMap[derivedType.Name] = paramType;
                    }
                }
            }

            return _cachedCueToParamTypeMap;
        }

        public static XParam CreateCueParameter(string type, List<object> paramData = null)
        {
            var map = CueToCueParamTypeMap();
            if (!map.TryGetValue(type, out var cueParamConfigType))
                throw new KeyNotFoundException($"未找到类型为 {type} 的 XParam 类型。");
            var cueParamEditor = (XParam)Activator.CreateInstance(cueParamConfigType);
            if (paramData != null) cueParamEditor.DecodeExcelData(paramData);
            return cueParamEditor;
        }
        
        
        
        
        private static Dictionary<Type, Type> _cachedCueParamTypeToCueParamConfigTypeMap;
        private static Dictionary<string, Type> _cachedCueToCueParamConfigTypeMap;
        private static IEnumerable<Type> _cachedCueParamConfigTypes;
        private static IEnumerable<Type> _cachedCueTypes;
        
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
        
        public static IEnumerable<string> GetCachedCueTypeNames()
        {
            var types = GetCachedCueTypes();
            return types
                .Select(type => type.Name)
                .Where(name => !string.IsNullOrEmpty(name))
                .ToList();
        }
        #region Instant Cue
        
        private static IEnumerable<Type> _cachedInstantCueTypes;
        
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
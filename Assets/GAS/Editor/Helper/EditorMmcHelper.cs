using System.Collections.Generic;
using System;
using System.Linq;
using GAS.Runtime;

namespace GAS.Editor
{
    public static class EditorMmcHelper
    {
        private static Dictionary<string, Type> _cachedMmcToParamTypeMap;
        private static IEnumerable<Type> _cachedMmcTypes;
        
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
        
        public static IEnumerable<string> GetCachedMmcTypeNames()
        {
            var types = GetCachedMmcTypes();
            return types.Select(t => t.Name).ToList();
        }
        
        public static Dictionary<string, Type> MmcToMmcParamTypeMap()
        {
            if (_cachedMmcToParamTypeMap != null)
                return _cachedMmcToParamTypeMap;
            var cueTypes = GetCachedMmcTypes();
            _cachedMmcToParamTypeMap = new Dictionary<string, Type>();
            foreach (var derivedType in cueTypes)
            {
                var baseType = derivedType.BaseType; // 获取基类类型

                if (baseType != null && baseType.IsGenericType)
                {
                    var genericBaseDef = baseType.GetGenericTypeDefinition();

                    // 确认是否是所需的基类泛型定义
                    if (genericBaseDef == typeof(ModMagnitudeCalculationBase<>))
                    {
                        // 获取实际使用的泛型参数（如 MmcParamString）
                        var genericArgs = baseType.GetGenericArguments();
                        var paramType = genericArgs[0];

                        if (derivedType.FullName != null) _cachedMmcToParamTypeMap[derivedType.Name] = paramType;
                    }
                }
            }

            return _cachedMmcToParamTypeMap;
        }
        
        public static XParam CreateMmcParameter(string type, List<object> paramData = null)
        {
            var map = MmcToMmcParamTypeMap();
            if (!map.TryGetValue(type, out var mmcParamConfigType))
                throw new KeyNotFoundException($"未找到类型为 {type} 的 XParam 类型。");
            var mmcParamEditor = (XParam)Activator.CreateInstance(mmcParamConfigType);
            if (paramData != null) mmcParamEditor.DecodeExcelData(paramData);
            return mmcParamEditor;
        }
    }
}
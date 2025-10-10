using System;
using System.Collections.Generic;
using GAS.Runtime;
using GAS.RuntimeWithECS.Cue;

namespace GAS.Editor
{
    public static class EditorCueHelper
    {
        private static Dictionary<string, Type> _cachedCueToParamTypeMap;
        
        public static Dictionary<string, Type> CueToCueParamTypeMap()
        {
            if (_cachedCueToParamTypeMap != null)
                return _cachedCueToParamTypeMap;
            var cueTypes = EditCueHelper.GetCachedInstantCueTypes() ;
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

        public static ICueParameter CreateCueParameter(string type, List<object> paramData = null)
        {
            var map = CueToCueParamTypeMap();
            if (!map.TryGetValue(type, out var cueParamConfigType))
                throw new KeyNotFoundException($"未找到类型为 {type} 的 ICueParameter 类型。");
            var cueParamEditor = (ICueParameter)Activator.CreateInstance(cueParamConfigType);
            if (paramData != null) cueParamEditor.DecodeExcelData(paramData);
            return cueParamEditor;
        }
    }
}
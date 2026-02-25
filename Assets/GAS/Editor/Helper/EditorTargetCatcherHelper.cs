using System;  
using System.Collections.Generic;  
using System.Linq;  
using GAS.Runtime;  
  
namespace GAS.Editor  
{  
    public static class EditorTargetCatcherHelper  
    {  
        private static IEnumerable<Type> _cachedTargetCatcherTypes;  
  
        public static IEnumerable<Type> GetCachedTargetCatcherTypes()  
        {  
            if (_cachedTargetCatcherTypes != null) return _cachedTargetCatcherTypes;  
            var assemblies = AppDomain.CurrentDomain.GetAssemblies();  
            _cachedTargetCatcherTypes = assemblies  
                .SelectMany(asm => asm.GetTypes())  
                .Where(type =>  
                    type.IsSubclassOf(typeof(TargetCatcherBase)) &&  
                    !type.IsAbstract  
                )  
                .ToList();  
            return _cachedTargetCatcherTypes;  
        }  
  
        private static Dictionary<string, Type> _cachedCatcherToParamTypeMap;  
  
        public static Dictionary<string, Type> CatcherToParamTypeMap()  
        {  
            if (_cachedCatcherToParamTypeMap != null) return _cachedCatcherToParamTypeMap;  
            _cachedCatcherToParamTypeMap = new Dictionary<string, Type>();  
            var catcherTypes = GetCachedTargetCatcherTypes();  
            foreach (var catcherType in catcherTypes)  
            {  
                // 找泛型基类 TargetCatcherBase<T>，取 T 作为 ParamType  
                var baseType = catcherType.BaseType;  
                while (baseType != null)  
                {  
                    if (baseType.IsGenericType &&  
                        baseType.GetGenericTypeDefinition() == typeof(TargetCatcherBase<>))  
                    {  
                        var paramType = baseType.GetGenericArguments()[0];  
                        _cachedCatcherToParamTypeMap[catcherType.Name] = paramType;  
                        break;  
                    }  
                    baseType = baseType.BaseType;  
                }  
                // 如果没有泛型基类（直接继承非泛型 TargetCatcherBase），用 XParamNone  
                if (!_cachedCatcherToParamTypeMap.ContainsKey(catcherType.Name))  
                    _cachedCatcherToParamTypeMap[catcherType.Name] = typeof(XParamNone);  
            }  
            return _cachedCatcherToParamTypeMap;  
        }
    }  
}
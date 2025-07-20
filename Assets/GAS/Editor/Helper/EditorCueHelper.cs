using System;
using System.Collections.Generic;
using System.Linq;
using GAS.Runtime;
using GAS.RuntimeWithECS.Cue;

namespace GAS.Editor
{
    public static class EditorCueHelper
    {
        private static Dictionary<string, Type> _cachedCueToParamTypeMap;
        //private static IEnumerable<Type> _cachedCueParamEditorTypes;

        // public static IEnumerable<Type> GetCachedCueParamTypes()
        // {
        //     if (_cachedCueParamEditorTypes != null) return _cachedCueParamEditorTypes;
        //     var assemblies = AppDomain.CurrentDomain.GetAssemblies();
        //     _cachedCueParamEditorTypes = assemblies
        //         .SelectMany(asm => asm.GetTypes())
        //         .Where(type =>
        //             type.IsSubclassOf(typeof(CueParamEditorBase)) &&
        //             !type.IsAbstract
        //         )
        //         .ToList();
        //
        //     return _cachedCueParamEditorTypes;
        // }


        // public static Dictionary<string, Type> GetCachedCueToCueParamConfigTypeMap()
        // {
        //     if (_cachedCueToCueParamConfigTypeMap != null)
        //         return _cachedCueToCueParamConfigTypeMap;
        //     var cueTypes = GetCachedInstantCueTypes() ;
        //     _cachedCueToCueParamConfigTypeMap = new Dictionary<string, Type>();
        //     foreach (var derivedType in cueTypes)
        //     {
        //         var baseType = derivedType.BaseType; // 获取基类类型
        //
        //         if (baseType != null && baseType.IsGenericType)
        //         {
        //             // 获取泛型类型定义（如 ModMagnitudeCalculationBase<>）
        //             var genericBaseDef = baseType.GetGenericTypeDefinition();
        //
        //             // 确认是否是所需的基类泛型定义
        //             if (genericBaseDef == typeof(GameplayCueBase<>))
        //             {
        //                 // 获取实际使用的泛型参数（如 MmcParamString）
        //                 var genericArgs = baseType.GetGenericArguments();
        //                 var paramType = genericArgs[0];
        //
        //                 if (derivedType.FullName != null)
        //                 {
        //                     var param2ParamConfigMap = GetCachedCueParamTypeToCueParamConfigTypeMap();
        //                     if (!param2ParamConfigMap.TryGetValue(paramType, out var value))
        //                         EXEditorHelper.ShowNotification(
        //                             $"未找到对应的能力参数配置，请检查类【{derivedType.FullName}】是否继承自CueParamConfigBase<T>");
        //                     else
        //                         _cachedCueToCueParamConfigTypeMap[derivedType.FullName] = value;
        //                 }
        //             }
        //         }
        //     }
        //
        //     return _cachedCueToCueParamConfigTypeMap;
        // }
        
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

        public static ICueParameter CreateCueParamEditor(string type, List<object> paramData = null)
        {
            var map = CueToCueParamTypeMap();
            if (!map.TryGetValue(type, out var cueParamConfigType))
                throw new KeyNotFoundException($"未找到类型为 {type} 的 CueParamEditor 类型。");
            var cueParamEditor = (ICueParameter)Activator.CreateInstance(cueParamConfigType);
            if (paramData != null) cueParamEditor.DecodeExcelData(paramData);
            return cueParamEditor;
        }

        public static Type GetCueParamEditorType(string type)
        {
            var map = CueToCueParamTypeMap();
            if (!map.TryGetValue(type, out var cueParamConfigType))
                throw new KeyNotFoundException($"未找到类型为 {type} 的 CueParamEditor 类型。");

            return cueParamConfigType;
        }
    }
}
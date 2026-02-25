using System;  
using System.Collections.Generic;  
using UnityEngine;

namespace GAS.Runtime
{
    public static class TargetCatcherHelper
    {
        private static readonly Dictionary<string, Type> CatcherTypeMap = new();
        private static readonly Dictionary<string, Type> CatcherParamTypeMap = new();
        private static readonly Dictionary<string, string> CatcherType2ParamTypeMap = new();

        public static void RegisterTargetCatcher(string sType, Type catcherType, Type catcherParamType)
        {
            CatcherTypeMap[sType] = catcherType;
            CatcherParamTypeMap[catcherParamType.Name] = catcherParamType;
            CatcherType2ParamTypeMap[sType] = catcherParamType.Name;
        }

        public static TargetCatcherBase TryCreateTargetCatcher(string catcherType)
        {
            if (CatcherTypeMap.TryGetValue(catcherType, out var type))
                try
                {
                    return Activator.CreateInstance(type) as TargetCatcherBase;
                }
                catch (MissingMethodException e)
                {
                    Debug.LogError("[EX] 创建TargetCatcher失败: " +
                                   $"请检查这个类【'{type.FullName}'】是否继承自TargetCatcherBase;" +
                                   "或者，TargetCatcher的Type映射脚本是否更新，重新生成。" +
                                   $"Error Exception:{e.Message}");
                    throw;
                }
#if UNITY_EDITOR
            Debug.LogError($"[EX] 创建TargetCatcher失败:Can't find TargetCatcher for catcherType [{catcherType}]. " +
                           "TargetCatcher的Type映射脚本错误，请重新生成。");
#endif
            return null;
        }

        public static Type GetCatcherParamType(string catcherTypeName)
        {
            var paramName = CatcherType2ParamTypeMap[catcherTypeName];
            return CatcherParamTypeMap[paramName];
        }

        public static IEnumerable<string> GetCatcherTypeNames()
        {
            return CatcherTypeMap.Keys;
        }

        /// 对标 CueHelper.CreateCueParameter，用于 OnTypeChange 时创建默认参数实例  
        public static XParam CreateCatcherParameter(string catcherType)
        {
            if (string.IsNullOrEmpty(catcherType)) return null;
            if (!CatcherType2ParamTypeMap.TryGetValue(catcherType, out var paramName)) return null;
            if (!CatcherParamTypeMap.TryGetValue(paramName, out var paramType)) return null;
            return Activator.CreateInstance(paramType) as XParam;
        }
    }
}
using System;
using System.Collections.Generic;
using GAS.Editor;
using GAS.RuntimeWithECS.Cue;
using UnityEngine;

namespace GAS.Runtime
{
    public static class CueHelper
    {
        public static CueInstant TryCreateInstantCue(string cueType, CueParamConfigBase param)
        {
            return TryCreateCue( cueType,  param) as CueInstant;
        }

        public static CueDurational TryCreateDurationalCue(string cueType, CueParamConfigBase param)
        {
            return TryCreateCue(cueType, param) as CueDurational;
        }

        public static NewGameplayCueBase TryCreateCue(string cueType, CueParamConfigBase param)
        {
            return TryCreateCue(cueType, param.GetConfig());
        }
        
        public static NewGameplayCueBase TryCreateCue(string cueType, ICueParameter param)
        {
            if (CueTypeMap.TryGetValue(cueType, out var type))
                try
                {
                    if (Activator.CreateInstance(type) is NewGameplayCueBase cue)
                    {
                        cue.InitParameters(param);
                        return cue;
                    }
                }
                catch (MissingMethodException e)
                {
                    Debug.LogError("[EX] 创建Cue失败: " +
                                   $"请检查这个类【'{type.FullName}'】是否继承自NewGameplayCueBase;" +
                                   "或者，ModMagnitudeCalculation的Type映射脚本是否更新，重新生成。" +
                                   $"Error Exception:{e.Message}");
                    throw;
                }
#if UNITY_EDITOR
            Debug.LogError($"[EX] 创建Cue失败:Can't find Cue for cueType [{cueType}]. " +
                           "Cue的Type映射脚本错误，请重新生成。");
#endif

            return null;
        }

        #region Cue

        private static readonly Dictionary<string, Type> CueTypeMap = new();

        public static void RegisterCue(string sType, Type logicType)
        {
            CueTypeMap[sType] = logicType;
        }

        public static Type GetMmcType(string sType)
        {
            return CueTypeMap[sType];
        }

        public static void RegisterCue<T>(string sType) where T : NewGameplayCueBase
        {
            RegisterCue(sType, typeof(T));
        }

        #endregion
    }
}
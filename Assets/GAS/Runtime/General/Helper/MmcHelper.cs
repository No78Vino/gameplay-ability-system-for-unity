using System;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

namespace GAS.Runtime
{
    public static class MmcHelper
    {
        public static ModMagnitudeCalculationBase TryCreateMmc(string mmcType, XParam param)
        {
            var type = GetMmcType(mmcType);
            return type != null ? TryCreateMmc(type, param) : null;
        }

        public static ModMagnitudeCalculationBase TryCreateMmc(Type type, XParam param)
        {
            try
            {
                var mmc = Activator.CreateInstance(type) as ModMagnitudeCalculationBase;
                if (mmc != null)
                {
                    mmc.InitParameters(param);
                    return mmc;
                }
            }
            catch (MissingMethodException e)
            {
                Debug.LogError("[EX] 创建MMC失败: " +
                               $"请检查这个类【'{type.FullName}'】是否继承自ModMagnitudeCalculationBase;" +
                               "或者，ModMagnitudeCalculation的Type映射脚本是否更新，重新生成。" +
                               $"Error Exception:{e.Message}");
                throw;
            }
#if UNITY_EDITOR
            Debug.LogError($"[EX] 创建MMC失败:Can't find ModMagnitudeCalculation for mmcType [{type.Name}]. " +
                           "ModMagnitudeCalculation的Type映射脚本错误，请重新生成。");
#endif
            return null;
        }

        public static float Calculate(Entity ge, EffectModifier modifier, float sourceValue)
        {
            var result = sourceValue;
            var magnitude = modifier.MMC.CalculateMagnitude(ge, modifier.Magnitude);
            switch (modifier.Operation)
            {
                case GEOperation.Add:
                    result += magnitude;
                    break;
                case GEOperation.Minus:
                    result -= magnitude;
                    break;
                case GEOperation.Multiply:
                    result *= magnitude;
                    break;
                case GEOperation.Divide:
                    result /= magnitude;
                    break;
                case GEOperation.Override:
                    result = magnitude;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            return result;
        }

        #region MMC

        private static readonly Dictionary<string, Type> MmcTypeMap = new();
        private static readonly Dictionary<string, Type> MmcParamTypeMap = new();
        private static readonly Dictionary<string, string> MmcType2MmcParamTypeMap = new();
        
        public static void RegisterMmc(string sType, Type logicType,Type mmcParamType)
        {
            MmcTypeMap[sType] = logicType;
            MmcParamTypeMap[mmcParamType.Name] = mmcParamType;
            MmcType2MmcParamTypeMap[sType] = mmcParamType.Name;
        }

        public static Type GetMmcType(string sType)
        {
            return MmcTypeMap.GetValueOrDefault(sType);
        }

        public static Type GetMmcParamTypeByMmcType(Type mmcType)
        {
            var cueParam = MmcType2MmcParamTypeMap[mmcType.Name];
            return MmcParamTypeMap[cueParam];
        }
        
        public static Type GetMmcParamTypeByMmcType(string mmcType)
        {
            var cueParam = MmcType2MmcParamTypeMap[mmcType];
            return MmcParamTypeMap[cueParam];
        }
        
        public static void RegisterMmc<T>(string sType,Type mmcParam) where T : ModMagnitudeCalculationBase
        {
            RegisterMmc(sType, typeof(T),mmcParam);
        }

        
        #endregion
    }
}
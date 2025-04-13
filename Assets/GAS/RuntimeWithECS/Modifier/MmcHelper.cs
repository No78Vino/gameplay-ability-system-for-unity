using System;
using System.Collections.Generic;
using GAS.RuntimeDataHelper.GameplayEffect.MmcParam;
using GAS.RuntimeWithECS.GameplayEffect.Component;
using GAS.RuntimeWithECS.Modifier;
using GAS.RuntimeWithECS.Modifier.CommonUsage;
using Unity.Entities;
using UnityEngine;

namespace GAS.Runtime
{
    public static class MmcHelper
    {
        #region MMC
        private static readonly Dictionary<string, Type> MmcTypeMap = new();

        public static void RegisterMmc(string sType, Type logicType)
        {
            MmcTypeMap[sType] = logicType;
        }

        public static Type GetMmcType(string sType)
        {
            return MmcTypeMap[sType];
        }

        public static void RegisterMmc<T>(string sType) where T : ModMagnitudeCalculationBase
        {
            RegisterMmc(sType, typeof(T));
        }
        #endregion

        public static ModMagnitudeCalculationBase TryCreateMmc(string mmcType, MmcParamConfigBase param)
        {
            return TryCreateMmc(mmcType, param.GetConfig());
        }

        public static ModMagnitudeCalculationBase TryCreateMmc(string mmcType,IMmcParameter param)
        {
            if (MmcTypeMap.TryGetValue(mmcType, out var type))
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
            Debug.LogError($"[EX] 创建MMC失败:Can't find ModMagnitudeCalculation for mmcType [{mmcType}]. " +
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
        
        // public static ModMagnitudeCalculationBase CreateMmc(string mmcType,MmcParamConfigBase mmcParamConfig)
        // {
        //     var mmc =  
        //     if (mmc == null)
        //     {
        //         throw new ArgumentException($"No MMCCalculation found for type {mmcType}");
        //     }
        //     var mmcInstance = mmc.CreateMmc();
        //     mmcInstance.SetConfAssetMmc(mmcParamConfig);
        //     return mmcInstance;
        // }
    }
    

}
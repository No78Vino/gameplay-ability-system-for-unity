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
                if (Activator.CreateInstance(type) is ModMagnitudeCalculationBase mmc)
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
        
        // 框架内部方法，用户不直接调用  
        public static float Calculate(Entity ge, EffectModifier modifier, float sourceValue)  
        {  
            // 在这里构建 MmcContext，隔离 ECS 细节  
            var context = BuildMmcContext(ge);  
            return modifier.Apply(context, sourceValue);  
        }  
  
        /// <summary>  
        /// 指定 Source/Target 的 Calculate 重载，用于 Prototype GE（没有 CEffectInUsage）的场景  
        /// </summary>  
        public static float Calculate(Entity ge, EffectModifier modifier, float sourceValue, Entity sourceAsc, Entity targetAsc)  
        {  
            var context = BuildMmcContext(ge, sourceAsc, targetAsc);  
            return modifier.Apply(context, sourceValue);  
        }  
  
        public static MmcContext BuildMmcContext(Entity ge, Entity sourceAsc, Entity targetAsc)  
        {  
            return new MmcContext  
            {  
                EffectSpec = new GameplayEffectSpec(ge),  
                Source = GASManager.GetAscFromEntity(sourceAsc),  
                Target = GASManager.GetAscFromEntity(targetAsc),  
            };  
        }
        
        public static MmcContext BuildMmcContext(Entity ge)  
        {  
            // 从 GE Entity 上读取 Source/Target ASC Entity  
            // 再通过 GASManager 的 Entity→AbilitySystemCell 映射转换为 OOP 包装  
            var geData = EntityHelper.GetComponentData<CEffectInUsage>(ge);
            return new MmcContext  
            {  
                EffectSpec = new GameplayEffectSpec(ge),  
                Source = GASManager.GetAscFromEntity(geData.Source),  
                Target = GASManager.GetAscFromEntity(geData.Target),  
            };  
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
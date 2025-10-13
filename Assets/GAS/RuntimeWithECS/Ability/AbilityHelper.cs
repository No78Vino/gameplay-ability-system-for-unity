using System;
using System.Collections.Generic;
using GAS.RuntimeWithECS.ComponentConfig;
using Unity.Entities;
using UnityEngine;

namespace GAS.Runtime
{
    public static class AbilityHelper
    {
        #region AbilityLogic
        private static readonly Dictionary<string, Type> AbilityLogicMap = new();
        private static readonly Dictionary<string, Type> AbilityLogicParamTypeMap = new();
        private static readonly Dictionary<string, string> AbilityType2AbilityParamTypeMap = new();
        
        public static void RegisterAbilityLogic(string sType, Type logicType,Type abilityParamType)
        {
            AbilityLogicMap[sType] = logicType;
            AbilityLogicParamTypeMap[abilityParamType.Name] = abilityParamType;
            AbilityType2AbilityParamTypeMap[sType] = abilityParamType.Name;
        }

        public static Type GetAbilityLogicType(string sType)
        {
            return AbilityLogicMap[sType];
        }

        public static AbilityLogicBase TryCreateAbilityLogic(string logicType,Entity ability)
        {
            if (AbilityLogicMap.TryGetValue(logicType, out var type))
                try
                {
                    var abilityLogic = Activator.CreateInstance(type,ability) as AbilityLogicBase;
                    return abilityLogic;
                }
                catch (MissingMethodException e)
                {
                    Debug.LogError("[EX] 创建能力失败: " +
                                   $"请检查这个类【'{type.FullName}'】是否继承自AbilityLogicBase;" +
                                   "或者，AbilityLogic的Type映射脚本是否更新，重新生成。" +
                                   $"Error Exception:{e.Message}");
                    throw;
                }
#if UNITY_EDITOR
            Debug.LogError($"[EX] 创建AbilityLogic失败:Can't find AbilityLogic for logicType [{logicType}]. " +
                           "AbilityLogic的Type映射脚本错误，请重新生成。");
#endif

            return null;
        }

        public static Type GetAbilityLogicParamType(string abilityLogicName)
        {
            var abilityParam = AbilityType2AbilityParamTypeMap[abilityLogicName];
            return AbilityLogicParamTypeMap[abilityParam];
        }
        
        #endregion
        
        public static Entity CreateAbilityEntity(GameplayAbilityComponentConfig[] configs)
        {
            var entity = GASManager.EntityManager.CreateEntity();
            GASManager.EntityManager.SetName(entity, $"Ability_{entity.ToString()}");
            foreach (var config in configs)
                config.LoadToGameplayAbilityEntity(entity);
            
            return entity;
        }
        
    }
}
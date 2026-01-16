using System;
using System.Collections.Generic;
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
        
        #region AbilityTask
        private static readonly Dictionary<string, Type> AbilityTaskMap = new();
        private static readonly Dictionary<string, Type> AbilityTaskParamTypeMap = new();
        private static readonly Dictionary<string, string> AbilityTaskType2AbilityTaskParamTypeMap = new();
        
        public static void RegisterAbilityTask(string sType, Type taskType,Type taskParamType)
        {
            AbilityTaskMap[sType] = taskType;
            AbilityTaskParamTypeMap[taskParamType.Name] = taskParamType;
            AbilityTaskType2AbilityTaskParamTypeMap[sType] = taskParamType.Name;
        }

        public static Type GetAbilityTaskType(string sType)
        {
            return AbilityTaskMap[sType];
        }

        public static AbilityTaskBase TryCreateAbilityTask(string taskType,AbilityLogicBase abilityLogic)
        {
            if (AbilityTaskMap.TryGetValue(taskType, out var type))
                try
                {
                    var abilityTask = Activator.CreateInstance(type,abilityLogic) as AbilityTaskBase;
                    return abilityTask;
                }
                catch (MissingMethodException e)
                {
                    Debug.LogError("[EX] 创建能力Task失败: " +
                                   $"请检查这个类【'{type.FullName}'】是否继承自AbilityTaskBase;" +
                                   "或者，AbilityTask的Type映射脚本是否更新，重新生成。" +
                                   $"Error Exception:{e.Message}");
                    throw;
                }
#if UNITY_EDITOR
            Debug.LogError($"[EX] 创建AbilityTask失败:Can't find AbilityTask for taskType [{taskType}]. " +
                           "AbilityTask的Type映射脚本错误，请重新生成。");
#endif

            return null;
        }

        public static Type GetAbilityTaskParamType(string abilityTaskName)
        {
            var abilityParam = AbilityTaskType2AbilityTaskParamTypeMap[abilityTaskName];
            return AbilityTaskParamTypeMap[abilityParam];
        }
        
        #endregion
        
        public static Entity CreateAbilityEntity(AbilityComponentConfig[] configs)
        {
            var entity = GASManager.EntityManager.CreateEntity();
            GASManager.EntityManager.SetName(entity, $"Ability_{entity.ToString()}");
            foreach (var config in configs)
                config.LoadToGameplayAbilityEntity(entity);
            
            return entity;
        }
        
    }
}
using System;
using System.Collections.Generic;
using GAS.RuntimeWithECS.Ability.Component;
using GAS.RuntimeWithECS.Ability.ComponentConfig;
using GAS.RuntimeWithECS.Core;
using Unity.Entities;
using UnityEngine;

namespace GAS.RuntimeWithECS.Ability
{
    public static class AbilityHelper
    {
        #region AbilityLogic
        private static readonly Dictionary<string, Type> AbilityLogicMap = new();

        public static void RegisterAbilityLogic(string sType, Type logicType)
        {
            AbilityLogicMap[sType] = logicType;
        }

        public static Type GetAbilityLogicType(string sType)
        {
            return AbilityLogicMap[sType];
        }

        public static void RegisterAbilityLogic<T>(string sType) where T : AbilityLogicBase
        {
            RegisterAbilityLogic(sType, typeof(T));
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
        #endregion
        
        public static Entity CreateAbilityEntity(GameplayAbilityComponentConfig[] configs)
        {
            var entity = GASManager.EntityManager.CreateEntity();

            foreach (var config in configs)
                config.LoadToGameplayAbilityEntity(entity);
            return entity;
        }
        
    }
}
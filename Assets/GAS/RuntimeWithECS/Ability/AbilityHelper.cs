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
        private static readonly Dictionary<int, Type> AbilityLogicMap = new();

        public static void RegisterAbilityLogic(int code, Type logicType)
        {
            AbilityLogicMap[code] = logicType;
        }

        public static Type GetAbilityLogicType(int code)
        {
            return AbilityLogicMap[code];
        }

        public static void RegisterAbilityLogic<T>(int code) where T : AbilityLogicBase
        {
            RegisterAbilityLogic(code, typeof(T));
        }

        public static AbilityLogicBase TryCreateAbilityLogic(int code)
        {
            if (AbilityLogicMap.TryGetValue(code, out var type))
                try
                {
                    var ability = Activator.CreateInstance(type) as AbilityLogicBase;
                    return ability;
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
            Debug.LogError($"[EX] 创建AbilityLogic失败:Can't find AbilityLogic for code [{code}]. " +
                           "AbilityLogic的Type映射脚本错误，请重新生成。");
#endif

            return null;
        }
        
        public static Entity CreateAbilityEntity(GameplayAbilityComponentConfig[] configs)
        {
            var entity = GASManager.EntityManager.CreateEntity();

            foreach (var config in configs)
                config.LoadToGameplayAbilityEntity(entity);
            return entity;
        }
    }
}
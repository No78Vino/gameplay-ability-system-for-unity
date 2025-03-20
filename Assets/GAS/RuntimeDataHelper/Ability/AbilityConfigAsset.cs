using System;
using System.Collections.Generic;
using System.Linq;
using GAS.RuntimeDataHelper.Ability.AbilityComponentConfigAsset;
using GAS.RuntimeWithECS.Ability;
using GAS.RuntimeWithECS.Ability.ComponentConfig;
using Sirenix.OdinInspector;
using UnityEngine;

namespace GAS.RuntimeDataHelper.Ability
{
    [CreateAssetMenu(fileName = "AbilityConfigAsset", menuName = "EX-GAS/Ability", order = 0)]
    public class AbilityConfigAsset : ScriptableObject
    {
        [ShowInInspector]
        [InlineProperty()]
        [ListDrawerSettings(Expanded = true, DraggableItems = true)]
        [TypeFilter("GetFilteredTypes")]
        [ValidateInput("ValidateNoDuplicateTypes", "列表中不能有重复的子类类型！")]
        public List<BaseGameplayAbilityComponentConfigAsset> componentConfigs = 
            new()
            {
                new ConfAssetAbilityBaseInfo()
            };

        #region 编辑器工具
        // 动态获取所有子类类型
        private static IEnumerable<Type> _cachedTypes;
        private IEnumerable<Type> GetFilteredTypes() {
            var allSubTypes = GetCachedSubTypes();
            var existingTypes = componentConfigs
                .Where(item => item != null)
                .Select(item => item.GetType())
                .ToHashSet();
            return allSubTypes.Where(type => !existingTypes.Contains(type));
        }

        // 反射缓存所有子类类型
        private static IEnumerable<Type> _cachedSubTypes;
        private IEnumerable<Type> GetCachedSubTypes() {
            if (_cachedSubTypes == null) {
                var assemblies = AppDomain.CurrentDomain.GetAssemblies();
                _cachedSubTypes = assemblies
                    .SelectMany(asm => asm.GetTypes())
                    .Where(type => 
                        type.IsSubclassOf(typeof(BaseGameplayAbilityComponentConfigAsset)) &&
                        !type.IsAbstract &&
                        type.IsDefined(typeof(SerializableAttribute), false)
                    )
                    .ToList();
            }
            return _cachedSubTypes;
        }

        // 验证列表内容
        private bool ValidateNoDuplicateTypes(List<BaseGameplayAbilityComponentConfigAsset> list) {
            var existingTypes = new HashSet<Type>();
            foreach (var item in list) {
                if (item == null) continue;
                var type = item.GetType();
                if (!existingTypes.Add(type)) return false;
            }
            return true;
        }
        #endregion
        
        
        
        public AbilityConfig GetConfig()
        {
            List<GameplayAbilityComponentConfig> configs = new List<GameplayAbilityComponentConfig>();
            foreach (var config in componentConfigs)
            {
                configs.Add(config.GetConfig());
            }
            return new AbilityConfig(configs.ToArray());
        }
    }
}
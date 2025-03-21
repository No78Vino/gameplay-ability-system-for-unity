using System;
using System.Collections.Generic;
using System.Linq;
using GAS.RuntimeDataHelper.Ability.AbilityComponentConfigAsset;
using GAS.RuntimeWithECS.Ability;
using GAS.RuntimeWithECS.Ability.ComponentConfig;
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using UnityEngine;

namespace GAS.RuntimeDataHelper.Ability
{
    [CreateAssetMenu(fileName = "AbilityConfigAsset", menuName = "EX-GAS/Ability", order = 0)]
    public class AbilityConfigAsset : ScriptableObject
    {
        [ShowInInspector]
        [InlineProperty()]
        [TypeFilter("GetFilteredTypes")]
        //[ValidateInput("ValidateNoDuplicateTypes", "列表中不能有重复的子类类型！")]
        [ListDrawerSettings(
            Expanded = true,
            DraggableItems = true,
            OnBeginListElementGUI = "OnBeginElementGUI",
            OnEndListElementGUI = "OnEndElementGUI"
        )]
        [ValidateInput("ValidateListContainsClassB", "列表必须包含一个ClassB元素！")]
        public List<BaseGameplayAbilityComponentConfigAsset> componentConfigs =
            new();

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
      
        
    //--------------------------
    // 初始化时强制添加ClassB
    //--------------------------
    [OnInspectorInit]
    private void InitializeList() {
        // 确保列表初始化时至少有一个ClassB
        if (!componentConfigs.Any(item => item is ConfAssetAbilityBaseInfo))
        {
            componentConfigs.Add(new ConfAssetAbilityBaseInfo());
        }
    }
    

    //--------------------------
    // 禁止删除ClassB的逻辑
    //--------------------------
    private void OnBeginElementGUI(int index) {
        var item = componentConfigs[index];
        if (item is ConfAssetAbilityBaseInfo) {
            // 禁用删除按钮（通过隐藏并占位）
            GUILayout.Label("不可删除", GUILayout.Width(60));
            GUILayout.Space(-24); // 偏移按钮位置
        }
    }

    private void OnEndElementGUI(int index) {
        var item = componentConfigs[index];
        if (item is ConfAssetAbilityBaseInfo) {
            // 恢复布局防止错位
            GUILayout.Space(24);
        }
    }

    //--------------------------
    // 验证列表是否包含ClassB
    //--------------------------
    private bool ValidateListContainsClassB(List<BaseGameplayAbilityComponentConfigAsset> list) {
        return list.Any(item => item is ConfAssetAbilityBaseInfo);
    }

    //--------------------------
    // 类型缓存（反射获取所有子类）
    //--------------------------
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

    //--------------------------
    // 当尝试删除ClassB时的拦截逻辑
    //--------------------------
    [OnCollectionChanged("OnListChanged")]
    private void OnListChanged(CollectionChangeInfo info, object value) {
        if (info.ChangeType == CollectionChangeType.RemoveValue) {
            var removedItem = info.Value as BaseGameplayAbilityComponentConfigAsset;
            if (removedItem is ConfAssetAbilityBaseInfo) {
                Debug.LogWarning("禁止删除ClassB元素！");
                // 恢复被删除的ClassB
                componentConfigs.Insert(info.Index, removedItem);
            }
        }
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
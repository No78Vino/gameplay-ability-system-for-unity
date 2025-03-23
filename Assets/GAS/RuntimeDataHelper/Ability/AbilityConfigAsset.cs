using System;
using System.Collections.Generic;
using System.Linq;
using GAS.RuntimeDataHelper.Ability.AbilityComponentConfigAsset;
using GAS.RuntimeDataHelper.Helper;
using GAS.RuntimeWithECS.Ability;
using GAS.RuntimeWithECS.Ability.ComponentConfig;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;

namespace GAS.RuntimeDataHelper.Ability
{
    [CreateAssetMenu(fileName = "AbilityConfigAsset", menuName = "EX-GAS/Ability", order = 0)]
    public class AbilityConfigAsset : ScriptableObject
    {
        [ShowInInspector]
        [InlineProperty]
        [TypeFilter(nameof(GetFilteredTypes))]
        [ListDrawerSettings(
            DraggableItems = true,
            CustomRemoveElementFunction = nameof(OnListRemove),
            ShowItemCount = true,
            ShowPaging = true,
            ShowFoldout = false)]
        [ValidateInput(nameof(ValidateList), "列表必须包含一个ClassB元素!\n列表中不能有重复的子类类型！")]
        [LabelText("能力组件配置")]
        public List<BaseGameplayAbilityComponentConfigAsset> ComponentConfigs =
            new();


        public AbilityConfig GetConfig()
        {
            var configs = new List<GameplayAbilityComponentConfig>();
            foreach (var config in ComponentConfigs) configs.Add(config.GetConfig());
            return new AbilityConfig(configs.ToArray());
        }

        #region 编辑器工具
        private IEnumerable<Type> GetFilteredTypes() => EXEditorHelper.GetCachedAbilityComponentSubTypes();

        [OnInspectorInit]
        private void InitializeList()
        {
            // 确保列表初始化时至少有一个ConfAssetAbilityBaseInfo
            if (!ComponentConfigs.Any(item => item is ConfAssetAbilityBaseInfo))
                ComponentConfigs.Add(new ConfAssetAbilityBaseInfo());
            // 确保列表初始化时至少有一个ConfAssetAbilityBaseInfo
            if (!ComponentConfigs.Any(item => item is ConfAssetAbilityBaseInfo))
                ComponentConfigs.Add(new ConfAssetAbilityBaseInfo());
        }

        private bool ValidateList(List<BaseGameplayAbilityComponentConfigAsset> list)
        {
            var existingTypes = new HashSet<Type>();
            foreach (var item in list)
            {
                if (item == null) continue;
                var type = item.GetType();
                if (!existingTypes.Add(type)) return false;
            }

            return list.Any(item => item is ConfAssetAbilityBaseInfo);
        }


        private static IEnumerable<Type> _cachedSubTypes;

        private IEnumerable<Type> GetCachedSubTypes()
        {
            if (_cachedSubTypes == null)
            {
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

        private bool OnListRemove(BaseGameplayAbilityComponentConfigAsset element)
        {
            if (element is ConfAssetAbilityBaseInfo)
            {
                Debug.LogWarning("禁止删除组件【ConfAssetAbilityBaseInfo】！");
                EXEditorHelper.ShowNotification("禁止删除组件【ConfAssetAbilityBaseInfo】！");
                return false; // 返回false表示阻止删除
            }
            return true; // 允许删除其他类型
        }

        #endregion
    }
}
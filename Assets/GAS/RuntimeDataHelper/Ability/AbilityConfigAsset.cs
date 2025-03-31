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
using MCConfAssetAbilityLogic = GAS.RuntimeDataHelper.Ability.AbilityComponentConfigAsset.MCConfAssetAbilityLogic;

namespace GAS.RuntimeDataHelper.Ability
{
    [CreateAssetMenu(fileName = "AbilityConfigAsset", menuName = "EX-GAS/Ability", order = 0)]
    public class AbilityConfigAsset : GEN_AbilityConfigtSO
    {
        // [ShowInInspector]
        // [InlineProperty]
        // [TypeFilter(nameof(GetFilteredTypes))]
        // [ListDrawerSettings(
        //     DraggableItems = true,
        //     CustomRemoveElementFunction = nameof(OnListRemove),
        //     ShowItemCount = true,
        //     ShowPaging = true,
        //     ShowFoldout = false)]
        // [ValidateInput(nameof(ValidateList), ContinuousValidationCheck = true)]
        // [LabelText("能力组件配置")]
        // [OnValueChanged(nameof(OnValueChange))]
        // [PropertyOrder(0)]
        
        
        public List<BaseGameplayAbilityComponentConfigAsset> ComponentConfigs;
        
        public AbilityConfig GetConfig()
        {
            // var configs = new List<GameplayAbilityComponentConfig>();
            // foreach (var config in ComponentConfigs) configs.Add(config.GetConfig());
            // return new AbilityConfig(configs.ToArray());
            return new AbilityConfig(new GameplayAbilityComponentConfig[]{});
        }

        #region 编辑器工具

        // private IEnumerable<Type> GetFilteredTypes()
        // {
        //     var types = EXEditorHelper.GetCachedAbilityComponentSubTypes();
        //     // 排除掉已有的类型
        //     var existingTypes = new HashSet<Type>(ComponentConfigs.Select(item => item.GetType()));
        //     return types.Where(type => !existingTypes.Contains(type));
        // }

        [OnInspectorInit]
        private void InitializeList()
        {
            // ComponentConfigs = GetCurrentData();
            // // 确保列表初始化时必须有一个ConfAssetAbilityBaseInfo
            // // 确保列表初始化时必须有一个MCConfAssetAbilityLogic
            // var isChanged = false;
            // if (!ComponentConfigs.Any(item => item is ConfAssetAbilityBaseInfo))
            // {
            //     ComponentConfigs.Insert(0, new ConfAssetAbilityBaseInfo());
            //     isChanged = true;
            // }
            //
            // if (!ComponentConfigs.Any(item => item is MCConfAssetAbilityLogic))
            // {
            //     ComponentConfigs.Insert(1, new MCConfAssetAbilityLogic());
            //     isChanged = true;
            // }
            //
            // if (isChanged) OnValueChange();
        }

        private bool ValidateList(List<BaseGameplayAbilityComponentConfigAsset> list, ref string errorMsg)
        {
            var messages = new List<string>();
            var existingTypes = new HashSet<Type>();
            foreach (var item in list)
            {
                if (item == null) continue;
                var type = item.GetType();
                if (!existingTypes.Add(type))
                {
                    messages.Add($"列表中不能有重复的子类类型：{type.Name}");
                    break;
                }
            }

            if (!list.Any(item => item is ConfAssetAbilityBaseInfo))
                messages.Add("列表必须包含一个ConfAssetAbilityBaseInfo元素！");
            if (!list.Any(item => item is MCConfAssetAbilityLogic))
                messages.Add("列表必须包含一个MCConfAssetAbilityLogic元素！");

            errorMsg = messages.Count > 0 ? string.Join("\n", messages) : null;
            return messages.Count == 0;
        }

        private void OnListRemove(BaseGameplayAbilityComponentConfigAsset element)
        {
            if (element is ConfAssetAbilityBaseInfo)
            {
                Debug.LogWarning("禁止删除组件【ConfAssetAbilityBaseInfo】！");
                EXEditorHelper.ShowNotification("禁止删除组件【ConfAssetAbilityBaseInfo】！");
                return;
            }

            // 禁止删除MCConfAssetAbilityLogic
            if (element is MCConfAssetAbilityLogic)
            {
                Debug.LogWarning("禁止删除组件【MCConfAssetAbilityLogic】！");
                EXEditorHelper.ShowNotification("禁止删除组件【MCConfAssetAbilityLogic】！");
                return;
            }

            //ComponentConfigs.Remove(element);
        }

        // 自动保存
        private void OnValueChange()
        {
            //UpdateData(ComponentConfigs);
            EditorUtility.SetDirty(this);
            AssetDatabase.SaveAssets();
        }
        #endregion
    }
}
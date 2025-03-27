using System;
using System.Collections.Generic;
using System.Linq;
using GAS.RuntimeDataHelper.Ability;
using GAS.RuntimeDataHelper.GameplayEffect.EffectComponentConfigAsset;
using GAS.RuntimeDataHelper.Helper;
using GAS.RuntimeWithECS.GameplayEffect;
using Sirenix.OdinInspector;
using UnityEngine;

namespace GAS.RuntimeDataHelper.GameplayEffect
{
    [CreateAssetMenu(fileName = "EffectConfigAsset", menuName = "EX-GAS/Effect", order = 0)]
    public class GameplayEffectConfigAsset : ScriptableObject
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
        [ValidateInput(nameof(ValidateList), ContinuousValidationCheck = true)]
        [LabelText("能力组件配置")]
        public List<BaseGameplayEffectComponentConfigAsset> ComponentConfigs =
            new();


        public GameplayEffectConfig GetConfig()
        {
            var configs = new List<GameplayEffectComponentConfig>();
            foreach (var config in ComponentConfigs) configs.Add(config.GetConfig());
            return new GameplayEffectConfig(configs.ToArray());
        }

        #region 编辑器工具

        private IEnumerable<Type> GetFilteredTypes()
        {
            var types = EXEditorHelper.GetCachedEffectComponentSubTypes();
            // 排除掉已有的类型
            var existingTypes = new HashSet<Type>(ComponentConfigs.Select(item => item.GetType()));
            return types.Where(type => !existingTypes.Contains(type));
        }

        [OnInspectorInit]
        private void InitializeList()
        {
            // 确保列表初始化时必须有一个ConfBasicInfo
            if (!ComponentConfigs.Any(item => item is ConfAssetEffectBasicInfo))
                ComponentConfigs.Add(new ConfAssetEffectBasicInfo());
        }

        private bool ValidateList(List<BaseGameplayEffectComponentConfigAsset> list, ref string errorMsg)
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

            if (!list.Any(item => item is ConfAssetEffectBasicInfo))
                messages.Add("列表必须包含一个ConfAssetBasicInfo元素！");

            errorMsg = messages.Count > 0 ? string.Join("\n", messages) : null;
            return messages.Count == 0;
        }

        private void OnListRemove(BaseGameplayEffectComponentConfigAsset element)
        {
            if (element is ConfAssetEffectBasicInfo)
            {
                Debug.LogWarning("禁止删除组件【ConfAssetBasicInfo】！");
                EXEditorHelper.ShowNotification("禁止删除组件【ConfAssetBasicInfo】！");
                return;
            }

            ComponentConfigs.Remove(element);
        }

        #endregion
    }
}
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using GAS.RuntimeDataHelper.Helper;
using GAS.RuntimeWithECS.Ability.Component.Static;
using GAS.RuntimeWithECS.Ability.ComponentConfig;
using Sirenix.OdinInspector;
using UnityEngine;

namespace GAS.RuntimeDataHelper.Ability.AbilityComponentConfigAsset
{
    [Serializable]
    public class ConfAssetAbilityBaseInfo : BaseGameplayAbilityComponentConfigAsset
    {
        public override GameplayAbilityComponentConfig GetConfig()
        {
            return new ConfAbilityBaseInfo
            {
                Code = Code,
                Level = Level
            };
        }

        #region 基础信息

        [TabGroup("Base", "基础信息", SdfIconType.TagsFill, TextColor = "#45B1FF", Order = 1)]
        [LabelWidth(80)]
        [LabelText("能力名")]
        [ValidateInput(nameof(ValidateAbilityName), ContinuousValidationCheck = true)]
        [Tooltip("能力名称将用于生成唯一标识码")]
        public string name;

        [TabGroup("Base","基础信息")]
        [LabelWidth(80)]
        [LabelText("能力代码")]
        [ShowInInspector]
        [DisplayAsString]
        [PropertyOrder(2)]
        [Tooltip("根据能力名自动生成的哈希码")]
        public int Code => string.IsNullOrEmpty(name) ? 0 : name.GetHashCode();

        [TabGroup("Base","基础信息")] [LabelText("能力描述")] [TextArea(3, 5)] [PropertyOrder(3)]
        public string description;

        [TabGroup("Base","基础信息")] [LabelText("初始等级")] [MinValue(0)] [PropertyOrder(4)]
        public int Level;

        #endregion

        #region 验证逻辑

        private bool ValidateAbilityName(string abilityName, ref string errorMsg)
        {
            var messages = new List<string>();

            if (string.IsNullOrEmpty(abilityName))
                messages.Add("能力名不可为空！");
            else if (!Regex.IsMatch(abilityName, @"^[a-zA-Z0-9_]+$"))
                messages.Add("只能包含字母/数字/下划线！");
            if (!ValidateUniqueName(abilityName))
                messages.Add("该名称已被其他配置使用！");

            errorMsg = messages.Count > 0 ? string.Join("\n", messages) : null;
            return messages.Count == 0;
        }

        private bool ValidateUniqueName(string abilityName)
        {
            foreach (var configAsset in EXEditorHelper.FindAll<AbilityConfigAsset>())
            foreach (var componentConfig in configAsset.ComponentConfigs)
                if (componentConfig != this &&
                    componentConfig is ConfAssetAbilityBaseInfo baseInfo &&
                    baseInfo.name == abilityName)
                    return false;
            return true;
        }

        #endregion
    }
}
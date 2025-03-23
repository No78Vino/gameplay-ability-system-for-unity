using System;
using System.Collections.Generic;
using GAS.RuntimeDataHelper.Ability.AbilityParam;
using GAS.RuntimeDataHelper.Helper;
using GAS.RuntimeWithECS.Ability.Component.Static;
using GAS.RuntimeWithECS.Ability.ComponentConfig;
using Sirenix.OdinInspector;

namespace GAS.RuntimeDataHelper.Ability.AbilityComponentConfigAsset
{
    [Serializable]
    public class MCConfAssetAbilityLogic : BaseGameplayAbilityComponentConfigAsset
    {
        [ValueDropdown("@EXEditorHelper.AbilityLogicChoices", IsUniqueList = true, HideChildProperties = true)]
        [TabGroup("AbilityLogic", "能力执行逻辑", SdfIconType.Activity, TextColor = "#D6626E")]
        [OnValueChanged(nameof(OnAbilityLogicTypeChanged))]
        [LabelText("能力逻辑")]
        public string AbilityLogicType;

        [TabGroup("AbilityLogic", "能力执行逻辑")] [LabelText("能力参数")] [TypeFilter(nameof(GetAbilityParamConfigSubTypes))]
        public AbilityParamConfigBase abilityParamConfig =
            Activator.CreateInstance(typeof(AbilityParamConfigNone)) as AbilityParamConfigBase;

        public override GameplayAbilityComponentConfig GetConfig()
        {
            return new MCConfAbilityLogic
            {
                abilityParam = abilityParamConfig.GetConfig()
            };
        }

        private IEnumerable<Type> GetAbilityParamConfigSubTypes()
        {
            return EXEditorHelper.GetCachedAbilityParamConfigTypes();
        }

        private void OnAbilityLogicTypeChanged()
        {
            var typeMap = EXEditorHelper.GetCachedAbilityLogicToAbilityParamConfigTypeMap();
            if (typeMap.TryGetValue(AbilityLogicType, out var value))
                abilityParamConfig = Activator.CreateInstance(value) as AbilityParamConfigBase;
            else
                EXEditorHelper.ShowNotification($"未找到对应的能力参数配置，请检查类【{AbilityLogicType}】是否继承自AbilityParamConfigBase<T>");
        }
    }
}
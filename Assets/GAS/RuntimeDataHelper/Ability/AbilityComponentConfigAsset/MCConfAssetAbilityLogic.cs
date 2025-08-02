using System;
using System.Collections.Generic;
using GAS.RuntimeDataHelper.Ability.AbilityParam;
using GAS.RuntimeDataHelper.Helper;
using GAS.RuntimeWithECS.CommonAbilityLogic;
using GAS.RuntimeWithECS.Static;
using GAS.RuntimeWithECS.ComponentConfig;
using Sirenix.OdinInspector;
using UnityEngine;

namespace GAS.RuntimeDataHelper.Ability.AbilityComponentConfigAsset
{
    [Serializable]
    public class MCConfAssetAbilityLogic : BaseGameplayAbilityComponentConfigAsset
    {
        [ValueDropdown("@EXEditorHelper.AbilityLogicChoices", IsUniqueList = true, HideChildProperties = true)]
        [TabGroup("AbilityLogic", "能力执行逻辑", SdfIconType.Activity, TextColor = "#D6626E")]
        [OnValueChanged(nameof(OnValueChanged))]
        [LabelText("能力逻辑")]
        public string AbilityLogicType;

        [SerializeField] [HideInInspector] private JsonConfigData _jsonAbilityParamConfig;

        [TabGroup("AbilityLogic", "能力执行逻辑")]
        [TypeFilter(nameof(GetAbilityParamConfigType))]
        [LabelText("能力参数")]
        [ShowInInspector]
        public AbilityParamConfigBase abilityParamConfig;

        public override GameplayAbilityComponentConfig GetConfig()
        {
            var realConfig = JsonProxyHelper.Deserialize<AbilityParamConfigBase>(_jsonAbilityParamConfig);
            return new MCConfAbilityLogic
            {
                AbilityLogicType = AbilityLogicType,
                abilityParam = realConfig.GetConfig()
            };
        }

        protected override void OnValueChanged()
        {
            abilityParamConfig ??= new AbilityParamConfigNone();
            _jsonAbilityParamConfig.TypeFullName = abilityParamConfig.GetType().FullName;
            _jsonAbilityParamConfig.Data = JsonProxyHelper.Serialize(abilityParamConfig);


            var typeMap = EXEditorHelper.GetCachedAbilityLogicToAbilityParamConfigTypeMap();
            if (typeMap.TryGetValue(AbilityLogicType, out var value))
            {
                if (abilityParamConfig.GetType() != value)
                    abilityParamConfig = Activator.CreateInstance(value) as AbilityParamConfigBase;
            }
            else
            {
                EXEditorHelper.ShowNotification(
                    $"未找到对应的能力参数配置，请检查类【{AbilityLogicType}】是否继承自AbilityParamConfigBase<T>");
            }

            abilityParamConfig?.SetConfAssetAbilityLogic(this);
            base.OnValueChanged();
        }

        private IEnumerable<Type> GetAbilityParamConfigType()
        {
            return null;
            // var typeMap = EXEditorHelper.GetCachedAbilityLogicToAbilityParamConfigTypeMap();
            // if (typeMap.TryGetValue(AbilityLogicType, out var value))
            //     return new[] { value };
            // return new[] { typeof(AbilityParamConfigNone) };
        }

        [OnInspectorInit]
        private void InitializeList()
        {
            if (string.IsNullOrEmpty(AbilityLogicType)) AbilityLogicType = typeof(ALDebugLog).FullName;
            if (!string.IsNullOrEmpty(_jsonAbilityParamConfig.TypeFullName))
                abilityParamConfig = JsonProxyHelper.Deserialize<AbilityParamConfigBase>(_jsonAbilityParamConfig);
            OnValueChanged();
        }

        public void TriggerOnValueChanged()
        {
            OnValueChanged();
        }
    }
}
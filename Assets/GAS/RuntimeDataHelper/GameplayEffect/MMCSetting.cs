using System;
using System.Collections.Generic;
using GAS.Runtime;
using GAS.RuntimeDataHelper.GameplayEffect.MmcParam;
using GAS.RuntimeDataHelper.Helper;
using GAS.RuntimeWithECS.Modifier;
using GAS.RuntimeWithECS.Modifier.CommonUsage;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Serialization;

namespace GAS.RuntimeDataHelper.GameplayEffect
{
    [Serializable]
    public class MMCSettingConfig
    {
        [FormerlySerializedAs("TypeCode")]
        [ValueDropdown("@EditGameplayEffectHelper.MmcChoices", IsUniqueList = true, HideChildProperties = true)]
        [TabGroup("MMC", "MMC", SdfIconType.Activity, TextColor = "#D6626E")]
        [OnValueChanged(nameof(OnValueChanged))]
        [LabelText("MMC类型")]
        public string MmcType;

        [SerializeField] [HideInInspector] private JsonConfigData _jsonMmcParamConfig;

        [TabGroup("MMC", "MMC")]
        [TypeFilter(nameof(GetMmcParamConfigType))]
        [LabelText("MMC参数")]
        [ShowInInspector]
        public MmcParamConfigBase mmcParamConfig;
        
        
        public ModMagnitudeCalculationBase CreateMmc()
        {
            var typeMap = EditGameplayEffectHelper.GetCachedMmcToMmcParamConfigTypeMap();
            MmcParamConfigBase config = null;
            if (typeMap.TryGetValue(MmcType, out var value))
                config = Activator.CreateInstance(value) as MmcParamConfigBase;
            
            return MmcHelper.TryCreateMmc(MmcType,config);
        }
        
        protected void OnValueChanged()
        {
            mmcParamConfig ??= new MmcParamConfigNone();
            _jsonMmcParamConfig.TypeFullName = mmcParamConfig.GetType().FullName;
            _jsonMmcParamConfig.Data = JsonProxyHelper.Serialize(mmcParamConfig);


            var typeMap = EditGameplayEffectHelper.GetCachedMmcToMmcParamConfigTypeMap();
            if (typeMap.TryGetValue(MmcType, out var value))
            {
                if (mmcParamConfig.GetType() != value)
                    mmcParamConfig = Activator.CreateInstance(value) as MmcParamConfigBase;
            }
            else
            {
                EXEditorHelper.ShowNotification(
                    $"未找到对应的MMC参数配置，请检查类【{MmcType}】是否继承自ModMagnitudeCalculationBase<T>");
            }

            mmcParamConfig?.SetConfAssetMmc(this);
        }

        private IEnumerable<Type> GetMmcParamConfigType() => null;

        [OnInspectorInit]
        private void InitializeList()
        {
            if (string.IsNullOrEmpty(MmcType)) MmcType = typeof(MMCNone).FullName;
            if (!string.IsNullOrEmpty(_jsonMmcParamConfig.TypeFullName))
                mmcParamConfig = JsonProxyHelper.Deserialize<MmcParamConfigBase>(_jsonMmcParamConfig);
            OnValueChanged();
        }

        public void TriggerOnValueChanged()
        {
            OnValueChanged();
        }
    }
}
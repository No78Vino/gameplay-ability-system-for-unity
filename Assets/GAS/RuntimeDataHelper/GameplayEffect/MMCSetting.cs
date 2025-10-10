using System;
using System.Collections.Generic;
using GAS.RuntimeDataHelper.GameplayEffect.MmcParam;
using GAS.Editor;
using GAS.Runtime;
using GAS.RuntimeWithECS.Modifier;
using GAS.RuntimeWithECS;
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
            var mmcParaType = MmcHelper.GetMmcParamTypeByMmcType(MmcType);
            if (mmcParaType != null)
            {
                MmcParamConfigBase config = Activator.CreateInstance(mmcParaType) as MmcParamConfigBase;
            }

            return null;
        }
        
        protected void OnValueChanged()
        {
            mmcParamConfig ??= new MmcParamConfigNone();
            _jsonMmcParamConfig.TypeFullName = mmcParamConfig.GetType().FullName;
            _jsonMmcParamConfig.Data = JsonProxyHelper.Serialize(mmcParamConfig);


            var mmcParaType = MmcHelper.GetMmcParamTypeByMmcType(MmcType);
            if (mmcParaType != null)
            {
                if (mmcParamConfig.GetType() != mmcParaType)
                    mmcParamConfig = Activator.CreateInstance(mmcParaType) as MmcParamConfigBase;
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
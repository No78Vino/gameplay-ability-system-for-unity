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

namespace GAS.Editor
{
    [Serializable]
    public class InstantCueSettingConfig:ICueSettingConfig
    {
        [ValueDropdown("@EditCueHelper.InstantCueChoices", IsUniqueList = true, HideChildProperties = true)]
        [TabGroup("CueLogic", "即时Cue", SdfIconType.FileMusic, TextColor = "#D6626E")]
        [OnValueChanged(nameof(OnValueChanged))]
        [LabelText("Cue类型")]
        public string CueType;

        [SerializeField] [HideInInspector] private JsonConfigData _jsonCueParamConfig;

        [TabGroup("CueLogic", "即时Cue")]
        [TypeFilter(nameof(GetCueParamConfigType))]
        [LabelText("Cue参数")]
        [ShowInInspector]
        public CueParamConfigBase cueParamConfig;
        
        
        public ModMagnitudeCalculationBase CreateCue()
        {
            var typeMap = EditGameplayEffectHelper.GetCachedMmcToMmcParamConfigTypeMap();
            MmcParamConfigBase config = null;
            if (typeMap.TryGetValue(CueType, out var value))
                config = Activator.CreateInstance(value) as MmcParamConfigBase;
            
            return MmcHelper.TryCreateMmc(CueType,config);
        }
        
        protected void OnValueChanged()
        {
            cueParamConfig ??= new CueParamConfigNone();
            _jsonCueParamConfig.TypeFullName = cueParamConfig.GetType().FullName;
            _jsonCueParamConfig.Data = JsonProxyHelper.Serialize(cueParamConfig);


            var typeMap = EditCueHelper.GetCachedMmcToMmcParamConfigTypeMap();
            if (typeMap.TryGetValue(CueType, out var value))
            {
                if (cueParamConfig.GetType() != value)
                    cueParamConfig = Activator.CreateInstance(value) as MmcParamConfigBase;
            }
            else
            {
                EXEditorHelper.ShowNotification(
                    $"未找到对应的MMC参数配置，请检查类【{CueType}】是否继承自ModMagnitudeCalculationBase<T>");
            }

            cueParamConfig?.SetConfAssetCue(this);
        }

        private IEnumerable<Type> GetCueParamConfigType() => null;

        [OnInspectorInit]
        private void InitializeList()
        {
            if (string.IsNullOrEmpty(CueType)) CueType = typeof(MMCNone).FullName;
            if (!string.IsNullOrEmpty(_jsonCueParamConfig.TypeFullName))
                cueParamConfig = JsonProxyHelper.Deserialize<MmcParamConfigBase>(_jsonCueParamConfig);
            OnValueChanged();
        }

        public void TriggerOnValueChanged()
        {
            OnValueChanged();
        }
    }
}
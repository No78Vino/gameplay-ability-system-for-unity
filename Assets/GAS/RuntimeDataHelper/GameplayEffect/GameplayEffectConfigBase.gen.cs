///////////////////////////////////
//// This is a generated file. ////
////     Do not modify it.     ////
///////////////////////////////////

using System;
using System.Collections.Generic;
using System.Linq;
using GAS.RuntimeDataHelper.GameplayEffect;
using GAS.RuntimeDataHelper.GameplayEffect.EffectComponentConfigAsset;
using Sirenix.OdinInspector;
using UnityEngine.Serialization;

namespace GAS.Runtime
{
  [Serializable]
  public class GameplayEffectConfigBase
  {
    [TabGroup("EffectConfig","效果组件类型控制",SdfIconType.TagsFill)]
    [ValueDropdown("@EditGameplayEffectHelper.EffectComponentTypeChoices", IsUniqueList = true, HideChildProperties = true)]
    public List<string> configTypes = new();

    [TabGroup("EffectConfig","配置详情",SdfIconType.Activity)]
    [HideLabel]
    [ShowIf(nameof(HasConfAssetApplicationCondition))]
    [OnValueChanged(nameof(OnConfigValueChanged))]
    public ConfAssetApplicationCondition ConfAssetApplicationCondition;

    [TabGroup("EffectConfig","配置详情",SdfIconType.Activity)]
    [HideLabel]
    [ShowIf(nameof(HasConfAssetCueOnAdd))]
    [OnValueChanged(nameof(OnConfigValueChanged))]
    public ConfAssetCueOnAdd ConfAssetCueOnAdd;

    [TabGroup("EffectConfig","配置详情",SdfIconType.Activity)]
    [HideLabel]
    [ShowIf(nameof(HasConfAssetCueOnExecution))]
    [OnValueChanged(nameof(OnConfigValueChanged))]
    public ConfAssetCueOnExecution ConfAssetCueOnExecution;

    [TabGroup("EffectConfig","配置详情",SdfIconType.Activity)]
    [HideLabel]
    [ShowIf(nameof(HasConfAssetEffectApplicationRequiredTags))]
    [OnValueChanged(nameof(OnConfigValueChanged))]
    public ConfAssetEffectApplicationRequiredTags ConfAssetEffectApplicationRequiredTags;

    [TabGroup("EffectConfig","配置详情",SdfIconType.Activity)]
    [HideLabel]
    [ShowIf(nameof(HasConfAssetEffectAssetTags))]
    [OnValueChanged(nameof(OnConfigValueChanged))]
    public ConfAssetEffectAssetTags ConfAssetEffectAssetTags;

    [TabGroup("EffectConfig","配置详情",SdfIconType.Activity)]
    [HideLabel]
    [ShowIf(nameof(HasConfAssetEffectBasicInfo))]
    [OnValueChanged(nameof(OnConfigValueChanged))]
    [PropertyOrder(-1)]
    public ConfAssetEffectBasicInfo ConfAssetEffectBasicInfo;

    [TabGroup("EffectConfig","配置详情",SdfIconType.Activity)]
    [HideLabel]
    [ShowIf(nameof(HasConfAssetEffectDuration))]
    [OnValueChanged(nameof(OnConfigValueChanged))]
    public ConfAssetEffectDuration ConfAssetEffectDuration;

    [TabGroup("EffectConfig","配置详情",SdfIconType.Activity)]
    [HideLabel]
    [ShowIf(nameof(HasConfAssetEffectGrantedTags))]
    [OnValueChanged(nameof(OnConfigValueChanged))]
    public ConfAssetEffectGrantedTags ConfAssetEffectGrantedTags;

    [TabGroup("EffectConfig","配置详情",SdfIconType.Activity)]
    [HideLabel]
    [ShowIf(nameof(HasConfAssetEffectImmunityTags))]
    [OnValueChanged(nameof(OnConfigValueChanged))]
    public ConfAssetEffectImmunityTags ConfAssetEffectImmunityTags;

    [FormerlySerializedAs("ConfAssetModifiers")]
    [TabGroup("EffectConfig","配置详情",SdfIconType.Activity)]
    [HideLabel]
    [ShowIf(nameof(HasConfAssetModifiers))]
    [OnValueChanged(nameof(OnConfigValueChanged))]
    public MCConfAssetModifiers mcConfAssetModifiers;

    [TabGroup("EffectConfig","配置详情",SdfIconType.Activity)]
    [HideLabel]
    [ShowIf(nameof(HasConfAssetOngoingRequiredTags))]
    [OnValueChanged(nameof(OnConfigValueChanged))]
    public ConfAssetOngoingRequiredTags ConfAssetOngoingRequiredTags;

    [TabGroup("EffectConfig","配置详情",SdfIconType.Activity)]
    [HideLabel]
    [ShowIf(nameof(HasConfAssetPeriod))]
    [OnValueChanged(nameof(OnConfigValueChanged))]
    public ConfAssetPeriod ConfAssetPeriod;

    [TabGroup("EffectConfig","配置详情",SdfIconType.Activity)]
    [HideLabel]
    [ShowIf(nameof(HasConfAssetRemoveEffectWithTags))]
    [OnValueChanged(nameof(OnConfigValueChanged))]
    public ConfAssetRemoveEffectWithTags ConfAssetRemoveEffectWithTags;

    [TabGroup("EffectConfig","配置详情",SdfIconType.Activity)]
    [HideLabel]
    [ShowIf(nameof(HasConfAssetStacking))]
    [OnValueChanged(nameof(OnConfigValueChanged))]
    public ConfAssetStacking ConfAssetStacking;

    protected bool HasConfAssetApplicationCondition => 
        configTypes.Any( x => x == typeof(ConfAssetApplicationCondition).FullName);
    protected bool HasConfAssetCueOnAdd => 
        configTypes.Any( x => x == typeof(ConfAssetCueOnAdd).FullName);
    protected bool HasConfAssetCueOnExecution => 
        configTypes.Any( x => x == typeof(ConfAssetCueOnExecution).FullName);
    protected bool HasConfAssetEffectApplicationRequiredTags => 
        configTypes.Any( x => x == typeof(ConfAssetEffectApplicationRequiredTags).FullName);
    protected bool HasConfAssetEffectAssetTags => 
        configTypes.Any( x => x == typeof(ConfAssetEffectAssetTags).FullName);
    protected bool HasConfAssetEffectBasicInfo => 
        configTypes.Any( x => x == typeof(ConfAssetEffectBasicInfo).FullName);
    protected bool HasConfAssetEffectDuration => 
        configTypes.Any( x => x == typeof(ConfAssetEffectDuration).FullName);
    protected bool HasConfAssetEffectGrantedTags => 
        configTypes.Any( x => x == typeof(ConfAssetEffectGrantedTags).FullName);
    protected bool HasConfAssetEffectImmunityTags => 
        configTypes.Any( x => x == typeof(ConfAssetEffectImmunityTags).FullName);
    protected bool HasConfAssetModifiers => 
        configTypes.Any( x => x == typeof(MCConfAssetModifiers).FullName);
    protected bool HasConfAssetOngoingRequiredTags => 
        configTypes.Any( x => x == typeof(ConfAssetOngoingRequiredTags).FullName);
    protected bool HasConfAssetPeriod => 
        configTypes.Any( x => x == typeof(ConfAssetPeriod).FullName);
    protected bool HasConfAssetRemoveEffectWithTags => 
        configTypes.Any( x => x == typeof(ConfAssetRemoveEffectWithTags).FullName);
    protected bool HasConfAssetStacking => 
        configTypes.Any( x => x == typeof(ConfAssetStacking).FullName);

    protected void OnConfigValueChanged()
    {
        CheckComponentConfigOwnAsset();
        //EditorUtility.SetDirty(this);
        //AssetDatabase.SaveAssets();
    }

    public BaseGameplayEffectComponentConfigAsset GetConfigAsset(string type)
    {
            if(type==typeof(ConfAssetApplicationCondition).FullName)
                return HasConfAssetApplicationCondition?ConfAssetApplicationCondition:null;
            if(type==typeof(ConfAssetCueOnAdd).FullName)
                return HasConfAssetCueOnAdd?ConfAssetCueOnAdd:null;
            if(type==typeof(ConfAssetCueOnExecution).FullName)
                return HasConfAssetCueOnExecution?ConfAssetCueOnExecution:null;
            if(type==typeof(ConfAssetEffectApplicationRequiredTags).FullName)
                return HasConfAssetEffectApplicationRequiredTags?ConfAssetEffectApplicationRequiredTags:null;
            if(type==typeof(ConfAssetEffectAssetTags).FullName)
                return HasConfAssetEffectAssetTags?ConfAssetEffectAssetTags:null;
            if(type==typeof(ConfAssetEffectBasicInfo).FullName)
                return HasConfAssetEffectBasicInfo?ConfAssetEffectBasicInfo:null;
            if(type==typeof(ConfAssetEffectDuration).FullName)
                return HasConfAssetEffectDuration?ConfAssetEffectDuration:null;
            if(type==typeof(ConfAssetEffectGrantedTags).FullName)
                return HasConfAssetEffectGrantedTags?ConfAssetEffectGrantedTags:null;
            if(type==typeof(ConfAssetEffectImmunityTags).FullName)
                return HasConfAssetEffectImmunityTags?ConfAssetEffectImmunityTags:null;
            if(type==typeof(MCConfAssetModifiers).FullName)
                return HasConfAssetModifiers?mcConfAssetModifiers:null;
            if(type==typeof(ConfAssetOngoingRequiredTags).FullName)
                return HasConfAssetOngoingRequiredTags?ConfAssetOngoingRequiredTags:null;
            if(type==typeof(ConfAssetPeriod).FullName)
                return HasConfAssetPeriod?ConfAssetPeriod:null;
            if(type==typeof(ConfAssetRemoveEffectWithTags).FullName)
                return HasConfAssetRemoveEffectWithTags?ConfAssetRemoveEffectWithTags:null;
            if(type==typeof(ConfAssetStacking).FullName)
                return HasConfAssetStacking?ConfAssetStacking:null;
            return null;
    }
    protected bool ValidateList(List<string> _, ref string errorMsg)
    {
        return false;
    }
    [OnInspectorInit]
    private void InitializeList()
    {
        CheckComponentConfigOwnAsset();
    }

    protected void CheckComponentConfigOwnAsset()
    {
        ConfAssetApplicationCondition?.SetOwnAsset(this);
        ConfAssetCueOnAdd?.SetOwnAsset(this);
        ConfAssetCueOnExecution?.SetOwnAsset(this);
        ConfAssetEffectApplicationRequiredTags?.SetOwnAsset(this);
        ConfAssetEffectAssetTags?.SetOwnAsset(this);
        ConfAssetEffectBasicInfo?.SetOwnAsset(this);
        ConfAssetEffectDuration?.SetOwnAsset(this);
        ConfAssetEffectGrantedTags?.SetOwnAsset(this);
        ConfAssetEffectImmunityTags?.SetOwnAsset(this);
        mcConfAssetModifiers?.SetOwnAsset(this);
        ConfAssetOngoingRequiredTags?.SetOwnAsset(this);
        ConfAssetPeriod?.SetOwnAsset(this);
        ConfAssetRemoveEffectWithTags?.SetOwnAsset(this);
        ConfAssetStacking?.SetOwnAsset(this);
    }
  }
}

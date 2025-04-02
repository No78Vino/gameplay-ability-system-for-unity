///////////////////////////////////
//// This is a generated file. ////
////     Do not modify it.     ////
///////////////////////////////////

using System.Collections.Generic;
using System.Linq;
using GAS.RuntimeDataHelper.Ability.AbilityComponentConfigAsset;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;

namespace GAS.RuntimeDataHelper.Ability
{
  public class GEN_AbilityConfigSO:ScriptableObject
  {
    [TabGroup("AbilityConfig","能力组件类型控制",SdfIconType.TagsFill)]
    [ValueDropdown("@EXEditorHelper.AbilityComponentTypeChoices", IsUniqueList = true, HideChildProperties = true)]
    public List<string> configTypes = new();

    [TabGroup("AbilityConfig","组件配置详情",SdfIconType.Activity)]
    [LabelText("ConfAssetAbilityActivationBlockedTags")]
    [ShowIf(nameof(HasConfAssetAbilityActivationBlockedTags))]
    [OnValueChanged(nameof(OnConfigValueChanged))]
    public ConfAssetAbilityActivationBlockedTags ConfAssetAbilityActivationBlockedTags;

    [TabGroup("AbilityConfig","组件配置详情",SdfIconType.Activity)]
    [LabelText("ConfAssetAbilityActivationOwnedTags")]
    [ShowIf(nameof(HasConfAssetAbilityActivationOwnedTags))]
    [OnValueChanged(nameof(OnConfigValueChanged))]
    public ConfAssetAbilityActivationOwnedTags ConfAssetAbilityActivationOwnedTags;

    [TabGroup("AbilityConfig","组件配置详情",SdfIconType.Activity)]
    [LabelText("ConfAssetAbilityActivationRequiredTags")]
    [ShowIf(nameof(HasConfAssetAbilityActivationRequiredTags))]
    [OnValueChanged(nameof(OnConfigValueChanged))]
    public ConfAssetAbilityActivationRequiredTags ConfAssetAbilityActivationRequiredTags;

    [TabGroup("AbilityConfig","组件配置详情",SdfIconType.Activity)]
    [LabelText("ConfAssetAbilityAssetTags")]
    [ShowIf(nameof(HasConfAssetAbilityAssetTags))]
    [OnValueChanged(nameof(OnConfigValueChanged))]
    public ConfAssetAbilityAssetTags ConfAssetAbilityAssetTags;

    [TabGroup("AbilityConfig","组件配置详情",SdfIconType.Activity)]
    [LabelText("ConfAssetAbilityBaseInfo")]
    [ShowIf(nameof(HasConfAssetAbilityBaseInfo))]
    [OnValueChanged(nameof(OnConfigValueChanged))]
    [PropertyOrder(-1)]
    public ConfAssetAbilityBaseInfo ConfAssetAbilityBaseInfo;

    [TabGroup("AbilityConfig","组件配置详情",SdfIconType.Activity)]
    [LabelText("ConfAssetAbilityCooldown")]
    [ShowIf(nameof(HasConfAssetAbilityCooldown))]
    [OnValueChanged(nameof(OnConfigValueChanged))]
    public ConfAssetAbilityCooldown ConfAssetAbilityCooldown;

    [TabGroup("AbilityConfig","组件配置详情",SdfIconType.Activity)]
    [LabelText("ConfAssetAbilityCost")]
    [ShowIf(nameof(HasConfAssetAbilityCost))]
    [OnValueChanged(nameof(OnConfigValueChanged))]
    public ConfAssetAbilityCost ConfAssetAbilityCost;

    [TabGroup("AbilityConfig","组件配置详情",SdfIconType.Activity)]
    [LabelText("ConfAssetBlockAbilityTags")]
    [ShowIf(nameof(HasConfAssetBlockAbilityTags))]
    [OnValueChanged(nameof(OnConfigValueChanged))]
    public ConfAssetBlockAbilityTags ConfAssetBlockAbilityTags;

    [TabGroup("AbilityConfig","组件配置详情",SdfIconType.Activity)]
    [LabelText("ConfAssetCancelAbilityTags")]
    [ShowIf(nameof(HasConfAssetCancelAbilityTags))]
    [OnValueChanged(nameof(OnConfigValueChanged))]
    public ConfAssetCancelAbilityTags ConfAssetCancelAbilityTags;

    [TabGroup("AbilityConfig","组件配置详情",SdfIconType.Activity)]
    [LabelText("MCConfAssetAbilityLogic")]
    [ShowIf(nameof(HasMCConfAssetAbilityLogic))]
    [OnValueChanged(nameof(OnConfigValueChanged))]
    [PropertyOrder(-1)]
    public MCConfAssetAbilityLogic MCConfAssetAbilityLogic;

    protected bool HasConfAssetAbilityActivationBlockedTags => 
        configTypes.Any( x => x == typeof(ConfAssetAbilityActivationBlockedTags).FullName);
    protected bool HasConfAssetAbilityActivationOwnedTags => 
        configTypes.Any( x => x == typeof(ConfAssetAbilityActivationOwnedTags).FullName);
    protected bool HasConfAssetAbilityActivationRequiredTags => 
        configTypes.Any( x => x == typeof(ConfAssetAbilityActivationRequiredTags).FullName);
    protected bool HasConfAssetAbilityAssetTags => 
        configTypes.Any( x => x == typeof(ConfAssetAbilityAssetTags).FullName);
    protected bool HasConfAssetAbilityBaseInfo => 
        configTypes.Any( x => x == typeof(ConfAssetAbilityBaseInfo).FullName);
    protected bool HasConfAssetAbilityCooldown => 
        configTypes.Any( x => x == typeof(ConfAssetAbilityCooldown).FullName);
    protected bool HasConfAssetAbilityCost => 
        configTypes.Any( x => x == typeof(ConfAssetAbilityCost).FullName);
    protected bool HasConfAssetBlockAbilityTags => 
        configTypes.Any( x => x == typeof(ConfAssetBlockAbilityTags).FullName);
    protected bool HasConfAssetCancelAbilityTags => 
        configTypes.Any( x => x == typeof(ConfAssetCancelAbilityTags).FullName);
    protected bool HasMCConfAssetAbilityLogic => 
        configTypes.Any( x => x == typeof(MCConfAssetAbilityLogic).FullName);

    protected void OnConfigValueChanged()
    {
        CheckComponentConfigOwnAsset();
        EditorUtility.SetDirty(this);
        //AssetDatabase.SaveAssets();
    }

    protected BaseGameplayAbilityComponentConfigAsset GetConfigAsset(string type)
    {
        return type switch
        {
            nameof(ConfAssetAbilityActivationBlockedTags) => HasConfAssetAbilityActivationBlockedTags?ConfAssetAbilityActivationBlockedTags:null,
            nameof(ConfAssetAbilityActivationOwnedTags) => HasConfAssetAbilityActivationOwnedTags?ConfAssetAbilityActivationOwnedTags:null,
            nameof(ConfAssetAbilityActivationRequiredTags) => HasConfAssetAbilityActivationRequiredTags?ConfAssetAbilityActivationRequiredTags:null,
            nameof(ConfAssetAbilityAssetTags) => HasConfAssetAbilityAssetTags?ConfAssetAbilityAssetTags:null,
            nameof(ConfAssetAbilityBaseInfo) => HasConfAssetAbilityBaseInfo?ConfAssetAbilityBaseInfo:null,
            nameof(ConfAssetAbilityCooldown) => HasConfAssetAbilityCooldown?ConfAssetAbilityCooldown:null,
            nameof(ConfAssetAbilityCost) => HasConfAssetAbilityCost?ConfAssetAbilityCost:null,
            nameof(ConfAssetBlockAbilityTags) => HasConfAssetBlockAbilityTags?ConfAssetBlockAbilityTags:null,
            nameof(ConfAssetCancelAbilityTags) => HasConfAssetCancelAbilityTags?ConfAssetCancelAbilityTags:null,
            nameof(MCConfAssetAbilityLogic) => HasMCConfAssetAbilityLogic?MCConfAssetAbilityLogic:null,
            _ => null
        };
    }
    protected virtual bool ValidateList(List<string> _, ref string errorMsg)
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
        ConfAssetAbilityActivationBlockedTags?.SetOwnAsset(this);
        ConfAssetAbilityActivationOwnedTags?.SetOwnAsset(this);
        ConfAssetAbilityActivationRequiredTags?.SetOwnAsset(this);
        ConfAssetAbilityAssetTags?.SetOwnAsset(this);
        ConfAssetAbilityBaseInfo?.SetOwnAsset(this);
        ConfAssetAbilityCooldown?.SetOwnAsset(this);
        ConfAssetAbilityCost?.SetOwnAsset(this);
        ConfAssetBlockAbilityTags?.SetOwnAsset(this);
        ConfAssetCancelAbilityTags?.SetOwnAsset(this);
        MCConfAssetAbilityLogic?.SetOwnAsset(this);
    }
  }
}

using System.Linq;
using GAS.General;
using Sirenix.OdinInspector;
using UnityEngine;

namespace GAS.Runtime
{
    [CreateAssetMenu(fileName = "GameplayEffect", menuName = "GAS/GameplayEffect")]
    public class GameplayEffectAsset : ScriptableObject, IGameplayEffectData
    {
        private const string GRP_BASE = "Base";
        private const string GRP_BASE_H = "Base/H";
        private const string GRP_BASE_H_LEFT = "Base/H/Left";
        private const string GRP_BASE_H_RIGHT = "Base/H/Right";

        private const string GRP_DATA = "Data";
        private const string GRP_DATA_H = "Data/H";
        private const string GRP_DATA_TAG = "Data/H/Tags";
        private const string GRP_DATA_MOD = "Data/H/Modifiers";
        private const string GRP_DATA_CUE = "Data/H/Cues";
        private const string GRP_DATA_H2 = "Data/H2";
        private const string GRP_DATA_STACK = "Data/H2/Stack";
        private const string GRP_DATA_GRANTED_ABILITIES = "Data/H2/GrantedAbilities";

        private const int WIDTH_LABEL = 70;

        private const string ERROR_NONE_CUE = "Cue CAN NOT be NONE!";
        private const string ERROR_DURATION = "Duration must be > 0.";
        private const string ERROR_PERIOD_GE_NONE = "Period GameplayEffect CAN NOT be NONE!";
        private const string ERROR_GRANTED_ABILITY_INVALID = "存在无效的Ability!";

        #region Base Info

        [TitleGroup(GRP_BASE)]
        [HorizontalGroup(GRP_BASE_H, Width = 1 - 0.618f)]
        [TabGroup(GRP_BASE_H_LEFT, "Summary", SdfIconType.InfoSquareFill, TextColor = "#0BFFC5")]
        [HideLabel]
        [MultiLineProperty(5)]
        public string Description;

        #endregion Base Info

        #region Policy

        [HorizontalGroup(GRP_BASE_H)]
        [TabGroup(GRP_BASE_H_RIGHT, "Policy", SdfIconType.AwardFill, TextColor = "#FF7F00")]
        [LabelText(GASTextDefine.LABLE_GE_POLICY, SdfIconType.Diagram3Fill)]
        [LabelWidth(WIDTH_LABEL)]
        [EnumToggleButtons]
        [PropertyOrder(1)]
        public EffectsDurationPolicy DurationPolicy = EffectsDurationPolicy.Instant;

        [TabGroup(GRP_BASE_H_RIGHT, "Policy")]
        [LabelWidth(WIDTH_LABEL)]
        [EnableIf("@DurationPolicy == EffectsDurationPolicy.Duration")]
        [Unit(Units.Second)]
        [ValidateInput("@DurationPolicy != EffectsDurationPolicy.Duration || Duration > 0", ERROR_DURATION)]
        [LabelText(GASTextDefine.LABLE_GE_DURATION, SdfIconType.HourglassSplit)]
        [PropertyOrder(2)]
        public float Duration;

        [TabGroup(GRP_BASE_H_RIGHT, "Policy")]
        [LabelText(GASTextDefine.LABLE_GE_INTERVAL, SdfIconType.AlarmFill)]
        [LabelWidth(WIDTH_LABEL)]
        [ShowIf("@DurationPolicy != EffectsDurationPolicy.Duration")]
        [EnableIf("IsDurationalPolicy")]
        [Unit(Units.Second)]
        [PropertyOrder(3)]
        public float Period;

        [TabGroup(GRP_BASE_H_RIGHT, "Policy")]
        [LabelText(GASTextDefine.LABLE_GE_INTERVAL, SdfIconType.AlarmFill)]
        [LabelWidth(WIDTH_LABEL)]
        [ShowIf("@DurationPolicy == EffectsDurationPolicy.Duration")]
        [InfoBox("Period < 0.01会出现误差", InfoMessageType.Warning,
            VisibleIf =
                "@DurationPolicy == EffectsDurationPolicy.Duration && PeriodForDurational > 0 && PeriodForDurational < 0.01f")]
        [EnableIf("IsDurationalPolicy")]
        [Unit(Units.Second)]
        [PropertyOrder(3)]
        [PropertyRange(0, "@Duration")]
        [ShowInInspector]
        // 这个Property是为了给"限时型"效果绘制一个范围滑动条
        public float PeriodForDurational
        {
            get => Period;
            set => Period = value;
        }

        [TabGroup(GRP_BASE_H_RIGHT, "Policy")]
        [LabelText(GASTextDefine.LABLE_GE_EXEC, SdfIconType.Magic)]
        [LabelWidth(WIDTH_LABEL)]
        [EnableIf("IsPeriodic")]
        [AssetSelector]
        [InfoBox(ERROR_PERIOD_GE_NONE, InfoMessageType.Error, VisibleIf = "IsPeriodGameplayEffectNone")]
        [InfoBox("必须为Instant类型", InfoMessageType.Error,
            VisibleIf =
                "@IsPeriodic() && (PeriodExecution != null && PeriodExecution.DurationPolicy != EffectsDurationPolicy.Instant)")]
        [PropertyOrder(4)]
        public GameplayEffectAsset PeriodExecution;

        #endregion Policy

        #region Stack

        [TitleGroup(GRP_DATA)]
        [HorizontalGroup(GRP_DATA_H2, order: 2, Width = 1 - 0.618f)]
        [TabGroup(GRP_DATA_STACK, "Stacking", SdfIconType.Stack, TextColor = "#9B4AE3", Order = 1)]
        [HideLabel]
        [EnableIf("IsDurationalPolicy")]
        [InfoBox("瞬时效果无法叠加", InfoMessageType.None, VisibleIf = "@IsInstantPolicy()")]
        public GameplayEffectStackingConfig Stacking;

#if UNITY_EDITOR
        [TabGroup(GRP_DATA_STACK, "Stacking")]
        [ShowIf("@IsDurationalPolicy() && Stacking.stackingType != StackingType.None")]
        [Button("使用资产名称作为堆叠识别码", ButtonSizes.Medium, Icon = SdfIconType.Hammer)]
        private void SetStackingCodeNameAsAssetName()
        {
            var stacking = Stacking;
            stacking.stackingCodeName = name;
            Stacking = stacking;
        }
#endif

        #endregion Stack

        #region Granted Abilities

        [TabGroup(GRP_DATA_GRANTED_ABILITIES, "Granted Abilities", SdfIconType.YinYang, TextColor = "#D6626E",
            Order = 2)]
        [EnableIf("IsDurationalPolicy")]
        [InfoBox("瞬时效果无法赋予能力", InfoMessageType.None, VisibleIf = "@IsInstantPolicy()")]
        [ListDrawerSettings(ShowFoldout = true, ShowItemCount = false)]
        [InfoBox(ERROR_GRANTED_ABILITY_INVALID, InfoMessageType.Error, VisibleIf = "IsGrantedAbilitiesInvalid")]
        public GrantedAbilityConfig[] GrantedAbilities;

        #endregion Granted Abilities

        #region Modifiers

        [HorizontalGroup(GRP_DATA_H, order: 1, Width = 0.618f * 0.618f)]
        [TabGroup(GRP_DATA_MOD, "Modifiers", SdfIconType.CalculatorFill, TextColor = "#FFE60B", Order = 2)]
        [ListDrawerSettings(ShowFoldout = true, ShowItemCount = false)]
        [InfoBox("依次执行多个修改器, 请注意执行顺序", InfoMessageType.Info, VisibleIf = "@$value != null && $value.Length > 1")]
        [InfoBox("瞬时效果不能修改非Stacking属性", InfoMessageType.Error, VisibleIf = "IsModifiersHasInvalid")]
        [LabelText(@"@IsInstantPolicy() ? ""仅在成功应用时执行"":""每次激活时都会执行""")]
        public GameplayEffectModifier[] Modifiers;

        bool IsModifiersHasInvalid()
        {
            if (IsInstantPolicy())
            {
                return Modifiers != null && Modifiers.Any(modifier =>
                {
                    var attributeBase = ReflectionHelper.GetAttribute(modifier.AttributeName);
                    if (attributeBase != null)
                    {
                        return attributeBase.CalculateMode != CalculateMode.Stacking;
                    }

                    return false;
                });
            }

            return false;
        }

        #endregion Modifiers

        #region Tags

        [HorizontalGroup(GRP_DATA_H, order: 1, Width = 1 - 0.618f)]
        [TabGroup(GRP_DATA_TAG, "Tags", SdfIconType.TagsFill, TextColor = "#45B1FF", Order = 1)]
        [ListDrawerSettings(ShowFoldout = true, ShowItemCount = false)]
        [ValueDropdown("@ValueDropdownHelper.GameplayTagChoices", IsUniqueList = true, HideChildProperties = true)]
        [LabelText(GASTextDefine.TITLE_GE_TAG_AssetTags)]
        [Tooltip(GASTextDefine.TIP_GE_TAG_AssetTags)]
        [ShowIf("IsDurationalPolicy")]
        public GameplayTag[] AssetTags;

        [Space()]
        [TabGroup(GRP_DATA_TAG, "Tags")]
        [ListDrawerSettings(ShowFoldout = true, ShowItemCount = false)]
        [ValueDropdown("@ValueDropdownHelper.GameplayTagChoices", IsUniqueList = true, HideChildProperties = true)]
        [LabelText(GASTextDefine.TITLE_GE_TAG_GrantedTags)]
        [Tooltip(GASTextDefine.TIP_GE_TAG_GrantedTags)]
        [ShowIf("IsDurationalPolicy")]
        public GameplayTag[] GrantedTags;

        [Space()]
        [TabGroup(GRP_DATA_TAG, "Tags")]
        [ListDrawerSettings(ShowFoldout = true, ShowItemCount = false)]
        [ValueDropdown("@ValueDropdownHelper.GameplayTagChoices", IsUniqueList = true, HideChildProperties = true)]
        [LabelText(GASTextDefine.TITLE_GE_TAG_ApplicationRequiredTags)]
        [Tooltip(GASTextDefine.TIP_GE_TAG_ApplicationRequiredTags)]
        public GameplayTag[] ApplicationRequiredTags;

        [Space()]
        [TabGroup(GRP_DATA_TAG, "Tags")]
        [ListDrawerSettings(ShowFoldout = true, ShowItemCount = false)]
        [ValueDropdown("@ValueDropdownHelper.GameplayTagChoices", IsUniqueList = true, HideChildProperties = true)]
        [LabelText(GASTextDefine.TITLE_GE_TAG_OngoingRequiredTags)]
        [Tooltip(GASTextDefine.TIP_GE_TAG_OngoingRequiredTags)]
        [ShowIf("IsDurationalPolicy")]
        public GameplayTag[] OngoingRequiredTags;

        [Space()]
        [TabGroup(GRP_DATA_TAG, "Tags")]
        [ListDrawerSettings(ShowFoldout = true, ShowItemCount = false)]
        [ValueDropdown("@ValueDropdownHelper.GameplayTagChoices", IsUniqueList = true, HideChildProperties = true)]
        [LabelText(GASTextDefine.TITLE_GE_TAG_RemoveGameplayEffectsWithTags)]
        [Tooltip(GASTextDefine.TIP_GE_TAG_RemoveGameplayEffectsWithTags)]
        public GameplayTag[] RemoveGameplayEffectsWithTags;

        [Space()]
        [TabGroup(GRP_DATA_TAG, "Tags")]
        [ListDrawerSettings(ShowFoldout = true, ShowItemCount = false)]
        [ValueDropdown("@ValueDropdownHelper.GameplayTagChoices", IsUniqueList = true, HideChildProperties = true)]
        [LabelText(GASTextDefine.TITLE_GE_TAG_ApplicationImmunityTags)]
        [Tooltip(GASTextDefine.TIP_GE_TAG_ApplicationImmunityTags)]
        public GameplayTag[] ApplicationImmunityTags;

        #endregion Tags

        #region Cues

        [TabGroup(GRP_DATA_CUE, "Cues", SdfIconType.Stars, TextColor = "#00FFFF", Order = 3)]
        [ListDrawerSettings(ShowFoldout = true, ShowItemCount = false)]
        [ShowIf("IsInstantPolicy")]
        [InfoBox(ERROR_NONE_CUE, InfoMessageType.Error, VisibleIf = "IsCueExecuteNone")]
        [AssetSelector]
        [LabelText(GASTextDefine.TITLE_GE_CUE_CueOnExecute)]
        public GameplayCueInstant[] CueOnExecute;

        [Space()]
        [TabGroup(GRP_DATA_CUE, "Cues")]
        [ListDrawerSettings(ShowFoldout = true, ShowItemCount = false)]
        [ShowIf("IsDurationalPolicy")]
        [InfoBox(ERROR_NONE_CUE, InfoMessageType.Error, VisibleIf = "IsCueDurationalNone")]
        [AssetSelector]
        [LabelText(GASTextDefine.TITLE_GE_CUE_CueDurational)]
        [Tooltip("生命周期完全和GameplayEffect同步")]
        public GameplayCueDurational[] CueDurational;

        [Space()]
        [TabGroup(GRP_DATA_CUE, "Cues")]
        [ListDrawerSettings(ShowFoldout = true, ShowItemCount = false)]
        [ShowIf("IsDurationalPolicy")]
        [AssetSelector]
        [LabelText(GASTextDefine.TITLE_GE_CUE_CueOnAdd)]
        public GameplayCueInstant[] CueOnAdd;

        [Space()]
        [TabGroup(GRP_DATA_CUE, "Cues")]
        [ListDrawerSettings(ShowFoldout = true, ShowItemCount = false)]
        [ShowIf("IsDurationalPolicy")]
        [AssetSelector]
        [LabelText(GASTextDefine.TITLE_GE_CUE_CueOnRemove)]
        public GameplayCueInstant[] CueOnRemove;

        [Space()]
        [TabGroup(GRP_DATA_CUE, "Cues")]
        [ListDrawerSettings(ShowFoldout = true, ShowItemCount = false)]
        [ShowIf("IsDurationalPolicy")]
        [AssetSelector]
        [LabelText(GASTextDefine.TITLE_GE_CUE_CueOnActivate)]
        public GameplayCueInstant[] CueOnActivate;

        [Space()]
        [TabGroup(GRP_DATA_CUE, "Cues")]
        [ListDrawerSettings(ShowFoldout = true, ShowItemCount = false)]
        [ShowIf("IsDurationalPolicy")]
        [AssetSelector]
        [LabelText(GASTextDefine.TITLE_GE_CUE_CueOnDeactivate)]
        public GameplayCueInstant[] CueOnDeactivate;

        #endregion Cues

        // TODO
        [HideInInspector]
        public ExecutionCalculation[] Executions;

        bool IsPeriodic()
        {
            return IsDurationalPolicy() && Period > 0;
        }

        bool IsDurationalPolicy()
        {
            return DurationPolicy == EffectsDurationPolicy.Duration || DurationPolicy == EffectsDurationPolicy.Infinite;
        }

        bool IsInstantPolicy() => DurationPolicy == EffectsDurationPolicy.Instant;

        bool IsCueExecuteNone() => CueOnExecute != null && CueOnExecute.Any(cue => cue == null);

        bool IsCueDurationalNone()
        {
            return (CueDurational != null && CueDurational.Any(cue => cue == null)) ||
                   (CueOnAdd != null && CueOnAdd.Any(cue => cue == null)) ||
                   (CueOnRemove != null && CueOnRemove.Any(cue => cue == null)) ||
                   (CueOnActivate != null && CueOnActivate.Any(cue => cue == null)) ||
                   (CueOnDeactivate != null && CueOnDeactivate.Any(cue => cue == null));
        }

        bool IsPeriodGameplayEffectNone()
        {
            return IsPeriodic() && PeriodExecution == null;
        }

        bool IsDurationInvalid() => DurationPolicy == EffectsDurationPolicy.Duration && Duration <= 0;
        bool IsPeriodInvalid() => IsDurationalPolicy() && Period < 0;

        bool IsGrantedAbilitiesInvalid()
        {
            return IsDurationalPolicy() &&
                   GrantedAbilities != null &&
                   GrantedAbilities.Any(abilityConfig => abilityConfig.AbilityAsset == null);
        }

        #region IGameplayEffectData

        public string GetDisplayName() => name;

        public EffectsDurationPolicy GetDurationPolicy() => DurationPolicy;

        public float GetDuration() => Duration;

        public float GetPeriod() => Period;

        public IGameplayEffectData GetPeriodExecution() => PeriodExecution;

        public GameplayTag[] GetAssetTags() => AssetTags;

        public GameplayTag[] GetGrantedTags() => GrantedTags;

        public GameplayTag[] GetApplicationRequiredTags() => ApplicationRequiredTags;

        public GameplayTag[] GetOngoingRequiredTags() => OngoingRequiredTags;

        public GameplayTag[] GetRemoveGameplayEffectsWithTags() => RemoveGameplayEffectsWithTags;

        public GameplayTag[] GetApplicationImmunityTags() => ApplicationImmunityTags;

        public GameplayCueInstant[] GetCueOnExecute() => CueOnExecute;

        public GameplayCueInstant[] GetCueOnRemove() => CueOnRemove;

        public GameplayCueInstant[] GetCueOnAdd() => CueOnAdd;

        public GameplayCueInstant[] GetCueOnActivate() => CueOnActivate;

        public GameplayCueInstant[] GetCueOnDeactivate() => CueOnDeactivate;

        public GameplayCueDurational[] GetCueDurational() => CueDurational;

        public GameplayEffectModifier[] GetModifiers() => Modifiers;

        public ExecutionCalculation[] GetExecutions() => Executions;

        public GrantedAbilityConfig[] GetGrantedAbilities() => GrantedAbilities;

        public GameplayEffectStacking GetStacking() => Stacking.ToRuntimeData();

        #endregion IGameplayEffectData
    }
}
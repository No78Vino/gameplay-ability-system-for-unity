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
        private const string GRP_DATA_CUE = "Data/H/Cues";
        private const string GRP_DATA_H2 = "Data/H2";
        private const string GRP_DATA_Snapshot = "Data/H2/Snapshot";
        private const string GRP_DATA_MOD = "Data/H2/Modifiers";
        private const string GRP_DATA_H3 = "Data/H3";
        private const string GRP_DATA_STACK = "Data/H3/Stack";
        private const string GRP_DATA_GRANTED_ABILITIES = "Data/H3/GrantedAbilities";

        private const int WIDTH_LABEL = 70;

        private const string ERROR_DURATION = "Duration must be > 0.";
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
        [PropertyOrder(1)]
        [LabelWidth(WIDTH_LABEL)]
        [LabelText(GASTextDefine.LABLE_GE_POLICY, SdfIconType.Diagram3Fill)]
        [EnumToggleButtons]
        public EffectsDurationPolicy DurationPolicy = EffectsDurationPolicy.Instant;

        [TabGroup(GRP_BASE_H_RIGHT, "Policy")]
        [PropertyOrder(2)]
        [LabelWidth(WIDTH_LABEL)]
        [LabelText(GASTextDefine.LABLE_GE_DURATION, SdfIconType.HourglassSplit)]
        [EnableIf("@DurationPolicy == EffectsDurationPolicy.Duration")]
        [Unit(Units.Second)]
        [ValidateInput("@DurationPolicy != EffectsDurationPolicy.Duration || Duration > 0", ERROR_DURATION)]
        public float Duration;

        [ShowIf("@DurationPolicy != EffectsDurationPolicy.Duration")]
        [TabGroup(GRP_BASE_H_RIGHT, "Policy")]
        [PropertyOrder(3)]
        [LabelWidth(WIDTH_LABEL)]
        [LabelText(GASTextDefine.LABLE_GE_INTERVAL, SdfIconType.AlarmFill)]
        [EnableIf("IsDurationalPolicy")]
        [Unit(Units.Second)]
        [ValidateInput("@DurationPolicy != EffectsDurationPolicy.Infinite || Period <= 0 || Period >= 0.01f", "Period < 0.01", InfoMessageType.Warning)]
        public float Period;

        [ShowIf("@DurationPolicy == EffectsDurationPolicy.Duration"),]
        [TabGroup(GRP_BASE_H_RIGHT, "Policy")]
        [PropertyOrder(3)]
        [ShowInInspector]
        [LabelWidth(WIDTH_LABEL)]
        [LabelText(GASTextDefine.LABLE_GE_INTERVAL, SdfIconType.AlarmFill)]
        [EnableIf("IsDurationalPolicy")]
        [Unit(Units.Second)]
        [PropertyRange(0, "@Duration")]
        [ValidateInput("@DurationPolicy != EffectsDurationPolicy.Duration || Period <= 0 || Period >= 0.01f", "Period < 0.01", InfoMessageType.Warning)]
        // 这个Property是为了给"限时型"效果绘制一个范围滑动条
        public float PeriodForDurational
        {
            get => Period;
            set => Period = value;
        }

        [TabGroup(GRP_BASE_H_RIGHT, "Policy")]
        [PropertyOrder(4)]
        [LabelWidth(WIDTH_LABEL)]
        [LabelText(GASTextDefine.LABLE_GE_EXEC, SdfIconType.Magic)]
        [EnableIf("IsPeriodic")]
        [AssetSelector]
        [ValidateInput("@IsPeriodic() ? (PeriodExecution != null && PeriodExecution.DurationPolicy == EffectsDurationPolicy.Instant) : true", "非空, 瞬时效果")]
        public GameplayEffectAsset PeriodExecution;

        #endregion Policy

        #region Stack

        [TitleGroup(GRP_DATA)]
        [HorizontalGroup(GRP_DATA_H3, order: 3, Width = 1 - 0.618f)]
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

        [TabGroup(GRP_DATA_GRANTED_ABILITIES, "Granted Abilities", SdfIconType.YinYang, TextColor = "#D6626E", Order = 2)]
        [EnableIf("IsDurationalPolicy")]
        [InfoBox("瞬时效果无法赋予能力", InfoMessageType.None, VisibleIf = "@IsInstantPolicy()")]
        [ListDrawerSettings(ShowFoldout = true, ShowItemCount = false, DraggableItems = false)]
        [DisableContextMenu(disableForMember: false, disableCollectionElements: true)]
        [CustomContextMenu("排序", "@SortGrantedAbilities()")]
        [ValidateInput("@IsGrantedAbilitiesInvalid() ? false : true", ERROR_GRANTED_ABILITY_INVALID)]
        public GrantedAbilityConfig[] GrantedAbilities;

        private void SortGrantedAbilities()
        {
            GrantedAbilities = GrantedAbilities?.OrderBy(abilityConfig => abilityConfig.AbilityAsset.name).ToArray();
        }

        #endregion Granted Abilities

        #region Modifiers

        [TabGroup(GRP_DATA_MOD, "Modifiers", SdfIconType.CalculatorFill, TextColor = "#FFE60B", Order = 2)]
        [LabelText(@"@IsInstantPolicy() ? ""依次执行(仅在成功应用时)"" : ""依次执行(每次激活时)""")]
        [ListDrawerSettings(ShowFoldout = true, ShowItemCount = false)]
        [ValidateInput("@ValidateInput_Modifiers()", "瞬时效果只能修改Stacking类型的属性")]
        public GameplayEffectModifier[] Modifiers;

        bool ValidateInput_Modifiers()
        {
            if (!IsInstantPolicy()) return true;
            if (Modifiers == null) return true;
            return Modifiers.All(modifier =>
            {
                var attributeBase = ReflectionHelper.GetAttribute(modifier.AttributeName);
                if (attributeBase != null)
                {
                    return attributeBase.CalculateMode == CalculateMode.Stacking;
                }

                return true;
            });
        }

        #endregion Modifiers

        #region Tags

        [ShowIf("IsDurationalPolicy")]
        [HorizontalGroup(GRP_DATA_H, order: 1, Width = 1 - 0.618f)]
        [TabGroup(GRP_DATA_TAG, "Tags", SdfIconType.TagsFill, TextColor = "#45B1FF", Order = 1)]
        [LabelText(GASTextDefine.TITLE_GE_TAG_AssetTags)]
        [Tooltip(GASTextDefine.TIP_GE_TAG_AssetTags)]
        [ListDrawerSettings(ShowFoldout = true, ShowItemCount = false, DraggableItems = false)]
        [DisableContextMenu(disableForMember: false, disableCollectionElements: true)]
        [CustomContextMenu("排序", "@AssetTags = TagHelper.Sort($value)")]
        [ValueDropdown("@ValueDropdownHelper.GameplayTagChoices", IsUniqueList = true, HideChildProperties = true)]
        public GameplayTag[] AssetTags;

        [ShowIf("IsDurationalPolicy")]
        [Space]
        [TabGroup(GRP_DATA_TAG, "Tags")]
        [LabelText(GASTextDefine.TITLE_GE_TAG_GrantedTags)]
        [Tooltip(GASTextDefine.TIP_GE_TAG_GrantedTags)]
        [ListDrawerSettings(ShowFoldout = true, ShowItemCount = false, DraggableItems = false)]
        [DisableContextMenu(disableForMember: false, disableCollectionElements: true)]
        [CustomContextMenu("排序", "@GrantedTags = TagHelper.Sort($value)")]
        [ValueDropdown("@ValueDropdownHelper.GameplayTagChoices", IsUniqueList = true, HideChildProperties = true)]
        public GameplayTag[] GrantedTags;

        [Space]
        [TabGroup(GRP_DATA_TAG, "Tags")]
        [LabelText(GASTextDefine.TITLE_GE_TAG_ApplicationRequiredTags)]
        [Tooltip(GASTextDefine.TIP_GE_TAG_ApplicationRequiredTags)]
        [ListDrawerSettings(ShowFoldout = true, ShowItemCount = false, DraggableItems = false)]
        [DisableContextMenu(disableForMember: false, disableCollectionElements: true)]
        [CustomContextMenu("排序", "@ApplicationRequiredTags = TagHelper.Sort($value)")]
        [ValueDropdown("@ValueDropdownHelper.GameplayTagChoices", IsUniqueList = true, HideChildProperties = true)]
        public GameplayTag[] ApplicationRequiredTags;

        [ShowIf("IsDurationalPolicy")]
        [Space]
        [TabGroup(GRP_DATA_TAG, "Tags")]
        [LabelText(GASTextDefine.TITLE_GE_TAG_OngoingRequiredTags)]
        [Tooltip(GASTextDefine.TIP_GE_TAG_OngoingRequiredTags)]
        [ListDrawerSettings(ShowFoldout = true, ShowItemCount = false, DraggableItems = false)]
        [DisableContextMenu(disableForMember: false, disableCollectionElements: true)]
        [CustomContextMenu("排序", "@OngoingRequiredTags = TagHelper.Sort($value)")]
        [ValueDropdown("@ValueDropdownHelper.GameplayTagChoices", IsUniqueList = true, HideChildProperties = true)]
        public GameplayTag[] OngoingRequiredTags;

        [Space]
        [TabGroup(GRP_DATA_TAG, "Tags")]
        [LabelText(GASTextDefine.TITLE_GE_TAG_RemoveGameplayEffectsWithTags)]
        [Tooltip(GASTextDefine.TIP_GE_TAG_RemoveGameplayEffectsWithTags)]
        [ListDrawerSettings(ShowFoldout = true, ShowItemCount = false, DraggableItems = false)]
        [DisableContextMenu(disableForMember: false, disableCollectionElements: true)]
        [CustomContextMenu("排序", "@RemoveGameplayEffectsWithTags = TagHelper.Sort($value)")]
        [ValueDropdown("@ValueDropdownHelper.GameplayTagChoices", IsUniqueList = true, HideChildProperties = true)]
        public GameplayTag[] RemoveGameplayEffectsWithTags;

        [Space]
        [TabGroup(GRP_DATA_TAG, "Tags")]
        [LabelText(GASTextDefine.TITLE_GE_TAG_ApplicationImmunityTags)]
        [Tooltip(GASTextDefine.TIP_GE_TAG_ApplicationImmunityTags)]
        [ListDrawerSettings(ShowFoldout = true, ShowItemCount = false, DraggableItems = false)]
        [DisableContextMenu(disableForMember: false, disableCollectionElements: true)]
        [CustomContextMenu("排序", "@ApplicationImmunityTags = TagHelper.Sort($value)")]
        [ValueDropdown("@ValueDropdownHelper.GameplayTagChoices", IsUniqueList = true, HideChildProperties = true)]
        public GameplayTag[] ApplicationImmunityTags;

        #endregion Tags

        #region Cues

        [ShowIf("IsInstantPolicy")]
        [TabGroup(GRP_DATA_CUE, "Cues", SdfIconType.Stars, TextColor = "#00FFFF", Order = 3)]
        [LabelText(GASTextDefine.TITLE_GE_CUE_CueOnExecute)]
        [ListDrawerSettings(ShowFoldout = true, ShowItemCount = false, DraggableItems = false)]
        [DisableContextMenu(disableForMember: false, disableCollectionElements: true)]
        [AssetSelector]
        public GameplayCueInstant[] CueOnExecute;

        [ShowIf("IsDurationalPolicy")]
        [Space]
        [TabGroup(GRP_DATA_CUE, "Cues")]
        [LabelText(GASTextDefine.TITLE_GE_CUE_CueDurational)]
        [Tooltip("生命周期完全和GameplayEffect同步")]
        [ListDrawerSettings(ShowFoldout = true, ShowItemCount = false, DraggableItems = false)]
        [DisableContextMenu(disableForMember: false, disableCollectionElements: true)]
        [AssetSelector]
        public GameplayCueDurational[] CueDurational;

        [ShowIf("IsDurationalPolicy")]
        [Space]
        [TabGroup(GRP_DATA_CUE, "Cues")]
        [LabelText(GASTextDefine.TITLE_GE_CUE_CueOnAdd)]
        [ListDrawerSettings(ShowFoldout = true, ShowItemCount = false, DraggableItems = false)]
        [DisableContextMenu(disableForMember: false, disableCollectionElements: true)]
        [AssetSelector]
        public GameplayCueInstant[] CueOnAdd;

        [ShowIf("IsDurationalPolicy")]
        [Space]
        [TabGroup(GRP_DATA_CUE, "Cues")]
        [LabelText(GASTextDefine.TITLE_GE_CUE_CueOnRemove)]
        [ListDrawerSettings(ShowFoldout = true, ShowItemCount = false, DraggableItems = false)]
        [DisableContextMenu(disableForMember: false, disableCollectionElements: true)]
        [AssetSelector]
        public GameplayCueInstant[] CueOnRemove;

        [ShowIf("IsDurationalPolicy")]
        [Space]
        [TabGroup(GRP_DATA_CUE, "Cues")]
        [LabelText(GASTextDefine.TITLE_GE_CUE_CueOnActivate)]
        [ListDrawerSettings(ShowFoldout = true, ShowItemCount = false, DraggableItems = false)]
        [DisableContextMenu(disableForMember: false, disableCollectionElements: true)]
        [AssetSelector]
        public GameplayCueInstant[] CueOnActivate;

        [ShowIf("IsDurationalPolicy")]
        [Space]
        [TabGroup(GRP_DATA_CUE, "Cues")]
        [LabelText(GASTextDefine.TITLE_GE_CUE_CueOnDeactivate)]
        [ListDrawerSettings(ShowFoldout = true, ShowItemCount = false, DraggableItems = false)]
        [DisableContextMenu(disableForMember: false, disableCollectionElements: true)]
        [AssetSelector]
        public GameplayCueInstant[] CueOnDeactivate;

        #endregion Cues

        #region Snapshot

        [HorizontalGroup(GRP_DATA_H2, order: 2, Width = 1 - 0.618f)]
        [TabGroup(GRP_DATA_Snapshot, "Snapshots", SdfIconType.Camera, TextColor = "#FF7F00", Order = 1)]
        [LabelWidth(WIDTH_LABEL)]
        [LabelText(GASTextDefine.LABLE_GE_SnapshotPolicy, SdfIconType.Camera)]
        [EnumToggleButtons]
        public GameplayEffectSnapshotPolicy SnapshotPolicy = GameplayEffectSnapshotPolicy.Specified;

        [ShowIf("@SnapshotPolicy == GameplayEffectSnapshotPolicy.Specified")]
        [TabGroup(GRP_DATA_Snapshot, "Snapshots")]
        [LabelText("需要快照的属性")]
        [ListDrawerSettings(ShowFoldout = true, ShowItemCount = false, DraggableItems = false)]
        public GameplayEffectSpecifiedSnapshotConfig[] SpecifiedSnapshotConfigs;

        #endregion Snapshot

        // TODO
        [HideInInspector]
        public ExecutionCalculation[] Executions;

        bool IsPeriodic() => IsDurationalPolicy() && Period > 0;

        bool IsDurationalPolicy() => DurationPolicy == EffectsDurationPolicy.Duration || DurationPolicy == EffectsDurationPolicy.Infinite;

        bool IsInstantPolicy() => DurationPolicy == EffectsDurationPolicy.Instant;

        bool IsGrantedAbilitiesInvalid() => IsDurationalPolicy() && GrantedAbilities != null && GrantedAbilities.Any(abilityConfig => abilityConfig.AbilityAsset == null);

        #region IGameplayEffectData

        public string GetDisplayName() => name;

        public EffectsDurationPolicy GetDurationPolicy() => DurationPolicy;

        public float GetDuration() => Duration;

        public float GetPeriod() => Period;

        public IGameplayEffectData GetPeriodExecution() => PeriodExecution;

        public GameplayEffectSnapshotPolicy GetSnapshotPolicy() => SnapshotPolicy;

        public GameplayEffectSpecifiedSnapshotConfig[] GetSpecifiedSnapshotConfigs() => SpecifiedSnapshotConfigs;

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

        #region shared GameplayEffect instance

        /// <summary>
        /// 共享实例, 一个GameplayEffectAsset对应一个共享实例, 首次访问时创建
        /// <remarks>
        /// <para>优点: 通过共享实例, 可以减少GameplayEffect的实例化次数, 减少内存开销, 同时也可以减少GC的产生, 提高性能</para>
        /// <para>缺点: Editor下实时修改GameplayEffectAsset无法实时生效, 因为共享实例一旦创建, 就不会再改变, 可以设置GasRuntimeSettings.DisableGameplayEffectSharedInstance来禁用Editor模式下的SharedInstance</para>
        /// </remarks> 
        /// </summary>
        public GameplayEffect SharedInstance
        {
            get
            {
#if UNITY_EDITOR
                if (GasRuntimeSettings.DisableGameplayEffectSharedInstance)
                    return new GameplayEffect(this);
#endif
                return _sharedInstance ??= new GameplayEffect(this);
            }
        }

        private GameplayEffect _sharedInstance;

        #endregion shared GameplayEffect instance
    }
}
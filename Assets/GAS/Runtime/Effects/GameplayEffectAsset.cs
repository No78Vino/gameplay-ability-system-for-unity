using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using GAS.General;
using Sirenix.OdinInspector;
using UnityEngine;

namespace GAS.Runtime
{
    [CreateAssetMenu(fileName = "GameplayEffect", menuName = "GAS/GameplayEffect")]
    public class GameplayEffectAsset : ScriptableObject
    {
        private const string GRP_BASE = "Base Info";
        private const string GRP_BASE_H = "Base Info/H";
        private const string GRP_BASE_H_LEFT = "Base Info/H/Left";
        private const string GRP_BASE_H_RIGHT = "Base Info/H/Right";
        private const string GRP_BASE_H_RIGHT_PERIOD = "Base Info/H/Right/Period";
        private const string GRP_BASE_H_RIGHT_POLICY = "Base Info/H/Right/Policy";


        private const string GRP_DATA = "DATA";
        private const string GRP_DATA_H = "DATA/H";
        private const string GRP_DATA_MOD = "DATA/H/Modifiers";
        private const string GRP_DATA_TAG = "DATA/H/Tags";
        private const string GRP_DATA_CUE = "DATA/H/Cue";

        private const int WIDTH_LABEL = 100;
        private const int WIDTH_GRP_BASE_H_LEFT = 250;
        private const int WIDTH_GRP_BASE_H_RIGHT = 500;
        private const int WIDTH_GRP_EACH_TAG = 250;

        private const string ERROR_POLICY = "Policy CAN NOT be NONE!";
        private const string ERROR_NONE_CUE = "Cue CAN NOT be NONE!";
        private const string ERROR_DURATION = "Duration must be > 0.";
        private const string ERROR_PERIOD = "Period must be >= 0.";
        private const string ERROR_PERIOD_GE_NONE = "Period GameplayEffect CAN NOT be NONE!";


        private static IEnumerable TagChoices = new ValueDropdownList<GameplayTag>();

        [BoxGroup(GRP_BASE, false)]
        [InfoBox(GASTextDefine.TIP_BASEINFO)]
        [Title(GASTextDefine.TITLE_BASEINFO, bold: true)]
        [HorizontalGroup(GRP_BASE_H, Width = WIDTH_GRP_BASE_H_LEFT)]
        [VerticalGroup(GRP_BASE_H_LEFT)]
        [LabelText(GASTextDefine.LABLE_GE_NAME)]
        public string Name;

        [VerticalGroup(GRP_BASE_H_LEFT)]
        [Title(GASTextDefine.TITLE_DESCRIPTION, bold: false)]
        [HideLabel]
        [MultiLineProperty(5)]
        public string Description;


        [Title(GASTextDefine.TITLE_GE_POLICY, bold: true)]
        [HorizontalGroup(GRP_BASE_H, PaddingLeft = 0.025f)]
        [VerticalGroup(GRP_BASE_H_RIGHT)]
        [LabelText(GASTextDefine.LABLE_GE_POLICY)]
        [LabelWidth(WIDTH_LABEL)]
        [InfoBox(ERROR_DURATION, InfoMessageType.Error, "IsDurationInvalid")]
        [InfoBox(ERROR_PERIOD, InfoMessageType.Error, "IsPeriodInvalid")]
        [InfoBox(ERROR_PERIOD_GE_NONE, InfoMessageType.Error, VisibleIf = "IsPeriodGameplayEffectNone")]
        [InfoBox(GASTextDefine.TIP_GE_POLICY)]
        public EffectsDurationPolicy DurationPolicy = EffectsDurationPolicy.Instant;

        [VerticalGroup(GRP_BASE_H_RIGHT)]
        [LabelWidth(WIDTH_LABEL)]
        [ShowIf("DurationPolicy", EffectsDurationPolicy.Duration)]
        [Unit(Units.Second)]
        [LabelText(GASTextDefine.LABLE_GE_DURATION)]
        public float Duration;

        [VerticalGroup(GRP_BASE_H_RIGHT)]
        [HorizontalGroup(GRP_BASE_H_RIGHT_PERIOD, width: 100)]
        [LabelText(GASTextDefine.LABLE_GE_PER)]
        [LabelWidth(25)]
        [ShowIf("IsDurationalPolicy")]
        [Unit(Units.Second)]
        public float Period;
        
        [HorizontalGroup(GRP_BASE_H_RIGHT_PERIOD)]
        [LabelText(GASTextDefine.LABLE_GE_EXEC)]
        [LabelWidth(50)]
        [ShowIf("IsPeriodic")]
        [AssetSelector]
        public GameplayEffectAsset PeriodExecution;

        [Space]
        [VerticalGroup(GRP_BASE_H_RIGHT)]
        [Title(GASTextDefine.TITLE_GE_GrantedAbilities, Bold = true)]
        [AssetSelector]
        [ShowIf("IsDurationalPolicy")]
        [ListDrawerSettings(Expanded = true, ShowIndexLabels = false, ShowItemCount = false)]
        public AbilityAsset[] GrantedAbilities;

        // Mod
        [Title(GASTextDefine.TITLE_GE_MOD, bold: true)]
        [BoxGroup(GRP_DATA, false)]
        [HorizontalGroup(GRP_DATA_H)]
        [VerticalGroup(GRP_DATA_MOD)]
        [ListDrawerSettings(Expanded = true, ShowIndexLabels = false, ShowItemCount = false)]
        public GameplayEffectModifier[] Modifiers;

        // Tag Container
        [Title(GASTextDefine.TITLE_GE_TAG, bold: true)]
        [HorizontalGroup(GRP_DATA_H)]
        [VerticalGroup(GRP_DATA_TAG)]
        [ListDrawerSettings(Expanded = true)]
        [ValueDropdown("TagChoices", HideChildProperties = true)]
        [LabelText(GASTextDefine.TITLE_GE_TAG_AssetTags)]
        [Tooltip(GASTextDefine.TIP_GE_TAG_AssetTags)]
        public GameplayTag[] AssetTags;

        [Title("")]
        [VerticalGroup(GRP_DATA_TAG)]
        [ListDrawerSettings(Expanded = true)]
        [ValueDropdown("TagChoices", HideChildProperties = true)]
        [LabelText(GASTextDefine.TITLE_GE_TAG_GrantedTags)]
        [Tooltip(GASTextDefine.TIP_GE_TAG_GrantedTags)]
        public GameplayTag[] GrantedTags;

        [Title("")]
        [VerticalGroup(GRP_DATA_TAG)]
        [ListDrawerSettings(Expanded = true)]
        [ValueDropdown("TagChoices", HideChildProperties = true)]
        [LabelText(GASTextDefine.TITLE_GE_TAG_ApplicationRequiredTags)]
        [Tooltip(GASTextDefine.TIP_GE_TAG_ApplicationRequiredTags)]
        public GameplayTag[] ApplicationRequiredTags;

        [Title("")]
        [VerticalGroup(GRP_DATA_TAG)]
        [ListDrawerSettings(Expanded = true)]
        [ValueDropdown("TagChoices", HideChildProperties = true)]
        [LabelText(GASTextDefine.TITLE_GE_TAG_OngoingRequiredTags)]
        [Tooltip(GASTextDefine.TIP_GE_TAG_OngoingRequiredTags)]
        public GameplayTag[] OngoingRequiredTags;

        [Title("")]
        [VerticalGroup(GRP_DATA_TAG)]
        [ListDrawerSettings(Expanded = true)]
        [ValueDropdown("TagChoices", HideChildProperties = true)]
        [LabelText(GASTextDefine.TITLE_GE_TAG_RemoveGameplayEffectsWithTags)]
        [Tooltip(GASTextDefine.TIP_GE_TAG_RemoveGameplayEffectsWithTags)]
        public GameplayTag[] RemoveGameplayEffectsWithTags;

        [Title("")]
        [VerticalGroup(GRP_DATA_TAG)]
        [ListDrawerSettings(Expanded = true)]
        [ValueDropdown("TagChoices", HideChildProperties = true)]
        [LabelText(GASTextDefine.TITLE_GE_TAG_ApplicationImmunityTags)]
        [Tooltip(GASTextDefine.TIP_GE_TAG_ApplicationImmunityTags)]
        public GameplayTag[] ApplicationImmunityTags;


        // Cues
        [Title(GASTextDefine.TITLE_GE_CUE, bold: true)]
        [HorizontalGroup(GRP_DATA_H)]
        [VerticalGroup(GRP_DATA_CUE)]
        [ListDrawerSettings(Expanded = true)]
        [ShowIf("IsInstantPolicy")]
        [InfoBox(ERROR_NONE_CUE, InfoMessageType.Error, VisibleIf = "IsCueExecuteNone")]
        [AssetSelector]
        [LabelText(GASTextDefine.TITLE_GE_CUE_CueOnExecute)]
        public GameplayCueInstant[] CueOnExecute;

        [Title(GASTextDefine.TITLE_GE_CUE, bold: true)]
        [VerticalGroup(GRP_DATA_CUE)]
        [ListDrawerSettings(Expanded = true)]
        [ShowIf("IsDurationalPolicy")]
        [InfoBox(ERROR_NONE_CUE, InfoMessageType.Error, VisibleIf = "IsCueDurationalNone")]
        [AssetSelector]
        [LabelText(GASTextDefine.TITLE_GE_CUE_CueDurational)]
        [Tooltip("生命周期完全和GameplayEffect同步")]
        public GameplayCueDurational[] CueDurational;

        [Title("")]
        [VerticalGroup(GRP_DATA_CUE)]
        [ListDrawerSettings(Expanded = true)]
        [ShowIf("IsDurationalPolicy")]
        [AssetSelector]
        [LabelText(GASTextDefine.TITLE_GE_CUE_CueOnAdd)]
        public GameplayCueInstant[] CueOnAdd;

        [Title("")]
        [VerticalGroup(GRP_DATA_CUE)]
        [ListDrawerSettings(Expanded = true)]
        [ShowIf("IsDurationalPolicy")]
        [AssetSelector]
        [LabelText(GASTextDefine.TITLE_GE_CUE_CueOnRemove)]
        public GameplayCueInstant[] CueOnRemove;

        [Title("")]
        [VerticalGroup(GRP_DATA_CUE)]
        [ListDrawerSettings(Expanded = true)]
        [ShowIf("IsDurationalPolicy")]
        [AssetSelector]
        [LabelText(GASTextDefine.TITLE_GE_CUE_CueOnActivate)]
        public GameplayCueInstant[] CueOnActivate;

        [Title("")]
        [VerticalGroup(GRP_DATA_CUE)]
        [ListDrawerSettings(Expanded = true)]
        [ShowIf("IsDurationalPolicy")]
        [AssetSelector]
        [LabelText(GASTextDefine.TITLE_GE_CUE_CueOnDeactivate)]
        public GameplayCueInstant[] CueOnDeactivate;


        // TODO
        [HideInInspector] public ExecutionCalculation[] Executions;


        private void OnEnable()
        {
            SetTagChoices();
            GameplayEffectModifier.SetAttributeChoices();
        }

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

        private static void SetTagChoices()
        {
            Type gameplayTagSumCollectionType = TypeUtil.FindTypeInAllAssemblies("GAS.Runtime.GTagLib");
            if (gameplayTagSumCollectionType == null)
            {
                Debug.LogError("[EX] Type 'GTagLib' not found. Please generate the TAGS CODE first!");
                TagChoices = new ValueDropdownList<GameplayTag>();
                return;
            }

            FieldInfo tagMapField =
                gameplayTagSumCollectionType.GetField("TagMap", BindingFlags.Public | BindingFlags.Static);

            if (tagMapField != null)
            {
                Dictionary<string, GameplayTag> tagMapValue =
                    (Dictionary<string, GameplayTag>)tagMapField.GetValue(null);
                var tagChoices = tagMapValue.Values.ToList();
                var choices = new ValueDropdownList<GameplayTag>();
                foreach (var tag in tagChoices) choices.Add(tag.Name, tag);
                TagChoices = choices;
            }
            else
            {
                TagChoices = new ValueDropdownList<GameplayTag>();
            }
        }
    }
}
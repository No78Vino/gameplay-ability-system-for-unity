using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using GAS.Runtime.Cue;
using GAS.Runtime.Effects.Execution;
using GAS.Runtime.Effects.Modifier;
using GAS.Runtime.Tags;
using Sirenix.OdinInspector;
using UnityEngine;

namespace GAS.Runtime.Effects
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

        private const int WIDTH_LABLE = 100;
        private const int WIDTH_GRP_BASE_H_LEFT = 250;
        private const int WIDTH_GRP_BASE_H_RIGHT = 500;
        private const int WIDTH_GRP_EACH_TAG = 250;

        private const string ERROR_POLICY = "Policy CAN NOT be NONE!";
        private const string ERROR_NONE_CUE = "Cue CAN NOT be NONE!";
        private const string ERROR_PERIODGE_NONE = "Period GameplayEffect CAN NOT be NONE!";
        
        
        
        
        
        private static IEnumerable TagChoices = new ValueDropdownList<GameplayTag>();
        
        [BoxGroup(GRP_BASE,false)]
        [Title("Base Information", bold: true)]
        [HorizontalGroup(GRP_BASE_H,Width = WIDTH_GRP_BASE_H_LEFT)]
        [VerticalGroup(GRP_BASE_H_LEFT)]
        public string Name;
        
        [VerticalGroup(GRP_BASE_H_LEFT)]
        [Title("Description", bold: false)]
        [HideLabel]
        [MultiLineProperty(5)]
        public string Description;
        
        
        [Title("Gameplay Effect Policy", bold: true)]
        [HorizontalGroup(GRP_BASE_H,PaddingLeft = 0.025f)]
        [VerticalGroup(GRP_BASE_H_RIGHT)]
        //[HorizontalGroup(GRP_BASE_H_RIGHT_POLICY)]
        [LabelWidth(WIDTH_LABLE)]
        [InfoBox(ERROR_POLICY,InfoMessageType.Error, VisibleIf = "IsDurationPolicyNone")]
        [InfoBox(ERROR_PERIODGE_NONE,InfoMessageType.Error, VisibleIf = "IsPeriodGameplayEffectNone")]
        public EffectsDurationPolicy DurationPolicy;
        
        [VerticalGroup(GRP_BASE_H_RIGHT)]
        [LabelWidth(WIDTH_LABLE)]
        [ShowIf("DurationPolicy",EffectsDurationPolicy.Duration)]
        [Unit(Units.Second)]
        public float Duration;
        
        [VerticalGroup(GRP_BASE_H_RIGHT)]
        [HorizontalGroup(GRP_BASE_H_RIGHT_PERIOD,width:100)]
        [LabelText("Every")]
        [LabelWidth(50)]
        [ShowIf("IsDurationalPolicy")]
        [Unit(Units.Second)]
        public float Period;
        
        [HorizontalGroup(GRP_BASE_H_RIGHT_PERIOD)]
        [LabelText(" execute")]
        [LabelWidth(50)]
        [ShowIf("IsPeriodic")]
        [AssetSelector]
        public GameplayEffectAsset PeriodExecution;

        // Mod
        [Title("Modifier",bold:true)]
        [BoxGroup(GRP_DATA,false)]
        [HorizontalGroup(GRP_DATA_H)]
        [VerticalGroup(GRP_DATA_MOD)]
        [ListDrawerSettings(Expanded = true,ShowIndexLabels = false,ShowItemCount = false)]
        public GameplayEffectModifier[] Modifiers;
        
        // Tag Container
        [Title("Tags",bold:true)]
        [HorizontalGroup(GRP_DATA_H)]
        [VerticalGroup(GRP_DATA_TAG)]
        [ListDrawerSettings(Expanded = true)]
        [ValueDropdown("TagChoices",HideChildProperties = true)]
        public GameplayTag[] AssetTags;
        
        [Title("")]
        [VerticalGroup(GRP_DATA_TAG)]
        [ListDrawerSettings(Expanded = true)]
        [ValueDropdown("TagChoices",HideChildProperties = true)]
        public GameplayTag[] GrantedTags;
        
        [Title("")]
        [VerticalGroup(GRP_DATA_TAG)]
        [ListDrawerSettings(Expanded = true)]
        [ValueDropdown("TagChoices",HideChildProperties = true)]
        public GameplayTag[] ApplicationRequiredTags;
        
        [Title("")]
        [VerticalGroup(GRP_DATA_TAG)]
        [ListDrawerSettings(Expanded = true)]
        [ValueDropdown("TagChoices",HideChildProperties = true)]
        public GameplayTag[] OngoingRequiredTags;
        
        [Title("")]
        [VerticalGroup(GRP_DATA_TAG)]
        [ListDrawerSettings(Expanded = true)]
        [ValueDropdown("TagChoices",HideChildProperties = true)]
        public GameplayTag[] RemoveGameplayEffectsWithTags;

        // Cues
        [Title("Cue",bold:true)]
        [HorizontalGroup(GRP_DATA_H)]
        [VerticalGroup(GRP_DATA_CUE)]
        [ListDrawerSettings(Expanded = true)]
        [ShowIf("IsInstantPolicy")]
        [InfoBox(ERROR_NONE_CUE,InfoMessageType.Error, VisibleIf = "IsCueExecuteNone")]
        [AssetSelector]
        public GameplayCueInstant[] CueOnExecute;
        
        [Title("Cue",bold:true)]
        [VerticalGroup(GRP_DATA_CUE)]
        [ListDrawerSettings(Expanded = true)]
        [ShowIf("IsDurationalPolicy")]
        [InfoBox(ERROR_NONE_CUE,InfoMessageType.Error, VisibleIf = "IsCueDurationalNone")]
        [AssetSelector]
        public GameplayCueDurational[] CueDurational;
        
        [Title("")]
        [VerticalGroup(GRP_DATA_CUE)]
        [ListDrawerSettings(Expanded = true)]
        [ShowIf("IsDurationalPolicy")]
        [AssetSelector]
        public GameplayCueInstant[] CueOnAdd;
        
        [Title("")]
        [VerticalGroup(GRP_DATA_CUE)]
        [ListDrawerSettings(Expanded = true)]
        [ShowIf("IsDurationalPolicy")]
        [AssetSelector]
        public GameplayCueInstant[] CueOnRemove;
        
        [Title("")]
        [VerticalGroup(GRP_DATA_CUE)]
        [ListDrawerSettings(Expanded = true)]
        [ShowIf("IsDurationalPolicy")]
        [AssetSelector]
        public GameplayCueInstant[] CueOnActivate;
        
        [Title("")]
        [VerticalGroup(GRP_DATA_CUE)]
        [ListDrawerSettings(Expanded = true)]
        [ShowIf("IsDurationalPolicy")]
        [AssetSelector]
        public GameplayCueInstant[] CueOnDeactivate;


        // TODO
        [HideInInspector]
        public ExecutionCalculation[] Executions;


        
        
        
        
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

        bool IsDurationPolicyNone() => DurationPolicy == EffectsDurationPolicy.None;
        
        bool IsInstantPolicy() => DurationPolicy == EffectsDurationPolicy.Instant;

        bool IsCueExecuteNone() => CueOnExecute.Any(cue => cue == null);

        bool IsCueDurationalNone()
        {
            return CueDurational.Any(cue => cue == null) ||
                   CueOnAdd.Any(cue => cue == null) ||
                   CueOnRemove.Any(cue => cue == null) ||
                   CueOnActivate.Any(cue => cue == null) ||
                   CueOnDeactivate.Any(cue => cue == null);
        }

        bool IsPeriodGameplayEffectNone()
        {
            return IsPeriodic() && PeriodExecution == null;
        }
        
        private static void SetTagChoices()
        {
            Type gameplayTagSumCollectionType = Type.GetType($"GAS.Runtime.Tags.GameplayTagSumCollection, Assembly-CSharp");
            if(gameplayTagSumCollectionType == null)
            {
                Debug.LogError("[EX] Type 'GameplayTagSumCollection' not found. Please generate the TAGS CODE first!");
                TagChoices = new ValueDropdownList<GameplayTag>();
                return;
            }
            FieldInfo tagMapField = gameplayTagSumCollectionType.GetField("TagMap", BindingFlags.Public | BindingFlags.Static);

            if (tagMapField != null)
            {
                Dictionary<string, GameplayTag> tagMapValue = (Dictionary<string, GameplayTag>)tagMapField.GetValue(null);
                var tagChoices = tagMapValue.Values.ToList();
                var choices = new ValueDropdownList<GameplayTag>();
                foreach (var tag in tagChoices) choices.Add(tag.Name,tag);
                TagChoices = choices;
            }
            else
            {
                TagChoices = new ValueDropdownList<GameplayTag>();
            }
        }
    }
}
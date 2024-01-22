using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using GAS.Runtime.Cue;
using GAS.Runtime.Effects;
using GAS.Runtime.Tags;
using Sirenix.OdinInspector;
using UnityEngine;

namespace GAS.Runtime.Ability
{
    [CreateAssetMenu(fileName = "AbilityAsset", menuName = "GAS/AbilityAsset", order = 0)]
    public class AbilityAsset : ScriptableObject
    {
        private const string GRP_BASE = "Base Info";
        private const string GRP_BASE_H = "Base Info/H";
        private const string GRP_BASE_H_LEFT = "Base Info/H/Left";
        private const string GRP_BASE_H_RIGHT = "Base Info/H/Right";
        
        private const string GRP_DATA = "DATA";
        private const string GRP_DATA_H = "DATA/H";
        private const string GRP_DATA_PARAMETER = "DATA/H/Parameter";
        private const string GRP_DATA_SUMMON = "DATA/H/Summon";
        private const string GRP_DATA_TAG = "DATA/H/Tags";
        private const string GRP_CUE = "CUE";
        private const string GRP_CUE_INSTANT = "CUE/Instant";
        private const string GRP_CUE_DURATIONAL = "CUE/Durational";
        
        
        private const int WIDTH_LABLE = 100;
        private const int WIDTH_GRP_BASE_H_LEFT = 350;

        private const string TIP_UNAME = "<size=12><b><color=white><color=orange>Unique Name is very important!</color>" +
                                         "GAS will use the unique name as a UID for the ability." +
                                         "Therefore,you must keep this name unique." +
                                         "Don't worry.When generating the code, the tool will check this.</color></b></size>";


        private static IEnumerable AbilityClassChoice = new ValueDropdownList<string>();
        private static IEnumerable TagChoices = new ValueDropdownList<GameplayTag>();
        
        
        
        [BoxGroup(GRP_BASE, false)]
        [Title("Base Information", bold: true)]
        [HorizontalGroup(GRP_BASE_H, Width = WIDTH_GRP_BASE_H_LEFT)]
        [VerticalGroup(GRP_BASE_H_LEFT)]
        [InfoBox(TIP_UNAME)]
        [LabelText("U-Name")]
        [LabelWidth(WIDTH_LABLE)]
        public string UniqueName;
        
        [VerticalGroup(GRP_BASE_H_LEFT)]
        [LabelText("Class",SdfIconType.FileEarmarkMedical)]
        [LabelWidth(WIDTH_LABLE)]
        [ValueDropdown("AbilityClassChoice")]
        public string InstanceAbilityClassFullName; 
        
        [VerticalGroup(GRP_BASE_H_LEFT)]
        [Title("Description", bold: false)]
        [HideLabel]
        [MultiLineProperty(5)]
        public string Description;
        
        
        [Title("Gameplay Effect", bold: true)]
        [HorizontalGroup(GRP_BASE_H,PaddingLeft = 0.025f)]
        [VerticalGroup(GRP_BASE_H_RIGHT)]
        [LabelWidth(WIDTH_LABLE)]
        [AssetSelector]
        public GameplayEffectAsset Cost;
        
        [VerticalGroup(GRP_BASE_H_RIGHT)]
        [LabelWidth(WIDTH_LABLE)]
        [AssetSelector]
        public GameplayEffectAsset Cooldown;
        
        [VerticalGroup(GRP_BASE_H_RIGHT)]
        [LabelWidth(WIDTH_LABLE)]
        [LabelText(SdfIconType.ClockFill,Text = "CD Time")]
        [ShowIf("CooldownExist")]
        public float CooldownTime;
        
        [VerticalGroup(GRP_BASE_H_RIGHT)]
        [ListDrawerSettings(Expanded = true)]
        [LabelText("Extras")]
        [Space(10)]
        [LabelWidth(WIDTH_LABLE)]
        [AssetSelector]
        public GameplayEffectAsset[] UsedGameplayEffects = new GameplayEffectAsset[0];

        
        
        [Title("Instant Cue",bold:true)]
        [Space(10)]
        [BoxGroup(GRP_CUE,false)]
        [HorizontalGroup(GRP_CUE_INSTANT)]
        [ListDrawerSettings(Expanded = true,ShowIndexLabels = false,ShowItemCount = false)]
        [AssetSelector]
        public GameplayCueInstant[] instantCues =new GameplayCueInstant[0]; 
        
        [Title("Durational Cue",bold:true)]
        [Space(10)]
        [HorizontalGroup(GRP_CUE_INSTANT)]
        [ListDrawerSettings(Expanded = true,ShowIndexLabels = false,ShowItemCount = false)]
        [AssetSelector]
        public GameplayCueDurational[] durationalCues =new GameplayCueDurational[0]; 
        
        
        
        [Title("Parameter",bold:true)]
        [Space(10)]
        [BoxGroup(GRP_DATA,false)]
        [HorizontalGroup(GRP_DATA_H)]
        [VerticalGroup(GRP_DATA_PARAMETER)]
        [ListDrawerSettings(Expanded = true,ShowIndexLabels = false,ShowItemCount = false)]
        public float[] parameter;
        
        [Title("Summon",bold:true)]
        [Space(10)]
        [BoxGroup(GRP_DATA,false)]
        [HorizontalGroup(GRP_DATA_H)]
        [VerticalGroup(GRP_DATA_SUMMON)]
        [AssetSelector]
        [ListDrawerSettings(Expanded = true,ShowIndexLabels = false,ShowItemCount = false)]
        public GameObject[] Summon;
        
        // Tags
        [Title("Tags",bold:true)]
        [Space(10)]
        [HorizontalGroup(GRP_DATA_H)]
        [VerticalGroup(GRP_DATA_TAG)]
        [ListDrawerSettings(Expanded = true)]
        [ValueDropdown("TagChoices",HideChildProperties = true)]
        public GameplayTag[] AssetTag;
        
        [VerticalGroup(GRP_DATA_TAG)]
        [ListDrawerSettings(Expanded = true)]
        [ValueDropdown("TagChoices",HideChildProperties = true)]
        [Space]
        public GameplayTag[] CancelAbilityTags;
        
        [VerticalGroup(GRP_DATA_TAG)]
        [ListDrawerSettings(Expanded = true)]
        [ValueDropdown("TagChoices",HideChildProperties = true)]
        [Space]
        public GameplayTag[] BlockAbilityTags;
        
        [VerticalGroup(GRP_DATA_TAG)]
        [ListDrawerSettings(Expanded = true)]
        [ValueDropdown("TagChoices",HideChildProperties = true)]
        [Space]
        public GameplayTag[] ActivationOwnedTag;
        
        [VerticalGroup(GRP_DATA_TAG)]
        [ListDrawerSettings(Expanded = true)]
        [ValueDropdown("TagChoices",HideChildProperties = true)]
        [Space]
        public GameplayTag[] ActivationRequiredTags;
        
        [VerticalGroup(GRP_DATA_TAG)]
        [ListDrawerSettings(Expanded = true)]
        [ValueDropdown("TagChoices",HideChildProperties = true)]
        [Space]
        public GameplayTag[] ActivationBlockedTags;
        // public GameplayTag[] SourceRequiredTags;
        // public GameplayTag[] SourceBlockedTags;
        // public GameplayTag[] TargetRequiredTags;
        // public GameplayTag[] TargetBlockedTags;


        
        
        
        private void OnEnable()
        {
            SetAbilityClassChoices();
            SetTagChoices();
        }

        private static void SetAbilityClassChoices()
        {
            var classChoices = new List<string>();
            var assemblies = AppDomain.CurrentDomain.GetAssemblies();
            foreach (var assembly in assemblies)
            {
                try
                {
                    var types = assembly.GetTypes();
                    foreach (var type in types)
                    {
                        if (type.IsSubclassOf(typeof(AbstractAbility)))
                        {
                            classChoices.Add(type.FullName);
                        }
                    }
                }
                catch (ReflectionTypeLoadException)
                {
                }
            }
            var choices = new ValueDropdownList<string>();
            foreach (var tag in classChoices) choices.Add(tag, tag);
            AbilityClassChoice = choices;
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
        
        bool CooldownExist() => Cooldown != null;
    }
}
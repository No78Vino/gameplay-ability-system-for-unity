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
    public abstract class AbilityAsset : ScriptableObject
    {
        private const string GRP_BASE = "Base Info";
        private const string GRP_BASE_H = "Base Info/H";
        protected const string GRP_BASE_H_LEFT = "Base Info/H/Left";
        private const string GRP_BASE_H_RIGHT = "Base Info/H/Right";
        
        private const string GRP_DATA = "DATA";
        private const string GRP_DATA_H = "DATA/H";
        private const string GRP_DATA_PARAMETER = "DATA/H/Parameter";
        private const string GRP_DATA_SUMMON = "DATA/H/Summon";
        private const string GRP_DATA_TAG = "DATA/H/Tags";
        private const string GRP_CUE = "CUE";
        private const string GRP_CUE_INSTANT = "CUE/Instant";
        private const string GRP_CUE_DURATIONAL = "CUE/Durational";
        
        
        protected const int WIDTH_LABLE = 100;
        private const int WIDTH_GRP_BASE_H_LEFT = 350;

        private const string TIP_UNAME = "<size=12><b><color=white><color=orange>Unique Name is very important!</color>" +
                                         "GAS will use the unique name as a UID for the ability." +
                                         "Therefore,you must keep this name unique." +
                                         "Don't worry.When generating the code, the tool will check this.</color></b></size>";


        private static IEnumerable AbilityClassChoice = new ValueDropdownList<string>();
        private static IEnumerable TagChoices = new ValueDropdownList<GameplayTag>();
        
        
        
        public virtual Type AbilityType => typeof(AbstractAbility); 
        
        public string InstanceAbilityClassFullName => AbilityType.FullName;

#if UNITY_EDITOR
        [BoxGroup("Class",false,order:-1)]
        [HideLabel]
        [LabelWidth(WIDTH_LABLE)]
        [ShowInInspector]
        [DisplayAsString(TextAlignment.Left,true)]
        [GUIColor(1,1,1,1)]
        private string AbilityTypeFoShow => $"<size=15><color=white><b>Ability Class : {InstanceAbilityClassFullName}</b></color></size>";
#endif
        
        [BoxGroup(GRP_BASE, false)]
        [Title("Base Information", bold: true)]
        [HorizontalGroup(GRP_BASE_H, Width = WIDTH_GRP_BASE_H_LEFT)]
        [VerticalGroup(GRP_BASE_H_LEFT)]
        [InfoBox(TIP_UNAME)]
        [LabelText("U-Name")]
        [LabelWidth(WIDTH_LABLE)]
        public string UniqueName;
        
        [VerticalGroup(GRP_BASE_H_LEFT)]
        [Title("Description", bold: false)]
        [HideLabel]
        [MultiLineProperty(5)]
        public string Description;
        
        [Space]
        [Title("Gameplay Effect", bold: true)]
        [VerticalGroup(GRP_BASE_H_LEFT)]
        [LabelWidth(WIDTH_LABLE)]
        [AssetSelector]
        public GameplayEffectAsset Cost;
        
        [VerticalGroup(GRP_BASE_H_LEFT)]
        [LabelWidth(WIDTH_LABLE)]
        [AssetSelector]
        public GameplayEffectAsset Cooldown;
        
        [VerticalGroup(GRP_BASE_H_LEFT)]
        [LabelWidth(WIDTH_LABLE)]
        [LabelText(SdfIconType.ClockFill,Text = "CD Time")]
        public float CooldownTime;
        
        // Tags
        [Title("Tags",bold:true)]
        [HorizontalGroup(GRP_BASE_H,PaddingLeft = 0.025f)]
        [VerticalGroup(GRP_BASE_H_RIGHT)]
        [ListDrawerSettings(Expanded = true)]
        [ValueDropdown("TagChoices",HideChildProperties = true)]
        public GameplayTag[] AssetTag;
        
        [VerticalGroup(GRP_BASE_H_RIGHT)]
        [ListDrawerSettings(Expanded = true)]
        [ValueDropdown("TagChoices",HideChildProperties = true)]
        [Space]
        public GameplayTag[] CancelAbilityTags;
        
        [VerticalGroup(GRP_BASE_H_RIGHT)]
        [ListDrawerSettings(Expanded = true)]
        [ValueDropdown("TagChoices",HideChildProperties = true)]
        [Space]
        public GameplayTag[] BlockAbilityTags;
        
        [VerticalGroup(GRP_BASE_H_RIGHT)]
        [ListDrawerSettings(Expanded = true)]
        [ValueDropdown("TagChoices",HideChildProperties = true)]
        [Space]
        public GameplayTag[] ActivationOwnedTag;
        
        [VerticalGroup(GRP_BASE_H_RIGHT)]
        [ListDrawerSettings(Expanded = true)]
        [ValueDropdown("TagChoices",HideChildProperties = true)]
        [Space]
        public GameplayTag[] ActivationRequiredTags;
        
        [VerticalGroup(GRP_BASE_H_RIGHT)]
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
            SetTagChoices();
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
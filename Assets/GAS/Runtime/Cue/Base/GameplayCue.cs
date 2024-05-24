using Sirenix.OdinInspector;
using UnityEngine;

namespace GAS.Runtime
{
    public abstract class GameplayCue : ScriptableObject
    {
        private const string TopGroup = "TopGroup";
        private const string TopGroup_H = "TopGroup/H";
        private const string TopGroup_H_Left = "TopGroup/H/Left";
        private const string TopGroup_H_Right = "TopGroup/H/Right";

        [Title("基本信息")]
        [BoxGroup(TopGroup, false)]
        [HorizontalGroup(TopGroup_H, Width = 250)]
        [VerticalGroup(TopGroup_H_Left)]
        [InfoBox("基本信息仅仅是为了备注以方便理解, 不会对游戏产生实质性影响。", SdfIconType.None)]
        public string Name = "Unnamed";

        [VerticalGroup(TopGroup_H_Left)]
        [Title("Description", bold: false)]
        [HideLabel]
        [MultiLineProperty(5)]
        public string Description = "He is very lazy and left nothing behind.";

        [Title("标签")]
        [HorizontalGroup(TopGroup_H, PaddingLeft = 0.025f)]
        [VerticalGroup(TopGroup_H_Right)]
        [ListDrawerSettings(ShowFoldout = true)]
        [ValueDropdown("@ValueDropdownHelper.GameplayTagChoices", IsUniqueList = true, HideChildProperties = true)]
        public GameplayTag[] RequiredTags;

        [VerticalGroup(TopGroup_H_Right)]
        [ListDrawerSettings(ShowFoldout = true)]
        [ValueDropdown("@ValueDropdownHelper.GameplayTagChoices", IsUniqueList = true, HideChildProperties = true)]
        public GameplayTag[] ImmunityTags;

        public virtual bool Triggerable(AbilitySystemComponent owner)
        {
            if (owner == null) return false;
            // 持有【所有】RequiredTags才可触发
            if (!owner.HasAllTags(new GameplayTagSet(RequiredTags)))
                return false;

            // 持有【任意】ImmunityTags不可触发
            if (owner.HasAnyTags(new GameplayTagSet(ImmunityTags)))
                return false;

            return true;
        }
    }

    public abstract class GameplayCue<T> : GameplayCue where T : GameplayCueSpec
    {
        public abstract T CreateSpec(GameplayCueParameters parameters);
    }
}
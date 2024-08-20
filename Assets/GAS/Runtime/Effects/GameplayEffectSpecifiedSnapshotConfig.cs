using System;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;

namespace GAS.Runtime
{
    [Serializable]
    public struct GameplayEffectSpecifiedSnapshotConfig
    {
        public enum ESnapshotTarget
        {
            [LabelText("施法者", SdfIconType.Magic)]
            Source,

            [LabelText("持有者", SdfIconType.Person)]
            Target
        }

        private const int LABEL_WIDTH = 70;

        [LabelText("目标", SdfIconType.PersonBadge)]
        [LabelWidth(LABEL_WIDTH)]
        [EnumToggleButtons]
        public ESnapshotTarget SnapshotTarget;

        [LabelText("属性", SdfIconType.Fingerprint)]
        [LabelWidth(LABEL_WIDTH)]
        [OnValueChanged("OnAttributeChanged")]
        [ValueDropdown("@ValueDropdownHelper.AttributeChoices", IsUniqueList = true)]
        [ValidateInput("@AttributeValidator.IsValidAttributeName($value)", "属性名无效")]
        public string AttributeName;

        [HideInInspector]
        public string AttributeSetName;

        [HideInInspector]
        public string AttributeShortName;

        public GameplayEffectSpecifiedSnapshotConfig(ESnapshotTarget snapshotTarget, string attributeName)
        {
            SnapshotTarget = snapshotTarget;
            AttributeName = attributeName;
            AttributeSetName = string.Empty;
            AttributeShortName = string.Empty;

            OnAttributeChanged();
        }

        private void OnAttributeChanged()
        {
            var split = AttributeName.Split('.');
            AttributeSetName = split[0];
            AttributeShortName = split[1];
        }
    }
}
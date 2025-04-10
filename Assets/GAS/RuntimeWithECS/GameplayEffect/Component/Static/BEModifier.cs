using System;
using System.Collections.Generic;
using GAS.Runtime;
using GAS.RuntimeDataHelper.Helper;
using GAS.RuntimeWithECS.Modifier;
using Sirenix.OdinInspector;
using Unity.Collections;
using Unity.Entities;

namespace GAS.RuntimeWithECS.GameplayEffect.Component
{
    public struct BEModifier : IBufferElementData
    {
        public int AttrSetCode;
        public int AttrCode;
        public GEOperation Operation;
        public float Magnitude;
        public MMCSetting MMC;
    }

    public sealed class ConfModifiers : GameplayEffectComponentConfig
    {
        public ModifierSetting[] modifierSettings;

        public override void LoadToGameplayEffectEntity(Entity ge)
        {
            if (!_entityManager.HasBuffer<BEModifier>(ge))
                _entityManager.AddBuffer<BEModifier>(ge);

            var buffer = _entityManager.GetBuffer<BEModifier>(ge);
            foreach (var modifierSetting in modifierSettings)
            {
                var stringParams = modifierSetting.MMC.stringParams == null
                    ? Array.Empty<FixedString32Bytes>()
                    : new FixedString32Bytes[modifierSetting.MMC.stringParams.Length];

                if (modifierSetting.MMC.stringParams != null)
                    for (var i = 0; i < modifierSetting.MMC.stringParams.Length; i++)
                        stringParams[i] = modifierSetting.MMC.stringParams[i];

                var floatParams = modifierSetting.MMC.floatParams ?? Array.Empty<float>();
                var intParams = modifierSetting.MMC.intParams ?? Array.Empty<int>();
                buffer.Add(new BEModifier
                {
                    AttrSetCode = modifierSetting.AttrSetCode,
                    AttrCode = modifierSetting.AttrCode,
                    Operation = modifierSetting.Operation,
                    Magnitude = modifierSetting.Magnitude,
                    MMC = new MMCSetting
                    {
                        TypeCode = modifierSetting.MMC.TypeCode,
                        floatParams = new NativeArray<float>(floatParams, Allocator.Persistent),
                        intParams = new NativeArray<int>(intParams, Allocator.Persistent),
                        stringParams = new NativeArray<FixedString32Bytes>(stringParams, Allocator.Persistent)
                    }
                });
            }
        }
    }

    [Serializable]
    public struct ModifierSetting
    {
        [LabelText("生效的属性集")] [ValueDropdown("@EditAttributeHelper.AttributeSetChoices", IsUniqueList = true)]
        public int AttrSetCode;

        [LabelText("生效的属性")] [ValueDropdown(nameof(AttributeChoice), IsUniqueList = true)]
        public int AttrCode;

        [LabelText("操作类型")] public GEOperation Operation;

        [LabelText("通用基础模值")] public float Magnitude;

        [HideLabel]
        public MMCSettingConfig MMC;

#if UNITY_EDITOR
        private IEnumerable<ValueDropdownItem> AttributeChoice()
        {
            return EditAttributeHelper.GetAttributeChoiceByAttrSet(AttrSetCode);
        }
#endif
    }
}
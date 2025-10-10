using System;
using System.Collections.Generic;
using GAS.Runtime;
using GAS.RuntimeDataHelper.GameplayEffect;
using GAS.Editor;
using GAS.RuntimeWithECS.GameplayEffect;
using GAS.RuntimeWithECS.Modifier;
using Sirenix.OdinInspector;
using Unity.Collections;
using Unity.Entities;

namespace GAS.Runtime
{

    public class MCModifiers:IComponentData
    {
        public EffectModifier[] Modifiers;
        
        public MCModifiers(EffectModifier[] modifiers)
        {
            Modifiers = modifiers;
        }
        
        public MCModifiers()
        {
        }
    }

    public struct EffectModifier
    {
        public int AttrSetCode;
        public int AttrCode;
        public GEOperation Operation;
        public float Magnitude;
        public ModMagnitudeCalculationBase MMC;
    }

    public sealed class MCConfModifiers : GameplayEffectComponentConfig
    {
        public ModifierSetting[] modifierSettings;

        public override void LoadToGameplayEffectEntity(Entity ge)
        {
            EntityHelper.AddManagedComponent<MCModifiers>(ge);

            EffectModifier[] effectModifiers = new EffectModifier[modifierSettings.Length];
            for (var i = 0; i < modifierSettings.Length; i++)
            {
                var modifierSetting = modifierSettings[i];
                effectModifiers[i] = new EffectModifier
                {
                    AttrSetCode = modifierSetting.AttrSetCode,
                    AttrCode = modifierSetting.AttrCode,
                    Operation = modifierSetting.Operation,
                    Magnitude = modifierSetting.Magnitude,
                    MMC = modifierSetting.MMC.CreateMmc()
                };
            }
            MCModifiers mcModifiers = new MCModifiers(effectModifiers);
            EntityHelper.SetManagedComponent(ge, mcModifiers);
        }

        public void TriggerOnValueChanged()
        {
            foreach (var mod in modifierSettings)
                mod.OnValueChanged();
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
        public void OnValueChanged() => MMC?.TriggerOnValueChanged();
        
        private IEnumerable<ValueDropdownItem> AttributeChoice()
        {
            return null;
            //return EditAttributeHelper.GetAttributeChoiceByAttrSet(AttrSetCode);
        }
#endif
    }
}
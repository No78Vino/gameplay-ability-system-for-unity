using System;
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
        
        public float Apply(MmcContext mmcContext, float currentValue)
        {
            var magnitude = MMC.CalculateMagnitude(mmcContext, Magnitude);  
            return Operation switch  
            {  
                GEOperation.Add      => currentValue + magnitude,  
                GEOperation.Minus    => currentValue - magnitude,  
                GEOperation.Multiply => currentValue * magnitude,  
                GEOperation.Divide   => currentValue / magnitude,  
                GEOperation.Override => magnitude,  
                _                    => throw new ArgumentOutOfRangeException()  
            };  
        }
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
    }

    [Serializable]
    public struct ModifierSetting
    {
        public int AttrSetCode;
        public int AttrCode; 
        public GEOperation Operation; 
        public float Magnitude;
        public MMCConfig MMC;
    }
}
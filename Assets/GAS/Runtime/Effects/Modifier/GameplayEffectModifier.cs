using System.Collections.Generic;
using GAS.Runtime.Attribute;
using GAS.Runtime.Tags;

namespace GAS.Runtime.Effects.Modifier
{
    public enum GEOperation
    {
        Add,
        Multiply,
        Override
    }

    public enum GEModifierType
    {
        ScalableFloat,
        AttributeBased, 
        CustomCalculationClass,
        SetByCaller
    }
    
    public enum GECalculationType
    {
        Single,
        Combine,
    }

    public enum GEAttributeCaptureType
    {
        SnapShot,
        Track,
    }

    public struct GameplayEffectModifier
    {
        public readonly string AttributeName;
        public readonly string AttributeSetName;
        public readonly string AttributeShortName;
        public readonly float Coefficient;
        
        public readonly GEOperation Operation;
        public readonly GEModifierType ModifierType;
        public readonly GECalculationType CalculationType;
        public readonly GEAttributeCaptureType CaptureType;
        public readonly ModifierMagnitudeCalculation ModifierMagnitude;
        
        public GameplayEffectModifier(
            string attributeName, 
            float coefficient,
            GEOperation operation,
            GEModifierType modifierType, 
            GECalculationType calculationType,
            GEAttributeCaptureType captureType)
        {
            AttributeName = attributeName;
            var splits = attributeName.Split('.');
            AttributeSetName = splits[0];
            AttributeShortName = splits[1];
            
            Coefficient = coefficient;
                
            Operation = operation;
            ModifierType = modifierType;
            CalculationType = calculationType;
            CaptureType = captureType;
            
            // TODO
            ModifierMagnitude = null;
        }
        
        public GameplayEffectModifier(GameplayEffectModifier modifier)
        {
            AttributeName = modifier.AttributeName;
            AttributeSetName = modifier.AttributeSetName;
            AttributeShortName = modifier.AttributeShortName;
            Coefficient = modifier.Coefficient;
            Operation = modifier.Operation;
            ModifierType = modifier.ModifierType;
            CalculationType = modifier.CalculationType;
            CaptureType = modifier.CaptureType;
            ModifierMagnitude = modifier.ModifierMagnitude;
        }
        
        // public float Value()
        // {
        //     if (ModifierType == GEModifierType.ScalableFloat)
        //     {
        //         return ModifierMagnitude.CalculateMagnitude(s);
        //     }
        //
        //     if (ModifierType == GEModifierType.AttributeBased)
        //     {
        //         
        //     }
        //     
        //     if(ModifierType == GEModifierType.SetByCaller)
        //     {
        //         
        //     }
        //     
        //     if( ModifierType == GEModifierType.CustomCalculationClass)
        //     {
        //         
        //     }
        //
        //     return 1;
        // }
    }
}
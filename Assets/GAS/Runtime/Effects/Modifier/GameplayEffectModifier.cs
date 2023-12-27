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

    public struct GameplayEffectModifier
    {
        public readonly string AttributeName;
        public readonly string AttributeSetName;
        public readonly string AttributeShortName;
        public readonly float ModiferMagnitude;
        
        public readonly GEOperation Operation;
        public readonly ModifierMagnitudeCalculation MMC;
        
        // TODO
        // public readonly GameplayTagSet SourceTag;
        
        // TODO
        // public readonly GameplayTagSet TargetTag;
        
        public GameplayEffectModifier(
            string attributeName, 
            float modiferMagnitude,
            GEOperation operation)
        {
            AttributeName = attributeName;
            var splits = attributeName.Split('.');
            AttributeSetName = splits[0];
            AttributeShortName = splits[1];
            ModiferMagnitude = modiferMagnitude;
            Operation = operation;
            
            // TODO
            MMC = null;
        }
        
        public GameplayEffectModifier(GameplayEffectModifier modifier)
        {
            AttributeName = modifier.AttributeName;
            AttributeSetName = modifier.AttributeSetName;
            AttributeShortName = modifier.AttributeShortName;
            ModiferMagnitude = modifier.ModiferMagnitude;
            Operation = modifier.Operation;
            MMC = modifier.MMC;
        }
    }
}
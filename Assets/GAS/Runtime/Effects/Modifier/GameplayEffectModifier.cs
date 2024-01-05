using System;
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

    [Serializable]
    public struct GameplayEffectModifier
    {
        public string AttributeName;
        public string AttributeSetName;
        public string AttributeShortName;
        public float ModiferMagnitude;
        
        public  GEOperation Operation;
        public  ModifierMagnitudeCalculation MMC;
        
        // TODO
        // public readonly GameplayTagSet SourceTag;
        
        // TODO
        // public readonly GameplayTagSet TargetTag;
        
        public GameplayEffectModifier(
            string attributeName, 
            float modiferMagnitude,
            GEOperation operation,
            ModifierMagnitudeCalculation mmc)
        {
            AttributeName = attributeName;
            var splits = attributeName.Split('.');
            AttributeSetName = splits[0];
            AttributeShortName = splits[1];
            ModiferMagnitude = modiferMagnitude;
            Operation = operation;
            MMC = mmc;
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

        public void SetModiferMagnitude(float value)
        {
            if (MMC is SetByCallerModCalculation)
            {
                ModiferMagnitude = value;
            }
            else
            {
                #if UNITY_EDITOR
                UnityEngine.Debug.LogError("[EX] this MMC is not SetByCallerModCalculation, can't set ModiferMagnitude!");
                #else
                UnityEngine.Debug.LogWarning("[EX] this MMC is not SetByCallerModCalculation, can't set ModiferMagnitude!");
                #endif
            }
        }
    }
}
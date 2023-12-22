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

    public enum GEAttributeReplicationType
    {
        SnapShot,
        Track,
    }

    public struct GameplayEffectModifier
    {
        public AttributeBase Attribute { get; private set; }
        public string AttributeName => Attribute.Name;
        
        private float _value;
        
        public GEOperation Operation { get; private set; }
        public GEModifierType ModifierType { get; private set; }
        public GECalculationType CalculationType { get; private set; }
        public GEAttributeReplicationType ReplicationType { get; private set; }
        public float Value => ReplicationType == GEAttributeReplicationType.SnapShot ? _value : Attribute.CurrentValue;
        
        public GameplayEffectModifier(
            AttributeBase attribute, 
            GEOperation operation,
            GEModifierType modifierType, 
            GECalculationType calculationType,
            GEAttributeReplicationType replicationType)
        {
            Attribute = attribute;
            _value = Attribute.CurrentValue;
            Operation = operation;
            ModifierType = modifierType;
            CalculationType = calculationType;
            ReplicationType = replicationType;
        }
        
        public GameplayEffectModifier(GameplayEffectModifier modifier)
        {
            Attribute = modifier.Attribute;
            _value = modifier.Value;
            Operation = modifier.Operation;
            ModifierType = modifier.ModifierType;
            CalculationType = modifier.CalculationType;
            ReplicationType = modifier.ReplicationType;
        }
        
        public bool CombinableWith(GameplayEffectModifier other)
        {
            return AttributeName == other.AttributeName && Operation == other.Operation && CalculationType == other.CalculationType;
        }
        
        // public bool CombineWith(GameplayEffectModifier other,out GameplayEffectModifier result)
        // {
        //     if (!CombinableWith(other))
        //     {
        //         result = this;
        //         return false;
        //     }
        //     
        //     switch (Operation)
        //     {
        //         case GEOperation.Add:
        //             Value += other.Value;
        //             break;
        //         case GEOperation.Multiply:
        //             Value *= other.Value;
        //             break;
        //         case GEOperation.Override:
        //             Value = other.Value;
        //             break;
        //     }
        //     
        //     result = this;
        //     return true;
        // }
    }
}
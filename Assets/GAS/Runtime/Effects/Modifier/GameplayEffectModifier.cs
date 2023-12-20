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

    public enum AttributeTrackType
    {
        
    }

    public struct GameplayEffectModifier
    {
        public AttributeBase Attribute { get; private set; }
        public string AttributeName { get; private set; }
        public float Value { get; private set; }
        public GEOperation Operation { get; private set; }
        public GEModifierType Type { get; private set; }
        public GECalculationType CalculationType { get; private set; }

        public List<GameplayTag> Tags{ get; private set; }
        
        public GameplayEffectModifier(string attributeName, float value, GEOperation operation,
            GEModifierType type, GECalculationType calculationType)
        {
            AttributeName = attributeName;
            Value = value;
            Operation = operation;
            Type = type;
            CalculationType = calculationType;
            Tags = new List<GameplayTag>();
        }
        
        public GameplayEffectModifier(GameplayEffectModifier modifier)
        {
            AttributeName = modifier.AttributeName;
            Value = modifier.Value;
            Operation = modifier.Operation;
            Type = modifier.Type;
            CalculationType = modifier.CalculationType;
            Tags = modifier.Tags;
        }
        
        public bool CombinableWith(GameplayEffectModifier other)
        {
            return AttributeName == other.AttributeName && Operation == other.Operation && CalculationType == other.CalculationType;
        }
        
        public bool CombineWith(GameplayEffectModifier other,out GameplayEffectModifier result)
        {
            if (!CombinableWith(other))
            {
                result = this;
                return false;
            }
            
            switch (Operation)
            {
                case GEOperation.Add:
                    Value += other.Value;
                    break;
                case GEOperation.Multiply:
                    Value *= other.Value;
                    break;
                case GEOperation.Override:
                    Value = other.Value;
                    break;
            }
            
            result = this;
            return true;
        }
    }
}
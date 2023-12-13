using GAS.Runtime.Attribute.Value;

namespace GAS.Runtime.Attribute.Modifier
{
    public enum AttributeModifierType
    {
        None,
        AddBase,
        AddAbsolute,
        MultiplyBase,
        MultiplyBaseWithAdditive,
        Override
    }
    
    public enum AttributeCombineTiming
    {
        Sequence,
        Pre,
        Post,
    }
    
    public enum AttributeCombineType
    {
        Single,
        Combine,
    }
    
    public struct AttributeModifier
    {
        public AttributeModifierType Type { get;private set; }
        public AttributeCombineTiming Timing { get;private set; }
        public AttributeCombineType CombineType { get;private set; }
        
        public AttributeModifier(AttributeModifierType type, AttributeCombineTiming timing, AttributeCombineType combineType)
        {
            Type = type;
            Timing = timing;
            CombineType = combineType;
        }
    }
}
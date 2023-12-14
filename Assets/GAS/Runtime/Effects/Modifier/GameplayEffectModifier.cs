using GAS.Runtime.Attribute;

namespace GAS.Runtime.Effects.Modifier
{
    public enum GEModifierType
    {
        None,
        AddBase,
        AddAbsolute,
        MultiplyBase,
        MultiplyBaseWithAdditive,
        Override
    }

    public enum GEAttributeCombineTiming
    {
        Sequence,
        Pre,
        Post
    }

    public enum GEAttributeCombineType
    {
        Single,
        Combine
    }

    public struct GameplayEffectModifier
    {
        public AttributeBase Attribute { get; private set; }
        public float Value { get; private set; }
        public GEModifierType Type { get; private set; }
        public GEAttributeCombineTiming Timing { get; private set; }
        public GEAttributeCombineType CombineType { get; private set; }


        public GameplayEffectModifier(AttributeBase attribute, float value, GEModifierType type,
            GEAttributeCombineTiming timing, GEAttributeCombineType combineType)
        {
            Attribute = attribute;
            Value = value;
            Type = type;
            Timing = timing;
            CombineType = combineType;
        }
    }
}
namespace GAS.Runtime.Effects.Modifier
{
    public class AttributeBasedModCalculation:ModifierMagnitudeCalculation
    {
        public AttributeBasedModCalculation(GameplayEffectSpec spec) : base(spec)
        {
        }

        public override float CalculateMagnitude(float modifierValue)
        {
            // TODO
            return 1;
        }
        
        protected virtual float PreModCalculate (float modValue)
        {
            return modValue;
        }
        
        protected virtual float PostModCalculate (float modValue)
        {
            return modValue;
        }
    }
}
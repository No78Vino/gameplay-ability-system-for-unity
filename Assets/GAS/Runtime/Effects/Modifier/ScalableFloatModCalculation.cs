namespace GAS.Runtime.Effects.Modifier
{
    public class ScalableFloatModCalculation:ModifierMagnitudeCalculation
    {
        
        public ScalableFloatModCalculation(GameplayEffectSpec spec) : base(spec)
        {
        }

        public override float CalculateMagnitude(float modifierValue)
        {
            // TODO
            return 1;
        }
    }
}
namespace GAS.Runtime.Effects.Modifier
{
    public class ScalableFloatModCalculation:ModifierMagnitudeCalculation
    {
        private float _k;
        private float _b;
        
        public ScalableFloatModCalculation(GameplayEffectSpec spec,float k,float b) : base(spec)
        {
            _k = k;
            _b = b;
        }

        public override float CalculateMagnitude(params float[] input)
        {
            return input[0] * _k + _b;
        }
    }
}
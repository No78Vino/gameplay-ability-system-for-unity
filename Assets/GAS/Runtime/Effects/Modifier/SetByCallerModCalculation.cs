namespace GAS.Runtime.Effects.Modifier
{
    public class SetByCallerModCalculation:ModifierMagnitudeCalculation
    {
        public SetByCallerModCalculation(GameplayEffectSpec spec) : base(spec)
        {
        }

        public override float CalculateMagnitude(params float[] input)
        {
            return input[0];
        }
    }
}
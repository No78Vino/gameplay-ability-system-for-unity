namespace GAS.Runtime.Effects.Modifier
{
    public class SetByCallerModCalculation:ModifierMagnitudeCalculation
    {
        public SetByCallerModCalculation(GameplayEffectSpec spec) : base(spec)
        {
        }

        public override float CalculateMagnitude(float modifierValue)
        {
            // TODO
            return 1;
        }
    }
}
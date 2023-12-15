namespace GAS.Runtime.Effects.Modifier
{
    public abstract class AbstractModifierMagnitudeCalculation
    {
        protected GameplayEffectSpec _spec;
        
        public AbstractModifierMagnitudeCalculation(GameplayEffectSpec spec)
        {
            _spec = spec;
        }

        public abstract float? CalculateMagnitude(GameplayEffectSpec spec, float multiplier);
    }
}
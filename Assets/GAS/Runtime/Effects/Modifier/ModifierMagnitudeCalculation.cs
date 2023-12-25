using GAS.Runtime.Tags;

namespace GAS.Runtime.Effects.Modifier
{
    public abstract class ModifierMagnitudeCalculation
    {
        protected GameplayEffectSpec _spec;
        
        public ModifierMagnitudeCalculation(GameplayEffectSpec spec)
        {
            _spec = spec;
        }

        public abstract float CalculateMagnitude(float modifierValue);
        
        float GetSetByCallerMagnitudeByName(string MagnitudeName)
        {
            // TODO
            return 0;
        }
        
        float GetSetByCallerMagnitudeByTag(GameplayTagSet tagSet)
        {
            // TODO
            return 0;
        }
    }
}
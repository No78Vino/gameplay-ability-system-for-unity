using GAS.Runtime.Tags;
using UnityEngine;

namespace GAS.Runtime.Effects.Modifier
{
    public abstract class ModifierMagnitudeCalculation:ScriptableObject
    {
        protected GameplayEffectSpec _spec;

        protected abstract float CalculateMagnitude(params float[] modifierValue);
        
        public virtual float CalculateMagnitude(GameplayEffectSpec spec, params float[] modifierValue)
        {
            _spec = spec;
            return CalculateMagnitude(modifierValue);
        }
    }
}
using GAS.Runtime.Tags;
using UnityEngine;

namespace GAS.Runtime.Effects.Modifier
{
    public abstract class ModifierMagnitudeCalculation:ScriptableObject
    {
        protected GameplayEffectSpec _spec;
        
        public void Init(GameplayEffectSpec spec)
        {
            _spec = spec;
        }

        public abstract float CalculateMagnitude(params float[] modifierValue);
    }
}
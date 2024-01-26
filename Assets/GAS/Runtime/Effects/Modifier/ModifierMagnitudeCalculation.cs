using GAS.Runtime.Tags;
using UnityEngine;

namespace GAS.Runtime.Effects.Modifier
{
    public abstract class ModifierMagnitudeCalculation:ScriptableObject
    {
        public abstract float CalculateMagnitude(GameplayEffectSpec spec,float modifierMagnitude);
    }
}
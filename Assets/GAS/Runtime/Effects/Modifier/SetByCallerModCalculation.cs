using UnityEngine;

namespace GAS.Runtime.Effects.Modifier
{
    [CreateAssetMenu(fileName = "SetByCallerModCalculation", menuName = "GAS/MMC/SetByCallerModCalculation")]
    public class SetByCallerModCalculation:ModifierMagnitudeCalculation
    {
        public override float CalculateMagnitude(GameplayEffectSpec spec,float input)
        {
            return input;
        }
    }
}
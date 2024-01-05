using UnityEngine;

namespace GAS.Runtime.Effects.Modifier
{
    [CreateAssetMenu(fileName = "SetByCallerModCalculation", menuName = "GAS/MMC/SetByCallerModCalculation")]
    public class SetByCallerModCalculation:ModifierMagnitudeCalculation
    {
        protected override float CalculateMagnitude(params float[] input)
        {
            return input[0];
        }
    }
}
using UnityEngine;

namespace GAS.Runtime.Effects.Modifier
{
    [CreateAssetMenu(fileName = "SetByCallerModCalculation", menuName = "GAS/MMC/SetByCallerModCalculation")]
    public class SetByCallerModCalculation:ModifierMagnitudeCalculation
    {
        public override float CalculateMagnitude(params float[] input)
        {
            return input[0];
        }
    }
}
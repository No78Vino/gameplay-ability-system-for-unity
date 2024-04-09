using UnityEngine;

namespace GAS.Runtime
{
    [CreateAssetMenu(fileName = "ScalableFloatModCalculation", menuName = "GAS/MMC/ScalableFloatModCalculation")]
    public class ScalableFloatModCalculation : ModifierMagnitudeCalculation
    {
        [SerializeField] private float k = 1f;
        [SerializeField] private float b = 0f;

        public override float CalculateMagnitude(GameplayEffectSpec spec, float input)
        {
            return input * k + b;
        }
    }
}
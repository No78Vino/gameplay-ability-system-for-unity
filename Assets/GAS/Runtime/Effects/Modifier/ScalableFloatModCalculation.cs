using Sirenix.OdinInspector;
using UnityEngine;

namespace GAS.Runtime
{
    [CreateAssetMenu(fileName = "ScalableFloatModCalculation", menuName = "GAS/MMC/ScalableFloatModCalculation")]
    public class ScalableFloatModCalculation : ModifierMagnitudeCalculation
    {
        private const string Desc = "计算公式：ModifierMagnitude * k + b";

        private const string Detail =
            "ScalableFloatModCalculation：可缩放浮点数计算\n该类型是根据Magnitude计算Modifier模值的，计算公式为：ModifierMagnitude * k + b 实际上就是一个线性函数，k和b为可编辑参数，可以在编辑器中设置。";

        [DetailedInfoBox(Desc, Detail, InfoMessageType.Info)] [SerializeField]
        private float k = 1f;

        [SerializeField] private float b = 0f;

        public override float CalculateMagnitude(GameplayEffectSpec spec, float input)
        {
            return input * k + b;
        }
    }
}
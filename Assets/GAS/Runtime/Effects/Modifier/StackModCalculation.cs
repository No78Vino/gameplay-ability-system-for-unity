using Sirenix.OdinInspector;
using UnityEngine;

namespace GAS.Runtime
{
    [CreateAssetMenu( fileName = "StackModCalculation", menuName = "GAS/MMC/StackModCalculation" )]
    public class StackModCalculation:ModifierMagnitudeCalculation
    {
        [InfoBox("计算逻辑与ScalableFloatModCalculation一致, 公式：(StackCount) * k + b")]
        [TabGroup("Default", "StackModCalculation")]
        [LabelText("系数(k)")]
        public float k = 1;

        [TabGroup("Default", "StackModCalculation")]
        [LabelText("常量(b)")]
        public float b = 0;
        
        public override float CalculateMagnitude(GameplayEffectSpec spec, float modifierMagnitude)
        {
            if (spec.Stacking.stackingType == StackingType.None) return 0;
            
            var stackCount = spec.StackCount;
            return stackCount * k + b;
        }
    }
}
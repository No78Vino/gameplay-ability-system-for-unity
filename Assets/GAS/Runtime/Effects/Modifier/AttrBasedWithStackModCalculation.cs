using Sirenix.OdinInspector;
using UnityEngine;

namespace GAS.Runtime
{
    /// <summary>
    ///  基于属性混合GE堆栈的MMC
    /// </summary>
    [CreateAssetMenu(fileName = "AttrBasedWithStackModCalculation", menuName = "GAS/MMC/AttrBasedWithStackModCalculation")]
    public class AttrBasedWithStackModCalculation:AttributeBasedModCalculation
    {
        public enum StackMagnitudeOperation
        {
            Add,
            Multiply
        }
        
        [InfoBox(" 公式：StackCount * sK + sB")]
        [TabGroup("Default", "AttributeBasedModCalculation")]
        [Title("堆叠幅值计算")]
        [LabelText("系数(sK)")]
        public float sK = 1;

        [TabGroup("Default", "AttributeBasedModCalculation")]
        [LabelText("常量(sB)")]
        public float sB = 0;

        [TabGroup("Default", "AttributeBasedModCalculation")]
        [Title("最终结果")]
        [InfoBox(" 最终公式： \n" +
                 "Add:(AttributeValue * k + b)+(StackCount * sK + sB); \n" +
                 "Multiply:(AttributeValue * k + b)*(StackCount * sK + sB)")]
        [LabelText("Stack幅值与Attr幅值计算方式")]
        public StackMagnitudeOperation stackMagnitudeOperation;

        [TabGroup("Default", "AttributeBasedModCalculation")]
        [LabelText("最终公式")]
        [ShowInInspector]
        [DisplayAsString(TextAlignment.Left, true)]
        public string FinalFormulae
        {
            get
            {
                var formulae = stackMagnitudeOperation switch
                {
                    StackMagnitudeOperation.Add => $"({attributeName} * {k} + {b}) + (StackCount * {sK} + {sB})",
                    StackMagnitudeOperation.Multiply => $"({attributeName} * {k} + {b}) * (StackCount * {sK} + {sB})",
                    _ => ""
                };

                return $"<size=15><b><color=green>{formulae}</color></b></size>";
            }
        }
        
        public override float CalculateMagnitude(GameplayEffectSpec spec, float modifierMagnitude)
        {
            var attrMagnitude = base.CalculateMagnitude(spec, modifierMagnitude);
            
            if (spec.Stacking.stackingType == StackingType.None) return attrMagnitude;
            
            var stackMagnitude = spec.StackCount * sK + sB;

            return stackMagnitudeOperation switch
            {
                StackMagnitudeOperation.Add => attrMagnitude + stackMagnitude,
                StackMagnitudeOperation.Multiply => attrMagnitude * stackMagnitude,
                _ => attrMagnitude + stackMagnitude
            };
        }
        
    }
}
using System;
using Sirenix.OdinInspector;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;

namespace GAS.RuntimeWithECS.Modifier.CommonUsage
{
    [Serializable]
    public class MMCScalableFloat:ModMagnitudeCalculation
    {
        private const string Desc = "计算公式：ModifierMagnitude * k + b";

        private const string Detail =
            "ScalableFloatModCalculation：可缩放浮点数计算\n该类型是根据Magnitude计算Modifier模值的，计算公式为：ModifierMagnitude * k + b 实际上就是一个线性函数，k和b为可编辑参数，可以在编辑器中设置。";

        [DetailedInfoBox(Desc, Detail, InfoMessageType.Info), SerializeField]
        private float k = 1f;

        [SerializeField] private float b;
        
        public override float CalculateMagnitude(Entity specEntity, float magnitude)
        {
            return magnitude * k + b;
        }

        public override void InitParameters(NativeArray<float> floatParams, NativeArray<int> intParams, NativeArray<FixedString32Bytes> stringParams)
        {
            k = floatParams[0];
            b = floatParams[1];
        }
    }
}
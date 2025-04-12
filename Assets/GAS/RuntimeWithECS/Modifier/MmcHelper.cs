using System;
using System.Collections.Generic;
using GAS.ECS_TEST_RUNTIME_GEN_LIB;
using GAS.Runtime;
using GAS.RuntimeWithECS.GameplayEffect.Component;
using GAS.RuntimeWithECS.Modifier.CommonUsage;
using Unity.Entities;
using UnityEngine;

namespace GAS.RuntimeWithECS.Modifier
{
    public static class MmcHelper
    {
        private static Dictionary<int, ModMagnitudeCalculationBase> _magnitudeCalculations;

        public static void Init()
        {
            _magnitudeCalculations = new Dictionary<int, ModMagnitudeCalculationBase>();
            // TODO :初始化项目内所有类型MMC实例
            _magnitudeCalculations.Add(MMCTypeToCode.Map[typeof(MMCScalableFloat)],new MMCScalableFloat());
        }
        
        public static float Calculate(Entity ge, EffectModifier modifier, float sourceValue)
        {
            var result = sourceValue;
            var magnitude = modifier.MMC.CalculateMagnitude(ge, modifier.Magnitude);
            switch (modifier.Operation)
            {
                case GEOperation.Add:
                    result += magnitude;
                    break;
                case GEOperation.Minus:
                    result -= magnitude;
                    break;
                case GEOperation.Multiply:
                    result *= magnitude;
                    break;
                case GEOperation.Divide:
                    result /= magnitude;
                    break;
                case GEOperation.Override:
                    result = magnitude;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            return result;
        }
    }
}
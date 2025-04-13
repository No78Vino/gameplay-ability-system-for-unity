using System;
using GAS.Runtime;
using GAS.RuntimeWithECS.GameplayEffect.Component;
using Unity.Entities;

namespace GAS.RuntimeWithECS.Modifier
{
    public static class MmcHelper
    {
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
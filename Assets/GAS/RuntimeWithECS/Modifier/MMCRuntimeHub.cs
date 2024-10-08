using System;
using System.Collections.Generic;
using GAS.RuntimeWithECS.GameplayEffect.Component;
using Unity.Entities;
using UnityEngine;

namespace GAS.RuntimeWithECS.Modifier
{
    public class MMCRuntimeHub
    {
        private static Dictionary<Type, ModMagnitudeCalculation> _magnitudeCalculations;

        public MMCRuntimeHub()
        {
            _magnitudeCalculations = new Dictionary<Type, ModMagnitudeCalculation>();
            // TODO 初始化项目内所有类型MMC实例

        }

        public static float Calculate(Entity ge, BuffEleModifier modifier)
        {
            var setting = modifier.MMC;
            float result;
#if UNITY_EDITOR
            try
            {
#endif
                var mmc = _magnitudeCalculations[setting.Type];
                mmc.InitParameters(setting.floatParams, setting.intParams, setting.stringParams);
                result = mmc.CalculateMagnitude(ge, modifier.Magnitude);
#if UNITY_EDITOR
            }
            catch (Exception e)
            {
                Debug.LogError(e.Message);
                throw;
            }
#endif
            return result;
        }
    }
}
using System;
using GAS.RuntimeDataHelper.Ability.AbilityParam;
using GAS.RuntimeWithECS.Ability.Component;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Serialization;

namespace DemoForESC._Script.Gas.Ability.AbilityParam
{
    [Serializable]
    public class AbilityParamConfMove: AbilityParamConfigBase<AbilityParamMove>
    {
        [FormerlySerializedAs("RotationOffset")]
        [LabelText("转身缓冲大小")]
        [OnValueChanged(nameof(OnAbilityParamValueChange))]
        public float rotationOffset = 0.1f;
        
        public override AbilityParamBase GetConfig()
        {
            var paramMove = new AbilityParamMove();
            paramMove.SetValue(Vector3.zero,Vector3.zero, rotationOffset);
            return paramMove;
        }
    }
}
using GAS.Runtime;
using Unity.Entities;
using UnityEngine;

namespace GAS.RuntimeWithECS.Ability.Component.CommonAbilityLogic
{
    public class ALDebugLog : AbilityLogicBase<AbilityParamString>
    {
        public ALDebugLog(Entity ability) : base(ability)
        {
        }
        
        public override void AbilityTick(GlobalTimer timer)
        {
            Debug.Log($"Entity:{_abilityEntity} AbilityTick: {_param.Value}");
        }

        public override void ActivateAbility(GlobalTimer timer)
        {
            Debug.Log($"Entity:{_abilityEntity}  ActivateAbility");
        }

        public override void CancelAbility(GlobalTimer timer)
        {
            Debug.Log($"Entity:{_abilityEntity}  CancelAbility");
        }

        public override void EndAbility(GlobalTimer timer)
        {
            Debug.Log($"Entity:{_abilityEntity}  EndAbility");
        }
    }
}
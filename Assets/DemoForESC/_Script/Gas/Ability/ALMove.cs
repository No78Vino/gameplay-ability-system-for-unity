using GAS.RuntimeWithECS.Ability.Component;
using GAS.RuntimeWithECS.Core;
using Unity.Entities;
using UnityEngine;

namespace DemoForESC._Script.Gas.Ability
{
    public class ALMove : AbilityLogicBase<AbilityParamVector3>
    {
        public ALMove(Entity ability) : base(ability)
        {
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

        public override void AbilityTick(GlobalTimer timer)
        {
            Debug.Log($"Entity:{_abilityEntity} AbilityTick: {_param.Value.ToString()}");
        }
    }
}
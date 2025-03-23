using GAS.RuntimeWithECS.Ability.Component;
using Unity.Entities;
using UnityEngine;

namespace DemoForESC._Script.Gas.Ability
{
    public class ALMove : AbilityLogicBase<AbilityParamVector3>
    {
        public ALMove(Entity ability) : base(ability)
        {
        }

        public override void AbilityTick()
        {
            Debug.Log($"Entity:{_abilityEntity} AbilityTick: {_param.Value.ToString()}");
        }

        public override void ActivateAbility()
        {
            Debug.Log($"Entity:{_abilityEntity}  ActivateAbility");
        }

        public override void CancelAbility()
        {
            Debug.Log($"Entity:{_abilityEntity}  CancelAbility");
        }

        public override void EndAbility()
        {
            Debug.Log($"Entity:{_abilityEntity}  EndAbility");
        }
    }
}
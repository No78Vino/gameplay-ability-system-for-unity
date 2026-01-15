using Unity.Entities;
using UnityEngine;

namespace GAS.Runtime
{
    public class ALDebugLog : AbilityLogicBase<XParamString>
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
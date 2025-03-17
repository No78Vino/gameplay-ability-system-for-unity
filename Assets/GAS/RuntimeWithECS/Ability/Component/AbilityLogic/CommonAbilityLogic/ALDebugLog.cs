using Unity.Entities;
using UnityEngine;

namespace GAS.RuntimeWithECS.Ability.Component.CommonAbilityLogic
{
    public class ALDebugLog : AbilityLogicBase
    {
        private AbilityParamString _logString;
        
        public ALDebugLog(Entity ability) : base(ability)
        {
        }
        
        public override void AbilityTick()
        {
            Debug.Log($"Entity:{_abilityEntity} AbilityTick: {_logString.Value}");
        }

        public override void SetParam(AbilityParamBase abilityParam)
        {
            base.SetParam(abilityParam);
            _logString = (AbilityParamString)abilityParam;
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
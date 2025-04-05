using GAS.Runtime;
using UnityEngine;

namespace DemoForESC._Script
{
    public class DemoPlayer : BaseUnit
    {
        UnityEngine.Camera _mainCamera;

        protected override void Awake()
        {
            base.Awake();
            _mainCamera = UnityEngine.Camera.main;
        }

        public override void Move(Vector3 direction)
        {
            if(!IsMoving()) AbilitySystemCellMono.TryActivateAbility(GEN_AbilityCode.ABILITY_move,_cacheParamMove);
            
            var viewPointForward = Vector3.ProjectOnPlane(_mainCamera.transform.forward, Vector3.up).normalized;
            _cacheParamMove.SetValue(direction,viewPointForward,0.1f);
            AbilitySystemCellMono.Cell.SetAbilityParam(GEN_AbilityCode.ABILITY_move,_cacheParamMove);
        }
    }
}
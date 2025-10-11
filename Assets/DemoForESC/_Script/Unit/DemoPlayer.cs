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
            if(!AbilitySystemCellMono.Cell.IsAbilityActive(XAbility.ABILITY_move))
                AbilitySystemCellMono.TryActivateAbility(XAbility.ABILITY_move,_cacheParamMove);
            
            var viewPointForward = Vector3.ProjectOnPlane(_mainCamera.transform.forward, Vector3.up).normalized;
            _cacheParamMove.SetValue(direction,viewPointForward);
            AbilitySystemCellMono.Cell.SetAbilityParam(XAbility.ABILITY_move,_cacheParamMove);
        }

        public void StartRun()
        {
            if(!AbilitySystemCellMono.Cell.IsAbilityActive(XAbility.ABILITY_RunSpeedUp))
                AbilitySystemCellMono.TryActivateAbility(XAbility.ABILITY_RunSpeedUp);
        }

        public void StopRun()
        {
            if(AbilitySystemCellMono.Cell.IsAbilityActive(XAbility.ABILITY_RunSpeedUp)) 
                AbilitySystemCellMono.TryEndAbility(XAbility.ABILITY_RunSpeedUp);
        }
    }
}
using GAS.Runtime;
using UnityEngine;

namespace DemoForESC._Script
{
    public class DemoPlayer : BaseUnit
    {
        private static DemoPlayer _player;
        public static DemoPlayer Player()
        {
            if (_player == null)
                _player = FindObjectOfType<DemoPlayer>();
            
            return _player;
        }
        
        UnityEngine.Camera _mainCamera;
        
        protected override void Awake()
        {
            base.Awake();
            _mainCamera = UnityEngine.Camera.main;


            AbilitySystemCellMono.Cell.AddFixedTag(XTag.Ability);
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
        
        public void StartDebugGE1()
        {
            if(!AbilitySystemCellMono.Cell.IsAbilityActive(XAbility.ABILITY_debug_ge_ability)) 
                AbilitySystemCellMono.TryActivateAbility(XAbility.ABILITY_debug_ge_ability);
        }
        
        public void StartDebugGE2()
        {
            if(!AbilitySystemCellMono.Cell.IsAbilityActive(XAbility.ABILITY_debug_ge_2)) 
                AbilitySystemCellMono.TryActivateAbility(XAbility.ABILITY_debug_ge_2);
        }
        
        public void StopDebugGE1()
        {
            if(AbilitySystemCellMono.Cell.IsAbilityActive(XAbility.ABILITY_debug_ge_ability)) 
                AbilitySystemCellMono.TryEndAbility(XAbility.ABILITY_debug_ge_ability);
        }
        
        public void StopDebugGE2()
        {
            if(AbilitySystemCellMono.Cell.IsAbilityActive(XAbility.ABILITY_debug_ge_2)) 
                AbilitySystemCellMono.TryEndAbility(XAbility.ABILITY_debug_ge_2);
        }
    }
}
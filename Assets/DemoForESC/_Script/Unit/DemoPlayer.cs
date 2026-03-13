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
        
        Camera _mainCamera;
        
        protected override void Awake()
        {
            base.Awake();
            _mainCamera = UnityEngine.Camera.main;

            
            // 自动恢复耐力Buff
            var buff1 = new GameplayEffectSpec(XLuban.GetGameplayEffectConfig(1007).ComponentConfigs);
            AbilitySystemComponent.Cell.ApplyGameplayEffectToSelf(buff1);
        }

        public override void Move(Vector3 direction)
        {
            if(!AbilitySystemComponent.Cell.IsAbilityActive(XAbility.ABILITY_move))
                AbilitySystemComponent.TryActivateAbility(XAbility.ABILITY_move,_cacheParamMove);
            
            var viewPointForward = Vector3.ProjectOnPlane(_mainCamera.transform.forward, Vector3.up).normalized;
            _cacheParamMove.SetDirection(direction,viewPointForward);
            AbilitySystemComponent.Cell.SetAbilityParam(XAbility.ABILITY_move,_cacheParamMove);
        }

        public void StartRun()
        {
            if(!AbilitySystemComponent.Cell.IsAbilityActive(XAbility.ABILITY_RunSpeedUp))
                AbilitySystemComponent.TryActivateAbility(XAbility.ABILITY_RunSpeedUp);
        }

        public void StopRun()
        {
            if(AbilitySystemComponent.Cell.IsAbilityActive(XAbility.ABILITY_RunSpeedUp)) 
                AbilitySystemComponent.TryEndAbility(XAbility.ABILITY_RunSpeedUp);
        }
        
        public void StartDebugGE1()
        {
            if(!AbilitySystemComponent.Cell.IsAbilityActive(XAbility.ABILITY_debug_ge_ability)) 
                AbilitySystemComponent.TryActivateAbility(XAbility.ABILITY_debug_ge_ability);
        }
        
        public void StartDebugGE2()
        {
            if(!AbilitySystemComponent.Cell.IsAbilityActive(XAbility.ABILITY_debug_ge_2)) 
                AbilitySystemComponent.TryActivateAbility(XAbility.ABILITY_debug_ge_2);
        }
        
        public void StopDebugGE1()
        {
            if(AbilitySystemComponent.Cell.IsAbilityActive(XAbility.ABILITY_debug_ge_ability)) 
                AbilitySystemComponent.TryEndAbility(XAbility.ABILITY_debug_ge_ability);
        }
        
        public void StopDebugGE2()
        {
            if(AbilitySystemComponent.Cell.IsAbilityActive(XAbility.ABILITY_debug_ge_2)) 
                AbilitySystemComponent.TryEndAbility(XAbility.ABILITY_debug_ge_2);
        }

        public void Dodge()
        {
            AbilitySystemComponent.TryActivateAbility(XAbility.ABILITY_Dodge);
        }

        protected override void OnSpChangeAfter(float lastSp, float newSp)
        {
            base.OnSpChangeAfter(lastSp, newSp);
            if(newSp<=0) StopRun();
        }
    }
}
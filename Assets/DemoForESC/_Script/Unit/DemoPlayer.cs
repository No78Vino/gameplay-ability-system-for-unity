using GAS.Runtime;
using GAS.RuntimeWithECS.Ability.Component;
using Sirenix.OdinInspector;
using UnityEngine;

namespace DemoForESC._Script
{
    public class DemoPlayer : BaseUnit
    {
        UnityEngine.Camera _mainCamera;

        private AbilityParamArrayGameplayEffect _cacheParamRun = new();

        
        protected override void Awake()
        {
            base.Awake();
            _mainCamera = UnityEngine.Camera.main;
        }

        public override void Move(Vector3 direction)
        {
            if(!AbilitySystemCellMono.Cell.IsAbilityActive(GEN_AbilityCode.ABILITY_move))
                AbilitySystemCellMono.TryActivateAbility(GEN_AbilityCode.ABILITY_move,_cacheParamMove);
            
            var viewPointForward = Vector3.ProjectOnPlane(_mainCamera.transform.forward, Vector3.up).normalized;
            _cacheParamMove.SetValue(direction,viewPointForward,0.1f);
            AbilitySystemCellMono.Cell.SetAbilityParam(GEN_AbilityCode.ABILITY_move,_cacheParamMove);
        }

        public void StartRun()
        {
            if(!AbilitySystemCellMono.Cell.IsAbilityActive(GEN_AbilityCode.ABILITY_RunSpeedUp))
                AbilitySystemCellMono.TryActivateAbility(GEN_AbilityCode.ABILITY_RunSpeedUp,_cacheParamRun);
        }

        public void StopRun()
        {
            if(AbilitySystemCellMono.Cell.IsAbilityActive(GEN_AbilityCode.ABILITY_RunSpeedUp)) 
                AbilitySystemCellMono.TryEndAbility(GEN_AbilityCode.ABILITY_RunSpeedUp);
        }

        [Button(name:"注册SpeedBaseValue变化事件")]
        public void TestButton1()
        {
            GASEventCenter.RegisterOnBaseValueChangeAfter(
                AbilitySystemCellMono.Cell,
                GEN_AttrSetCode.Fight,GEN_AttributeCode.SPEED,
                ( oldValue, newValue) =>
                {
                    Debug.Log($"Speed BaseValue After Changed: {oldValue} - {newValue}");
                });
            GASEventCenter.SetOnBaseValueChangeBefore( 
                AbilitySystemCellMono.Cell,
                GEN_AttrSetCode.Fight,GEN_AttributeCode.SPEED,
                (oldValue) =>
                {
                    var newValue = oldValue - 1;
                    Debug.Log($"Speed BaseValue Before Changing: {newValue}");
                    return newValue;
                });
            GASEventCenter.RegisterOnCurrentValueChangeAfter( 
                AbilitySystemCellMono.Cell,
                GEN_AttrSetCode.Fight,GEN_AttributeCode.SPEED,
                (oldValue, newValue) =>
                {
                    Debug.Log($"Speed CurrentValue After Changed: {oldValue} - {newValue}");
                });
        }
        
        [Button(name:"修改Speed的BaseValue")]
        public void TestButton2()
        {
            AbilitySystemCellMono.SetAttrBaseValue(
                GEN_AttrSetCode.Fight, GEN_AttributeCode.SPEED, 19f);
        }
    }
}
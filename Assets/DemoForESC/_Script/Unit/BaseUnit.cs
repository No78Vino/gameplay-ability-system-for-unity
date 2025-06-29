using DemoForESC._Script.Controller;
using DemoForESC._Script.Gas.Ability;
using DemoForESC._Script.Gen;
using EXToyLib;
using GAS.Runtime;
using GAS.RuntimeDataHelper.Ability;
using GAS.RuntimeDataHelper.ASCPreset;
using GAS.RuntimeWithECS.Ability.Component;
using GAS.RuntimeWithECS.AbilitySystemCell;
using Unity.VisualScripting;
using UnityEngine;

namespace DemoForESC._Script
{
    /// <summary>
    /// 基础单位
    /// </summary>
    public class BaseUnit : MonoBehaviour
    {
        protected AbilityParamMove _cacheParamMove = new AbilityParamMove();

        public AbilitySystemCellMono AbilitySystemCellMono { get; private set; }

        [SerializeField]
        private AbilitySystemCellConfigAsset _configAsset;
        
        protected virtual void Awake()
        {
            AbilitySystemCellMono = transform.GetOrAddComponent<AbilitySystemCellMono>();
            AbilitySystemCellMono.Init(_configAsset.GetConfig());
            var abilityLogic = AbilitySystemCellMono.Cell.GetAbilityLogic(GEN_AbilityCode.ABILITY_move);
            ((ALMove)abilityLogic.Logic).SetUnit(this);
        }
        
        private void OnEnable()
        {
            GravityForCharacterController.Instance.Register(GetComponent<CharacterController>());
        }
        
        private void OnDisable()
        {
            GravityForCharacterController.Instance.Unregister(GetComponent<CharacterController>());
        }
        
        public virtual void Move(Vector3 direction)
        {
            if(!AbilitySystemCellMono.Cell.IsAbilityActive(GEN_AbilityCode.ABILITY_move))
                AbilitySystemCellMono.TryActivateAbility(GEN_AbilityCode.ABILITY_move,_cacheParamMove);
            
            var viewPointForward = Vector3.ProjectOnPlane(transform.forward, Vector3.up).normalized;
            _cacheParamMove.SetValue(direction,viewPointForward,0.1f);
            AbilitySystemCellMono.Cell.SetAbilityParam(GEN_AbilityCode.ABILITY_move,_cacheParamMove);
        }
        
        public virtual void StopMove()
        {
            if(AbilitySystemCellMono.Cell.IsAbilityActive(GEN_AbilityCode.ABILITY_move)) 
                AbilitySystemCellMono.TryEndAbility(GEN_AbilityCode.ABILITY_move);
        }
        
        public virtual void Jump()
        {
            //_abilitySystemCellMono.TryActivateAbility(GEN_AbilityCode.Jump);
        }
        
        public virtual void Attack()
        {
            //_abilitySystemCellMono.TryActivateAbility(GEN_AbilityCode.Attack);
        }

        public bool IsMoving()
        {
            return AbilitySystemCellMono.Cell.HasTag(GTagLib.Event_Moving.HashCode);
        }

        #region Attributes

        public float GetSpeed()
        {
            return AbilitySystemCellMono.GetAttrCurrentValue(GEN_AttrSetCode.Fight ,GEN_AttributeCode.SPEED)/100;
        }

        #endregion
    }
}
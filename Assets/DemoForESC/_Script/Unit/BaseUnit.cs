using DemoForESC._Script.Gas.Ability;
using EXToyLib;
using GAS.Runtime;
using Sirenix.OdinInspector;
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

        [ShowInInspector]
        [LabelText("ASC预设")]
        [ValueDropdown("@GasXlsxChoice.Ascs()")]
        public int _ascPresetId = 0;
        
        protected virtual void Awake()
        {
            AbilitySystemCellMono = transform.GetOrAddComponent<AbilitySystemCellMono>();
            AbilitySystemCellMono.Init(XLuban.GetAscConfig(_ascPresetId));
            var abilityLogic = AbilitySystemCellMono.Cell.GetAbilityLogic(XAbility.ABILITY_move);
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
            if(!AbilitySystemCellMono.Cell.IsAbilityActive(XAbility.ABILITY_move))
                AbilitySystemCellMono.TryActivateAbility(XAbility.ABILITY_move,_cacheParamMove);
            
            var viewPointForward = Vector3.ProjectOnPlane(transform.forward, Vector3.up).normalized;
            _cacheParamMove.SetValue(direction,viewPointForward);
            AbilitySystemCellMono.Cell.SetAbilityParam(XAbility.ABILITY_move,_cacheParamMove);
        }
        
        public virtual void StopMove()
        {
            if(AbilitySystemCellMono.Cell.IsAbilityActive(XAbility.ABILITY_move)) 
                AbilitySystemCellMono.TryEndAbility(XAbility.ABILITY_move);
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
            var tagMoving = 1; //GTagLib.Event_Moving.HashCode
            return AbilitySystemCellMono.Cell.HasTag(tagMoving);
        }

        #region Attributes

        public float GetSpeed()
        {
            return AbilitySystemCellMono.GetAttrCurrentValue(XAttrSet.FightUnit ,XAttribute.Spd);
        }

        #endregion
    }
}
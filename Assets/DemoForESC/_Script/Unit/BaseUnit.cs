using DemoForESC._Script.Gas.Ability;
using EXToyLib;
using GAS.Runtime;
using Sirenix.OdinInspector;
using Unity.Mathematics;
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
        
        protected virtual void OnEnable()
        {
            GravityForCharacterController.Instance.Register(GetComponent<CharacterController>());
            // 血量，蓝量，耐力最大值钳制回调
            GASEventCenter.SetOnAttrBaseValueChangeBefore(AbilitySystemCellMono.Cell,XAttrSet.FightUnit,XAttribute.Hp,OnHpChangeBefore);
            GASEventCenter.SetOnAttrBaseValueChangeBefore(AbilitySystemCellMono.Cell,XAttrSet.FightUnit,XAttribute.Mp,OnMpChangeBefore);
            GASEventCenter.SetOnAttrBaseValueChangeBefore(AbilitySystemCellMono.Cell,XAttrSet.FightUnit,XAttribute.Sp,OnSpChangeBefore);
        }
        
        protected virtual void OnDisable()
        {
            GravityForCharacterController.Instance.Unregister(GetComponent<CharacterController>());
            // 血量，蓝量，耐力最大值钳制回调
            GASEventCenter.ClearOnAttrBaseValueChangeBefore(AbilitySystemCellMono.Cell,XAttrSet.FightUnit,XAttribute.Hp);
            GASEventCenter.ClearOnAttrBaseValueChangeBefore(AbilitySystemCellMono.Cell,XAttrSet.FightUnit,XAttribute.Mp);
            GASEventCenter.ClearOnAttrBaseValueChangeBefore(AbilitySystemCellMono.Cell,XAttrSet.FightUnit,XAttribute.Sp);
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

        private float OnHpChangeBefore(float newHp)
        {
            var hpMax = AbilitySystemCellMono.GetAttrCurrentValue(XAttrSet.FightUnit, XAttribute.HpMax);
            return math.min(newHp, hpMax);
        }
        private float OnMpChangeBefore(float newMp)
        {
            var mpMax = AbilitySystemCellMono.GetAttrCurrentValue(XAttrSet.FightUnit, XAttribute.MpMax);
            return math.min(newMp, mpMax);
        }
        private float OnSpChangeBefore(float newSp)
        {
            var spMax = AbilitySystemCellMono.GetAttrCurrentValue(XAttrSet.FightUnit, XAttribute.SpMax);
            return math.min(newSp, spMax);
        }
        
        #endregion
    }
}
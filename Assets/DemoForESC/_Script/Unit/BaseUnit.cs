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
        protected XParamMove _cacheParamMove = new XParamMove();

        public AbilitySystemComponent AbilitySystemComponent { get; private set; }

        [ShowInInspector]
        [LabelText("ASC预设")]
        [ValueDropdown("@GasXlsxChoice.Ascs()")]
        public int _ascPresetId = 0;
        
        protected virtual void Awake()
        {
            AbilitySystemComponent = transform.GetOrAddComponent<AbilitySystemComponent>();
            AbilitySystemComponent.Init(XLuban.GetAscConfig(_ascPresetId));
            var abilityLogic = AbilitySystemComponent.Cell.GetAbilityLogic(XAbility.ABILITY_move);
            ((ALMove)abilityLogic.Logic).SetUnit(this);
        }
        
        protected virtual void OnEnable()
        {
            GravityForCharacterController.Instance.Register(GetComponent<CharacterController>());
            // 血量，蓝量，耐力最大值钳制回调
            GASEventCenter.SetOnAttrBaseValueChangeBefore(AbilitySystemComponent.Cell,XAttrSet.FightUnit,XAttribute.Hp,OnHpChangeBefore);
            GASEventCenter.SetOnAttrBaseValueChangeBefore(AbilitySystemComponent.Cell,XAttrSet.FightUnit,XAttribute.Mp,OnMpChangeBefore);
            GASEventCenter.SetOnAttrBaseValueChangeBefore(AbilitySystemComponent.Cell,XAttrSet.FightUnit,XAttribute.Sp,OnSpChangeBefore);
            GASEventCenter.RegisterOnAttrCurrentValueChangeAfter(AbilitySystemComponent.Cell,XAttrSet.FightUnit,XAttribute.Sp,OnSpChangeAfter);
        }
        
        protected virtual void OnDisable()
        {
            GravityForCharacterController.Instance.Unregister(GetComponent<CharacterController>());
            // 血量，蓝量，耐力最大值钳制回调
            GASEventCenter.ClearOnAttrBaseValueChangeBefore(AbilitySystemComponent.Cell,XAttrSet.FightUnit,XAttribute.Hp);
            GASEventCenter.ClearOnAttrBaseValueChangeBefore(AbilitySystemComponent.Cell,XAttrSet.FightUnit,XAttribute.Mp);
            GASEventCenter.ClearOnAttrBaseValueChangeBefore(AbilitySystemComponent.Cell,XAttrSet.FightUnit,XAttribute.Sp);
            GASEventCenter.UnRegisterOnAttrCurrentValueChangeAfter(AbilitySystemComponent.Cell,XAttrSet.FightUnit,XAttribute.Sp,OnSpChangeAfter);
        }
        
        public virtual void Move(Vector3 direction)
        {
            if(!AbilitySystemComponent.Cell.IsAbilityActive(XAbility.ABILITY_move))
                AbilitySystemComponent.TryActivateAbility(XAbility.ABILITY_move,_cacheParamMove);
            
            var viewPointForward = Vector3.ProjectOnPlane(transform.forward, Vector3.up).normalized;
            _cacheParamMove.SetDirection(direction,viewPointForward);
            AbilitySystemComponent.Cell.SetAbilityParam(XAbility.ABILITY_move,_cacheParamMove);
        }
        
        public virtual void StopMove()
        {
            if(AbilitySystemComponent.Cell.IsAbilityActive(XAbility.ABILITY_move)) 
                AbilitySystemComponent.TryEndAbility(XAbility.ABILITY_move);
        }
        
        public virtual void Jump()
        {
            //_abilitySystemCellMono.TryActivateAbility(GEN_AbilityCode.Jump);
        }
        
        public virtual void Attack()
        {
            AbilitySystemComponent.Cell.TryActivateAbility(XAbility.ABILITY_Attack);
        }

        public bool IsMoving()
        {
            var tagMoving = 1; //GTagLib.Event_Moving.HashCode
            return AbilitySystemComponent.Cell.HasTag(tagMoving);
        }

        #region Attributes

        public float GetSpeed()
        {
            return AbilitySystemComponent.GetAttrCurrentValue(XAttrSet.FightUnit ,XAttribute.Spd);
        }

        private float OnHpChangeBefore(float newHp)
        {
            var hpMax = AbilitySystemComponent.GetAttrCurrentValue(XAttrSet.FightUnit, XAttribute.HpMax);
            return math.min(newHp, hpMax);
        }
        private float OnMpChangeBefore(float newMp)
        {
            var mpMax = AbilitySystemComponent.GetAttrCurrentValue(XAttrSet.FightUnit, XAttribute.MpMax);
            return math.min(newMp, mpMax);
        }
        private float OnSpChangeBefore(float newSp)
        {
            var spMax = AbilitySystemComponent.GetAttrCurrentValue(XAttrSet.FightUnit, XAttribute.SpMax);
            return math.min(newSp, spMax);
        }
        
        protected virtual void OnSpChangeAfter(float lastSp,float newSp)
        {
        }
        
        
        #endregion
    }
}
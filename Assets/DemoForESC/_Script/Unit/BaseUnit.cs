using DemoForESC._Script.Gen;
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
        private AbilitySystemCellMono _abilitySystemCellMono;
        
        [SerializeField]
        private AbilitySystemCellConfigAsset _configAsset;
        
        private void Awake()
        {
            _abilitySystemCellMono = transform.GetOrAddComponent<AbilitySystemCellMono>();
            _abilitySystemCellMono.Init(_configAsset.GetConfig());
        }
        
        public virtual void Move(Vector3 direction)
        {
            var param = new AbilityParamVector3(direction);
            _abilitySystemCellMono.TryActivateAbility(Gen_AbilityCode.Move,param);
        }
        
        public virtual void Jump()
        {
            _abilitySystemCellMono.TryActivateAbility(Gen_AbilityCode.Jump);
        }
        
        public virtual void Attack()
        {
            _abilitySystemCellMono.TryActivateAbility(Gen_AbilityCode.Attack);
        }
    }
}
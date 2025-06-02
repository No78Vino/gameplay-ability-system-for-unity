using GAS.Runtime;
using GAS.RuntimeWithECS.Ability.Component;
using UnityEngine;

namespace GAS.RuntimeWithECS.AbilitySystemCell
{
    public class AbilitySystemCellMono : MonoBehaviour
    {
        private AbilitySystemCell _abilitySystemCell;

        private void Awake()
        {
            _abilitySystemCell = new AbilitySystemCell();
            EntityHelper.BindGameObjectToEntity(_abilitySystemCell.Entity, gameObject);
        }

        private void OnDestroy()
        {
            EntityHelper.UnbindGameObjectToEntity(_abilitySystemCell.Entity);
            _abilitySystemCell.Dispose();
        }

        public void Init(AbilitySystemCellConfig config)
        {
            _abilitySystemCell.Init(config.BaseTags, config.AttrSets, config.BaseAbilities, config.Level);
        }

        public void TryActivateAbility(int abilityId, AbilityParamBase param = null) =>
            _abilitySystemCell.TryActivateAbility(abilityId, param);

        public void TryEndAbility(int abilityCode) => _abilitySystemCell.TryEndAbility(abilityCode);

        public void TryCancelAbility(int abilityCode)=> _abilitySystemCell.TryCancelAbility(abilityCode);

        public AbilitySystemCell Cell => _abilitySystemCell;

        #region Attribute

        public float GetAttrCurrentValue(int attrSetCode,int attributeCode)
        {
            return _abilitySystemCell.GetAttrCurrentValue(attrSetCode,attributeCode);
        }
        
        public float GetAttrBaseValue(int attrSetCode,int attributeCode)
        {
            return _abilitySystemCell.GetAttrBaseValue(attrSetCode,attributeCode);
        }

        public void SetAttrBaseValue(int attrSetCode,int attributeCode,float value)
        {
            _abilitySystemCell.SetAttrBaseValue(attrSetCode,attributeCode,value);
        }
        
        #endregion
    }
}
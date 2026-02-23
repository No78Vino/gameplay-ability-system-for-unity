using System;
using UnityEngine;

namespace GAS.Runtime
{
    public class AbilitySystemComponent : MonoBehaviour
    {
        private AbilitySystemCell _abilitySystemCell;

        private void Awake()
        {
            _abilitySystemCell = new AbilitySystemCell();
        }

        private void OnDestroy()
        {
            _abilitySystemCell.Dispose();
        }
        
        private void OnEnable()
        {
            EntityHelper.BindGameObjectToEntity(_abilitySystemCell.Entity, gameObject);
        }

        private void OnDisable()
        {
            EntityHelper.UnbindGameObjectToEntity(_abilitySystemCell.Entity);
        }
        

        public void Init(AbilitySystemCellConfig config)
        {
            _abilitySystemCell.Init(config.BaseTags, config.AttrSets, config.BaseAbilities, config.Level);
        }

        public void TryActivateAbility(int abilityId, XParam param = null) =>
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

        #region GameplayTag

        public bool HasTag(int gameplayTag)
        {
            return _abilitySystemCell.HasTag(gameplayTag);
        }

        #endregion
    }
}
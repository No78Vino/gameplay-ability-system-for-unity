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
        }

        public void Init(AbilitySystemCellConfig config)
        {
            _abilitySystemCell.Init(config.BaseTags, config.AttrSets, config.BaseAbilities, config.Level);
        }

        public void TryActivateAbility(int abilityId, AbilityParamBase param = null) =>
            _abilitySystemCell.TryActivateAbility(abilityId, param);

        public void TryEndAbility(int abilityCode) => _abilitySystemCell.TryEndAbility(abilityCode);

        public void TryCancelAbility(int abilityCode)=> _abilitySystemCell.TryCancelAbility(abilityCode);
    }
}
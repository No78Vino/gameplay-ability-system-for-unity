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

        public void Init()
        {
            var baseTags = new[] { 1, 2, 3 };
            var attrSets = new[] { 1, 2, 3 };
            //var baseAbilities = null;
            _abilitySystemCell.Init(baseTags, attrSets, null);
        }

        public void TryActivateAbility(int abilityId, params object[] args) =>
            _abilitySystemCell.TryActivateAbility(abilityId, args);

        public void TryEndAbility(int abilityCode) => _abilitySystemCell.TryEndAbility(abilityCode);

        public void TryCancelAbility(int abilityCode)=> _abilitySystemCell.TryCancelAbility(abilityCode);
    }
}
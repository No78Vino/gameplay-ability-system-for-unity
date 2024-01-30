using Sirenix.OdinInspector;
using UnityEngine;

namespace GAS.Runtime.Ability
{
    public class MoveAbilityAsset:AbilityAsset
    {
        [Space]
        [BoxGroup("MoveAbilityAsset",false)]
        [Title("MoveAbilityAsset", bold: true)]
        public bool IsControlledByOwner;
    }
}
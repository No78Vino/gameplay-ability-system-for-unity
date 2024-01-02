using GAS.Runtime.Ability;
using GAS.Runtime.Tags;
using UnityEngine;
using UnityEngine.Serialization;

namespace GAS.Runtime.Component
{
    [CreateAssetMenu(fileName = "AbilitySystemComponentPreset", menuName = "GAS/AbilitySystemComponentPreset")]
    public class AbilitySystemComponentPreset : ScriptableObject
    {
        public string Name;
        public string Description;

        public GameplayTag[] BaseTags;
        [FormerlySerializedAs("Abilities")] public AbilityAsset[] BaseAbilities;
    }
}
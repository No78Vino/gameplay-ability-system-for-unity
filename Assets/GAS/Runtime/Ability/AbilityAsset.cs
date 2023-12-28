using GAS.Runtime.Effects;
using UnityEngine;

namespace GAS.Runtime.Ability
{
    [CreateAssetMenu(fileName = "AbilityAsset", menuName = "GAS/AbilityAsset", order = 0)]
    public class AbilityAsset:ScriptableObject
    {
        public string Name;
        public string Description;
        
        public GameplayEffectAsset Cost;
        public GameplayEffectAsset Cooldown;
        
        public GameplayEffectAsset[] UsedGameplayEffects;
        
        // Tags
        public string[] AssetTag;
        public string[] CancelAbilityTags;
        public string[] BlockAbilityTags;
        public string[] ActivationOwnedTag;
        public string[] ActivationRequiredTags;
        public string[] ActivationBlockedTags;
        public string[] SourceRequiredTags;
        public string[] SourceBlockedTags;
        public string[] TargetRequiredTags;
        public string[] TargetBlockedTags;
    }
}
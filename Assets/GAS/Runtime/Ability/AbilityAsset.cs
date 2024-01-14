using GAS.Runtime.Effects;
using GAS.Runtime.Tags;
using UnityEngine;

namespace GAS.Runtime.Ability
{
    [CreateAssetMenu(fileName = "AbilityAsset", menuName = "GAS/AbilityAsset", order = 0)]
    public class AbilityAsset : ScriptableObject
    {
        public string Name;
        public string Description;

        public string UniqueName;
        public string InstanceAbilityClassFullName; 
        
        public GameplayEffectAsset Cost;
        public GameplayEffectAsset Cooldown;
        public GameplayEffectAsset[] UsedGameplayEffects = new GameplayEffectAsset[0];

        // Tags
        public GameplayTag[] AssetTag;
        public GameplayTag[] CancelAbilityTags;
        public GameplayTag[] BlockAbilityTags;
        public GameplayTag[] ActivationOwnedTag;
        public GameplayTag[] ActivationRequiredTags;
        public GameplayTag[] ActivationBlockedTags;
        public GameplayTag[] SourceRequiredTags;
        public GameplayTag[] SourceBlockedTags;
        public GameplayTag[] TargetRequiredTags;
        public GameplayTag[] TargetBlockedTags;
    }
}
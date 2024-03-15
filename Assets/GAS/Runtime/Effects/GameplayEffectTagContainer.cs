namespace GAS.Runtime
{
    /// <summary>
    /// https://github.com/BillEliot/GASDocumentation_Chinese?tab=readme-ov-file#457-gameplayeffect%E6%A0%87%E7%AD%BE
    /// </summary>
    public struct GameplayEffectTagContainer
    {
        public GameplayTagSet AssetTags;
        
        public GameplayTagSet GrantedTags;
        
        public GameplayTagSet ApplicationRequiredTags;
        public GameplayTagSet OngoingRequiredTags;
        
        public GameplayTagSet RemoveGameplayEffectsWithTags;
        
        public GameplayTagSet ApplicationImmunityTags;

        public GameplayEffectTagContainer(
            GameplayTag[] assetTags, 
            GameplayTag[] grantedTags,
            GameplayTag[] applicationRequiredTags,
            GameplayTag[] ongoingRequiredTags,
            GameplayTag[] removeGameplayEffectsWithTags,
            GameplayTag[] applicationImmunityTags)
        {
            AssetTags = new GameplayTagSet(assetTags);
            GrantedTags = new GameplayTagSet(grantedTags);
            ApplicationRequiredTags = new GameplayTagSet(applicationRequiredTags);
            OngoingRequiredTags = new GameplayTagSet(ongoingRequiredTags);
            RemoveGameplayEffectsWithTags = new GameplayTagSet(removeGameplayEffectsWithTags);
            ApplicationImmunityTags = new GameplayTagSet(applicationImmunityTags);
        }
    }
}
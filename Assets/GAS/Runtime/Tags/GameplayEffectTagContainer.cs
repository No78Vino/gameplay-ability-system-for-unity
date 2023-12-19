using System.Collections.Generic;

namespace GAS.Runtime.Tags
{
    public class GameplayEffectTagContainer
    {
        /// <summary>
        /// For the description of the GameplayEffect
        /// </summary>
        public GameplayTagSet AssetTags;
        
        /// <summary>
        /// For attachments to the ASC
        /// And the ACS will lose these tags when the GameplayEffect is removed
        /// </summary>
        public GameplayTagSet GrantedTags;
        
        /// <summary>
        /// For the Application Requirement
        /// </summary>
        public GameplayTagSet ApplicationRequiredTags;
        
        /// <summary>
        /// For the Requirement of Keeping Running 
        /// </summary>
        public GameplayTagSet KeepRunningRequiredTags;
        
        /// <summary>
        /// When the GameplayEffect is active, the ASC will lose these tags
        /// </summary>
        public GameplayTagSet RemoveGameplayEffectWithTags;
        
        /// <summary>
        /// the GameplayEffectSpec grants to the Target in addition to the GameplayTags that the GameplayEffect grants.
        /// </summary>
        public GameplayTagSet DynamicGrantedTags;
        
        /// <summary>
        /// the GameplayEffectSpec has in addition to the AssetTags that the GameplayEffect has.
        /// </summary>
        public GameplayTagSet DynamicAssetTags;
    }
}
using System.Collections.Generic;

namespace GAS.Runtime.Tags
{
    public class GameplayEffectTagContainer
    {
        /// <summary>
        /// For the description of the GameplayEffect
        /// </summary>
        public List<GameplayTag> AssetTags;
        
        /// <summary>
        /// For attachments to the ASC
        /// And the ACS will lose these tags when the GameplayEffect is removed
        /// </summary>
        public List<GameplayTag> GrantedTags;
        
        /// <summary>
        /// For the Application Requirement
        /// </summary>
        public List<GameplayTag> ApplicationRequiredTags;
        
        /// <summary>
        /// For the Requirement of Keeping Running 
        /// </summary>
        public List<GameplayTag> KeepRunningRequiredTags;
        
        /// <summary>
        /// When the GameplayEffect is active, the ASC will lose these tags
        /// </summary>
        public List<GameplayTag> RemoveGameplayEffectWithTags;
        
        /// <summary>
        /// the GameplayEffectSpec grants to the Target in addition to the GameplayTags that the GameplayEffect grants.
        /// </summary>
        public List<GameplayTag> DynamicGrantedTags;
        
        /// <summary>
        /// the GameplayEffectSpec has in addition to the AssetTags that the GameplayEffect has.
        /// </summary>
        public List<GameplayTag> DynamicAssetTags;
    }
}
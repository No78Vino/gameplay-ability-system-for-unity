using GAS.Runtime.Tags;

namespace GAS.Runtime.Effects
{
    public class GameplayEffectTagContainer
    {
        /// <summary>
        /// For the description of the GameplayEffect
        /// </summary>
        public GameplayTagSet AssetTags;
        
        /// <summary>
        /// For attachments to the ASC, if GameplayEffect is active
        /// And the ACS will lose these tags when the GameplayEffect is deactive
        /// </summary>
        public GameplayTagSet GrantedTags;
        
        /// <summary>
        /// For banning tags to the ASC, if GameplayEffect is active
        /// And the ACS will grant these tags when the GameplayEffect is deactive
        /// </summary>
        public GameplayTagSet BannedTags;
        
        /// <summary>
        /// For the Application Requirement
        /// </summary>
        public GameplayTagSet RequiredApplicationTags;
        
        /// <summary>
        /// For the Requirement of Keeping Running 
        /// </summary>
        public GameplayTagSet RequiredOngoingTags;
        
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
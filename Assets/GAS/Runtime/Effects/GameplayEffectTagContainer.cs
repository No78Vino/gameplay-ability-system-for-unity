using System;

namespace GAS.Runtime
{
    /// <summary>
    /// https://github.com/BillEliot/GASDocumentation_Chinese?tab=readme-ov-file#457-gameplayeffect%E6%A0%87%E7%AD%BE
    /// </summary>
    public struct GameplayEffectTagContainer
    {
        /// <summary>
        /// 游戏效果(GE)拥有的标签, 它们自身没有任何功能且只用于描述GameplayEffect。
        /// 此标签集合用于RemoveGameplayEffectsWithTags的匹配条件, 因此对于Instant型GE没有意义。
        /// 注意：GrantedTags也会被用于RemoveGameplayEffectsWithTags的匹配。
        /// </summary>
        public GameplayTagSet AssetTags;

        /// <summary>
        /// 当游戏效果(GE)处于激活状态时，赋予目标的标签集合。
        /// 存于GameplayEffect中且又用于GameplayEffect所应用ASC的标签.
        /// 当GameplayEffect移除时它们也会从ASC中移除. 该标签只作用于持续(Duration)和无限(Infinite)GameplayEffect, 对于Instant型GE没有意义.
        /// 当GE处于非激活状态时，这些标签将被临时移除，直到GE再次激活。
        /// 这些标签同样用于RemoveGameplayEffectsWithTags的匹配。
        /// </summary>
        public GameplayTagSet GrantedTags;

        /// <summary>
        /// 当GameplayEffect成功应用后, 如果位于目标上的该GameplayEffect在其Asset Tags或Granted Tags中有任意一个本标签的话, 其就会自目标上移除.
        /// 匹配判断发生在：
        ///   1. Instant GE被应用时；
        ///   2. 非Instant GE每次被激活时；
        ///   3. Period Execution GE(非Instant GE中的PeriodExecution)的每个周期到期时。
        /// </summary>
        public GameplayTagSet RemoveGameplayEffectsWithTags;

        /// <summary>
        /// ApplicationRequiredTags和ApplicationImmunityTags是一对条件：
        /// 游戏效果能够应用于目标的前提是：
        ///   1. 目标必须拥有ApplicationRequiredTags中的所有标签；
        ///   2. 目标不能拥有ApplicationImmunityTags中的任意标签。
        /// </summary>
        public GameplayTagSet ApplicationRequiredTags;

        /// <summary>
        /// ApplicationRequiredTags和ApplicationImmunityTags是一对条件：
        /// 游戏效果能够应用于目标的前提是：
        ///   1. 目标必须拥有ApplicationRequiredTags中的所有标签；
        ///   2. 目标不能拥有ApplicationImmunityTags中的任意标签。
        /// </summary>
        public GameplayTagSet ApplicationImmunityTags;

        /// <summary>
        /// 游戏效果(GE)激活所需的标签集合。
        /// 该标签只作用于持续(Duration)和无限(Infinite)GameplayEffect, 对于Instant型GE没有意义.
        /// 一旦GameplayEffect应用后, 这些标签将决定GameplayEffect是开启还是关闭. GameplayEffect可以是关闭但仍然是应用的.
        /// 如果某个GameplayEffect由于不符合Ongoing Tag Requirements而关闭, 但是之后又满足需求了, 那么该GameplayEffect会重新打开并重新应用它的Modifier.
        /// 使用场景包括：
        ///   1. GE应用时，如果满足条件则激活GE，否则不执行任何操作；
        ///   2. 标签发生变化时，如果满足条件则激活GE，否则使GE失效。
        /// </summary>
        public GameplayTagSet OngoingRequiredTags;

        public GameplayEffectTagContainer(IGameplayEffectData data) : this(
            data.GetAssetTags(),
            data.GetGrantedTags(),
            data.GetApplicationRequiredTags(),
            data.GetOngoingRequiredTags(),
            data.GetRemoveGameplayEffectsWithTags(),
            data.GetApplicationImmunityTags()
        )
        {
        }

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

        public static GameplayEffectTagContainer CreateEmpty()
        {
            return new GameplayEffectTagContainer(
                Array.Empty<GameplayTag>(),
                Array.Empty<GameplayTag>(),
                Array.Empty<GameplayTag>(),
                Array.Empty<GameplayTag>(),
                Array.Empty<GameplayTag>(),
                Array.Empty<GameplayTag>()
            );
        }
    }
}
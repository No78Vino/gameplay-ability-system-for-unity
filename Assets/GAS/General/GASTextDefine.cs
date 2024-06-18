namespace GAS.General
{
    public static class GASTextDefine
    {
        public const string TITLE_SETTING = "设置";
        public const string TITLE_PATHS = "路径";
        public const string TITLE_BASE_INFO = "基本信息";
        public const string TITLE_DESCRIPTION = "描述";

        
        #region GASSettingAsset

        public const string TIP_CREATE_GEN_AscUtilCode =
            "<color=white><size=15>生成ASC拓展类之前，一定要保证Ability，AttributeSet的集合工具类已经生成。因为ASC拓展类依赖于此</size></color>";

        public const string TIP_CREATE_FOLDERS =
            "<color=white><size=15>如果你修改了EX-GAS的配置Asset路径,请点击这个按钮来确保所有子文件夹和基础配置文件正确生成。</size></color>";

        public const string LABEL_OF_CodeGeneratePath = "脚本生成路径";
        public const string LABEL_OF_GASConfigAssetPath = "配置文件Asset路径";
        public const string BUTTON_CheckAllPathFolderExist = " 检查子目录和基础配置";
        public const string BUTTON_GenerateAscExtensionCode = " 生成AbilitySystemComponentExtension类脚本";

        #endregion

        
        #region Tag

        public const string BUTTON_ExpandAllTag = "展开全部";
        public const string BUTTON_CollapseAllTag = "折叠全部";
        public const string BUTTON_AddTag = "添加Tag";
        public const string BUTTON_RemoveTag = "移除Tag";
        public const string BUTTON_GenTagCode = "生成TagLib";

        #endregion
        
        
        #region Attribute
        public const string TIP_Warning_EmptyAttribute =
            "<size=13><color=yellow><color=orange>Attribute名</color>不准为<color=red><b>空</b></color>! " +
            "Please check!</color></size>";
        public const string BUTTON_GenerateAttributeCollection = " 生成AttrLib";
        
        public const string TIP_Warning_DuplicatedAttribute =
            "<size=13><color=yellow>The <color=orange>Attribute名</color> 禁止 <color=red><b>重复</b></color>!\n" +
            "重复的Attributes名:<size=15><b><color=white> {0} </color></b></size>.</color></size>";
        #endregion


        #region AttributeSet

        public const string ERROR_DuplicatedAttribute = "<size=16><b>存在重复Attribute！</b></size>";
        public const string ERROR_Empty = "<size=16><b>AttributeSet至少要有一个Attribute！</b></size>";
        public const string ERROR_EmptyName = "<size=16><b>AttributeSet名不可以为空！</b></size>";
        public const string ERROR_InElements = "<size=16><b><color=orange>请先修复AttributeSet的提示错误!</color></b></size>";
        
        public const string ERROR_DuplicatedAttributeSet = "<size=16><b><color=orange>存在重复AttributeSet!\n" +
                                                           "<color=white> ->  {0}</color></color></b></size>";
        public const string BUTTON_GenerateAttributeSetCode = " 生成AttrSetLib";
        
        #endregion


        #region GameplayEffect
        
        public const string TIP_BASE_INFO="仅用于描述，方便理解。";
        public const string TIP_GE_POLICY="Instant=瞬时，Duration=持续性，Infinite=永久";
        public const string LABLE_GE_NAME = "效果名称";
        public const string LABLE_GE_DESCRIPTION = "效果描述";
        public const string TITLE_GE_POLICY="Gameplay Effect实施策略";
        public const string LABLE_GE_POLICY = "时限策略";
        public const string LABLE_GE_DURATION = "持续时间";
        public const string LABLE_GE_INTERVAL = "间隔时间";
        public const string LABLE_GE_EXEC = "间隔效果";
        public const string TITLE_GE_GrantedAbilities = "授予能力(Ability)";
        public const string TITLE_GE_MOD = "修改器Modifier";
        public const string TITLE_GE_TAG = "标签Tag";
        public const string TITLE_GE_CUE = "提示Cue";
      
        public const string TITLE_GE_TAG_AssetTags = "AssetTags - 该[游戏效果]自身的标签";
        public const string TIP_GE_TAG_AssetTags = "AssetTags: 标签用于描述[游戏效果]自身的特定属性，包括但不限于伤害、治疗、控制等效果类型。\n这些标签有助于区分和定义[游戏效果]的作用和表现。\n可配合RemoveGameplayEffectsWithTags食用。";
        public const string TITLE_GE_TAG_GrantedTags = "GrantedTags - 授予目标单位的标签";
        public const string TIP_GE_TAG_GrantedTags = "GrantedTags: 当[游戏效果]生效时，标签会被添加到目标单位上，并在[游戏效果]失效时移除。\n该标签对即时型（Instant）[游戏效果]的无效。";
        public const string TITLE_GE_TAG_ApplicationRequiredTags = "ApplicationRequiredTags - 应用该[游戏效果]的【全部】前提";
        public const string TIP_GE_TAG_ApplicationRequiredTags = "ApplicationRequiredTags: [游戏效果]的目标单位必须具备【所有】这些标签才能应用于目标单位。\n如果想表达【任一】标签不可作用于目标，应该使用ApplicationImmunityTags标签。";
        public const string TITLE_GE_TAG_OngoingRequiredTags = "OngoingRequiredTags - 激活该[游戏效果]的【全部】前提";
        public const string TIP_GE_TAG_OngoingRequiredTags = "OngoingRequiredTags: [游戏效果]的目标单位必须具备【全部】这些标签，否则该效果不会触发。\n一旦[游戏效果]被施加，如果目标单位在效果持续期间标签发生变化，导致不再具备【全部】这些标签，效果将失效；反之，如果满足条件，效果将被激活。\n该标签对即时型（Instant）[游戏效果]的无效。";
        public const string TITLE_GE_TAG_RemoveGameplayEffectsWithTags = "RemoveGameplayEffectsWithTags - 移除具有【任一】标签的[游戏效果]";
        public const string TIP_GE_TAG_RemoveGameplayEffectsWithTags = "RemoveGameplayEffectsWithTags: [游戏效果]的目标单位当前持有的所有[游戏效果]中，其AssetTags或GrantedTags中具有【任一】这些标签的[游戏效果]将被移除。";
        public const string TITLE_GE_TAG_ApplicationImmunityTags = "ApplicationImmunityTags - 无法应用于具有【任一】标签的目标";
        public const string TIP_GE_TAG_ApplicationImmunityTags = "ApplicationImmunityTags: 该[游戏效果]无法作用于拥有【任一】这些标签的目标单位。";
        
        public const string TITLE_GE_CUE_CueOnExecute = "CueOnExecute - 执行时触发";
        public const string TITLE_GE_CUE_CueDurational = "CueDurational - 存在时持续触发";
        public const string TITLE_GE_CUE_CueOnAdd = "CueOnAdd - 添加时触发";
        public const string TITLE_GE_CUE_CueOnRemove = "CueOnRemove - 移除时触发";
        public const string TITLE_GE_CUE_CueOnActivate = "CueOnActivate - 激活时触发";
        public const string TITLE_GE_CUE_CueOnDeactivate = "CueOnDeactivate - 失活时触发";
        
        public const string LABEL_GRANT_ABILITY = "授予能力";
        public const string LABEL_GRANT_ABILITY_LEVEL = "能力等级";
        public const string LABEL_GRANT_ABILITY_ACTIVATION_POLICY = "激活策略";
        public const string LABEL_GRANT_ABILITY_DEACTIVATION_POLICY = "失活策略";
        public const string LABEL_GRANT_ABILITY_REMOVE_POLICY = "移除策略";
        public const string TIP_GRANT_ABILITY_ACTIVATION_POLICY = "None = 不激活，需要用户手动调用ASC相关接口激活; " +
                                                                  "WhenAdded = 添加时就激活;" +
                                                                  "SyncWithEffect = 同步GE，GE激活时激活"; 
        public const string TIP_GRANT_ABILITY_DEACTIVATION_POLICY = "None = 无相关取消激活逻辑, 需要用户调用ASC取消激活; " +
                                                                  "SyncWithEffect = 同步GE，GE失活时取消激活";   
        public const string TIP_GRANT_ABILITY_REMOVE_POLICY = "None = 不移除能力;" +
                                                              "SyncWithEffect = 同步GE，GE移除时移除" +
                                                              "WhenEnd = 能力结束时，移除自身;" +
                                                              "WhenCancel = 能力取消时，移除自身;" +
                                                              "WhenCancelOrEnd = 能力取消或结束时，移除自身";
        
        public const string TITLE_GE_STACKING = "堆叠配置";
        public const string LABEL_GE_STACKING_CODENAME = "堆叠GE识别码";
        public const string LABEL_GE_STACKING_TYPE = "堆叠类型";
        public const string LABEL_GE_STACKING_COUNT = "堆叠上限";
        public const string LABEL_GE_STACKING_DURATION_REFRESH_POLICY = "持续时间刷新策略";
        public const string LABEL_GE_STACKING_PERIOD_RESET_POLICY = "周期重置策略";
        public const string LABEL_GE_STACKING_EXPIRATION_POLICY = "持续时间结束策略";
        public const string LABEL_GE_STACKING_DENY_OVERFLOW_APPLICATION = "溢出的GE不生效";
        public const string LABEL_GE_STACKING_CLEAR_STACK_ON_OVERFLOW = "溢出时清空堆叠";
        public const string LABEL_GE_STACKING_CLEAR_OVERFLOW_EFFECTS = "溢出时触发的GE";
        
        
        #endregion

        #region Ability

        public const string ABILITY_BASEINFO="基本信息";
        public const string TIP_UNAME =
            "<size=12><b><color=white><color=orange>U-Name非常重要!</color>" +
            "GAS会使用U-Name作为Ability的标识符。" +
            "所以你必须保证U-Name的唯一性。" +
            "别担心，生成AbilityLib时工具会提醒你这一点。</color></b></size>";
        public const string ABILITY_CD_TIME="冷却时长";
        public const string ABILITY_EFFECT_CD="冷却效果";
        public const string ABILITY_EFFECT_COST="消耗效果";
        public const string ABILITY_MANUAL_ENDABILITY = "手动结束能力";
        public const string BUTTON_CHECK_TIMELINE_ABILITY = "查看/编辑能力时间轴";

        #endregion
        
        #region ASC
        
        public const string TIP_ASC_BASEINFO="基本信息只用于描述这个ASC，方便策划阅读理解该ASC。";
        public const string ASC_BASE_TAG="固有标签";
        public const string ASC_BASE_ABILITY="固有能力";
        public const string ASC_AttributeSet="属性集";
        
        #endregion

        #region Watcher

        public const string TIP_WATCHER = "该窗口用于监视GAS运行状态,建议在调试GAS的角色能力，效果时打开该窗口。";
        public const string TIP_WATCHER_OnlyForGameRunning = 
            "<size=20><b><color=yellow>监视器只在游戏运行时可用.</color></b></size>";

        #endregion

        #region Gameplay Cue

        public const string CUE_ANIMATION_PATH = "动画机相对路径";
        public const string CUE_ANIMATION_INCLUDE_CHILDREN = "包括子节点";
        public const string CUE_ANIMATION_INCLUDE_CHILDREN_ANIMATOR_TIP = "在自身及孩子节点中查找动画机, 当你的动画机路径不能完全确定时(例如: 不同的皮肤节点不一致), 可勾选此项";
        public const string CUE_ANIMATION_STATE = "Animation State名";
        public const string CUE_ANIMATION_PATH_TIP = "为空表示物体根节点,示例：'A'=根节点下名为'A'的子物体,'A/B'='A'节点下名为'B'的子物体";

        public const string CUE_SOUND_EFFECT = "音效源";
        public const string CUE_ATTACH_TO_OWNER = "是否附加到Owner";
        
        public const string CUE_VFX_PREFAB = "特效预制体";
        public const string CUE_VFX_OFFSET = "特效偏移";
        public const string CUE_VFX_ROTATION = "特效旋转";
        public const string CUE_VFX_SCALE = "特效缩放";
        public const string CUE_VFX_ACTIVE_WHEN_ADDED = "是否在添加时就被激活";

        #endregion
    }
}
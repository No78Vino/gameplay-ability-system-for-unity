namespace GAS.General
{
    public static class GASTextDefine
    {
        public const string TITLE_SETTING = "设置";
        public const string TITLE_PATHS = "路径";
        public const string TITLE_BASEINFO = "基本信息";
        public const string TITLE_DESCRIPTION = "描述";

        
        #region GASSettingAsset

        public const string TIP_CREATE_GEN_AscUtilCode =
            "<color=white><size=15>生成ASC拓展类之前，一定要保证Ability，AttributeSet的集合工具类已经生成。因为ASC拓展类依赖于此</size></color>";

        public const string TIP_CREATE_FOLDERS =
            "<color=white><size=15>如果你修改了EX-GAS的配置Asset路径,请点击这个按钮来确保所有子文件夹和基础配置文件正确生成。</size></color>";

        public const string LABLE_OF_CodeGeneratePath = "脚本生成路径";
        public const string LABLE_OF_GASConfigAssetPath = "配置文件Asset路径";
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
        
        public const string TIP_BASEINFO="基本信息只用于描述这个GameplayEffect，方便策划阅读理解该Effect。";
        public const string TIP_GE_POLICY="None=空，Instant=瞬时，Duration=持续性，Infinite=永久";
        public const string LABLE_GE_NAME = "效果名";
        public const string TITLE_GE_POLICY="Gameplay Effect实施策略";
        public const string LABLE_GE_POLICY = "执行策略";
        public const string LABLE_GE_DURATION = "持续时间";
        public const string LABLE_GE_PER = "每";
        public const string LABLE_GE_EXEC = "执行";
        public const string TITLE_GE_GrantedAbilities = "授予能力(Ability)";
        public const string TITLE_GE_MOD = "修改器Modifier";
        public const string TITLE_GE_TAG = "标签Tag";
        public const string TITLE_GE_CUE = "提示Cue";
        
        #endregion

        #region Ability

        public const string ABILITY_BASEINFO="基本信息";
        public const string TIP_UNAME =
            "<size=12><b><color=white><color=orange>U-Name非常重要!</color>" +
            "GAS 会使用U-Name作为Ability的标识符。" +
            "所以你必须保证U-Name的唯一性。" +
            "别担心，生成AbilityLib时工具会提醒你这一点。</color></b></size>";
        public const string ABILITY_CD_TIME="CD时间";
        public const string ABILITY_EFFECT_CD="冷却CD";
        public const string ABILITY_EFFECT_COST="消耗";
        public const string ABILITY_MANUAL_ENDABILITY = "手动结束能力";
        public const string BUTTON_CHECK_TIMELINE_ABILITY = "查看/编辑能力时间轴";

        #endregion
        
        #region ASC
        
        public const string TIP_ASC_BASEINFO="基本信息只用于描述这个ASC，方便策划阅读理解该ASC。";
        public const string ASC_BASE_TAG="固有Tag";
        public const string ASC_BASE_ABILITY="固有能力";
        public const string ASC_AttributeSet="属性集";
        
        #endregion

        #region Watcher

        public const string TIP_WATCHER = "该窗口用于监视GAS运行状态,建议在调试GAS的角色能力，效果时打开该窗口。";
        public const string TIP_WATCHER_OnlyForGameRunning = 
            "<size=20><b><color=yellow>监视器只在游戏运行时可用.</color></b></size>";

        #endregion
    }
}
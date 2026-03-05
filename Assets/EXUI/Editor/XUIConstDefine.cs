namespace EXUI.Editor  
{  
    public static class XUIConstDefine  
    {  
        public const string EXUI_VERSION = "1.0";  
  
        /// <summary>  
        /// XUISettingAsset 的持久化路径（存放在 ProjectSettings 下，不进入 Assets 资产管理）  
        /// </summary>  
        public const string EXUI_SETTING_PATH = "ProjectSettings/XUISettingAsset.asset";  
  
        public const string MENU_ROOT = "EXTool/XUI/";  
        public const string MENU_SETTING = MENU_ROOT + "XUI中心管理器";  
        public const string MENU_WINDOW_CREATOR = MENU_ROOT + "窗口生成器";  
    }  
}
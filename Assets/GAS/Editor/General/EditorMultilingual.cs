namespace GAS.Editor.General
{
    public enum Language
    {
        CN,
        EN
    }
    
    public struct MultilingualText
    {
        public string CN;
        public string EN;

        public string Text
        {
get
{
    var language = Language.CN;
                switch (language)
                {
                    case Language.CN:
                        return CN;
                    case Language.EN:
                        return EN;
                    default:
                        return CN;
                }
            }
        }
    }
    
    public static class EditorMultilingual
    {
        public static readonly string TIP_CREATE_GEN_AscUtilCode = "???";
        public static readonly MultilingualText TIP_CREATE_GEN_AscUtilCode_Info = new MultilingualText
        {
            CN = "<color=white><size=15>生成ASC拓展类之前，一定要保证Ability，AttributeSet的集合工具类已经生成。因为ASC拓展类依赖于此</size></color>",
            EN = "<color=white><size=15>Before generating ASC extension classes, make sure that the collection utility classes of Ability and AttributeSet have been generated. Because ASC extension classes depend on this</size></color>"
        };
    }
}
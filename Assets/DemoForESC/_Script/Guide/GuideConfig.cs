using System.Collections.Generic;

namespace DemoForESC._Script
{
    public static class GuideConfig
    {
        public readonly static Dictionary<int, List<GuideInfo>> Data = new Dictionary<int, List<GuideInfo>>()
        {

        };
    }

    public struct GuideInfo
    {
        public string title;
        public string content;
    }
}
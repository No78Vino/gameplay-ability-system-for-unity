using System;
using System.Collections.Generic;

namespace DemoForESC._Script
{
    public static class GuideConfig
    {
        public readonly static Dictionary<int, List<GuideInfo>> Data = new Dictionary<int, List<GuideInfo>>()
        {
            [1] = new List<GuideInfo>()
            {
              new GuideInfo()
              {
                  title = "EX-HARD",
                  content = "你好欢迎来到EX-GAS演示训练场。"
              },
              new GuideInfo()
              {
                  title = "EX-HARD",
                  content = "接下来，我会为你进行简单的引导。"
              },
              new GuideInfo()
              {
                  title = "这是移动。",
                  content = "按键：W/A/S/D"
              },
            },
        };
    }

    public struct GuideInfo
    {
        public string title;
        public string content;
        public Action onBegin;
        public Func<bool> checkFunction;
    }
}
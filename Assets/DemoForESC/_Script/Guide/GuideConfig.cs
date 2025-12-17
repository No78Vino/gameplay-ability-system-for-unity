using System;
using System.Collections.Generic;

namespace DemoForESC._Script
{
    public static class GuideConfig
    {
        public readonly static Dictionary<int, List<GuideInfo>> Data = new Dictionary<int, List<GuideInfo>>()
        {
            [0] = new List<GuideInfo>()
            {
              new GuideInfo()
              {
                  title = "EX-HARD",
                  content = "你好欢迎来到EX-GAS演示训练场。",
                  LearningKey = GuideLearningKey.None,
                  limitFovRotation = true,
              },
              new GuideInfo()
              {
                  title = "EX-HARD",
                  content = "接下来，我会为你进行简单的引导。",
                  LearningKey = GuideLearningKey.None,
                  limitFovRotation = true,
              },
              new GuideInfo()
              {
                  title = "这是移动。",
                  content = "按键：W/A/S/D",
                  LearningKey = GuideLearningKey.Move,
                  limitFovRotation = false,
              },
              new GuideInfo()
              {
                  title = "这是奔跑。",
                  content = "按键：【W/A/S/D】 + 【左Shift】",
                  LearningKey = GuideLearningKey.Run,
                  limitFovRotation = false,
              },
              new GuideInfo()
              {
                  title = "这是攻击。",
                  content = "按键：鼠标左键",
                  LearningKey = GuideLearningKey.MeleeAttack,
                  limitFovRotation = false,
              },
              new GuideInfo()
              {
                  title = "这是【SPIDER 终结者】。",
                  content = "加油！",
                  limitFovRotation = false,
              },
            },
        };
    }

    public class GuideInfo
    {
        public string title;
        public string content;
        public bool limitFovRotation = false;
        public GuideLearningKey LearningKey;
        public Action onBegin;
        public Func<bool> checkFunction;
    }
}
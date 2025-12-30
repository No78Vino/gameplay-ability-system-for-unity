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
                  resetPosition = true,
              },
              new GuideInfo()
              {
                  title = "EX-HARD",
                  content = "接下来，我会为你进行简单的引导。",
                  LearningKey = GuideLearningKey.None,
                  limitFovRotation = true,
                  resetPosition = false,
              },
              new GuideInfo()
              {
                  title = "这是移动。",
                  content = "按键：W/A/S/D\n【请移动触碰视野内任意蓝色光圈】",
                  LearningKey = GuideLearningKey.Move,
                  limitFovRotation = false,
                  resetPosition = false,
                  GuideEvent = "GuideMove",
                  onBegin = GuideManager.I.OnGuideStart_Move,
                  onFinish = GuideManager.I.OnGuideFinish_Move,
              },
              new GuideInfo()
              {
                  title = "这是奔跑。",
                  content = "按键：【W/A/S/D】 + 【左Shift】\n【请奔跑触碰视野内任意绿色光圈】",
                  LearningKey = GuideLearningKey.Run,
                  limitFovRotation = false,
                  resetPosition = true,
                  GuideEvent = "GuideRun",
                  onBegin = GuideManager.I.OnGuideStart_Run,
                  onFinish = GuideManager.I.OnGuideFinish_Run,
              },
              new GuideInfo()
              {
                  title = "这是攻击。",
                  content = "按键：鼠标左键\n【请攻击橙色训练假人】",
                  LearningKey = GuideLearningKey.MeleeAttack,
                  limitFovRotation = false,
                  resetPosition = true,
              },
              new GuideInfo()
              {
                  title = "这是【SPIDER 终结者】。",
                  content = "加油！",
                  limitFovRotation = false,
                  resetPosition = true,
              },
            },
        };
    }

    public class GuideInfo
    {
        public string title;
        public string content;
        public bool limitFovRotation = false;
        public bool resetPosition = false;
        public GuideLearningKey LearningKey;
        public string GuideEvent;
        public Action onBegin;
        public Action onFinish;

        public bool isFinish { get; private set; }

        public void BeginGuide()
        {
            isFinish = false;
            onBegin?.Invoke();
            if (!string.IsNullOrEmpty(GuideEvent))
            {
                EventCenter.Register(GuideEvent, FinishGuide);
            }
        }

        private void FinishGuide(object obj = null)
        {
            isFinish = true;
            onFinish?.Invoke();
            if (!string.IsNullOrEmpty(GuideEvent))
            {
                EventCenter.Unregister(GuideEvent, FinishGuide);
            }
            GuideManager.I.ContinueGuide();
        }
    }
}
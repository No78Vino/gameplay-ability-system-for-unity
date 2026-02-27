using System.Collections.Generic;
using DemoForESC._Script;

public static class GuideConfig  
{  
    public static readonly List<GuideInfo> Type1Steps = new List<GuideInfo>  
    {  
        new GuideInfo  
        {  
            title = "EX-HARD",  
            content = "你好，欢迎来到 EX-GAS 演示训练场。",  
            LearningKey = GuideLearningKey.None,  
            limitFovRotation = true,  
            resetPosition = true,  
        },  
        new GuideInfo  
        {  
            title = "EX-HARD",  
            content = "接下来，我会为你进行简单的操作引导。\n【按任意键继续】",  
            LearningKey = GuideLearningKey.None,  
            limitFovRotation = true,  
            resetPosition = false,  
        },  
        new GuideInfo  
        {  
            title = "移动",  
            content = "按键：W / A / S / D\n【移动并触碰蓝色光圈完成引导】",  
            LearningKey = GuideLearningKey.Move,  
            limitFovRotation = false,  
            resetPosition = false,  
            GuideEvent = "GuideMove",  
        },  
        new GuideInfo  
        {  
            title = "奔跑",  
            content = "按键：W/A/S/D + 左 Shift\n【奔跑并触碰绿色光圈完成引导】",  
            LearningKey = GuideLearningKey.Run,  
            limitFovRotation = false,  
            resetPosition = true,  
            GuideEvent = "GuideRun",  
        },  
        new GuideInfo  
        {  
            title = "攻击",  
            content = "按键：E\n【攻击橙色训练假人完成引导】",  
            LearningKey = GuideLearningKey.MeleeAttack,  
            limitFovRotation = false,  
            resetPosition = true,  
            GuideEvent = "GuideAttack",  
        },  
        new GuideInfo  
        {  
            title = "SPIDER 终结者",  
            content = "引导完成！前方出现了一只蜘蛛，加油！\n【按任意键开始战斗】",  
            LearningKey = GuideLearningKey.None,  
            limitFovRotation = false,  
            resetPosition = true,  
        },  
    };  
}  
  
public class GuideInfo  
{  
    public string title;  
    public string content;  
    public bool limitFovRotation;  
    public bool resetPosition;  
    public GuideLearningKey LearningKey;  
    public string GuideEvent;  // 触发完成的事件名，None步骤为空  
    public bool isFinish { get; private set; }  
  
    private System.Action _onFinishCallback;  
  
    public void BeginGuide(System.Action onFinish)  
    {  
        isFinish = false;  
        _onFinishCallback = onFinish;  
        if (!string.IsNullOrEmpty(GuideEvent))  
            EventCenter.Register(GuideEvent, OnEventFinish);  
    }  
  
    private void OnEventFinish(object obj = null)  
    {  
        isFinish = true;  
        if (!string.IsNullOrEmpty(GuideEvent))  
            EventCenter.Unregister(GuideEvent, OnEventFinish);  
        _onFinishCallback?.Invoke();  
    }  
}
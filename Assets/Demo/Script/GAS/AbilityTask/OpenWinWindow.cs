using Demo.Script.UI;
using EXMaidForUI.Runtime.EXMaid;
using GAS.Runtime.Ability;

namespace Demo.Script.GAS.AbilityTask
{
    public class OpenWinWindow:InstantAbilityTask
    {
        public override void OnExecute()
        {
            // 打开胜利窗口
            XUI.M.OpenWindow<RetryWindow>();
            XUI.M.VM<RetryWindowVM>().SetRetryWindow(true);
        }
    }
}
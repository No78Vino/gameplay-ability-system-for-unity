using Loxodon.Framework.Extension;

namespace DemoForESC._Script.UI.ViewModel
{
    public class VMMainWindow : ViewModelCommon
    {
        public ObservableVariable<string> LabelPlayer = new();
        
        public override void OnShow()
        {
            base.OnShow();
            LabelPlayer.Value = "[玩家]蜘蛛机器人";
        }
    }
}
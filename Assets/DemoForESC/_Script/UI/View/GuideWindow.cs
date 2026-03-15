using DemoForESC._Script.UI.ViewModel;
using EXUI;
using Loxodon.Framework.Binding.Builder;
using UnityEngine.UI;

namespace DemoForESC._Script.UI.View
{
    public class GuideWindow : BaseView<VMGuideWindow>
    {
        public override UILayer Layer => UILayer.Modal;

        [BindOneWay("guide/label_guide", nameof(VMGuideWindow.Title), nameof(Text.text))]
        private Text _labelTitle;

        [BindOneWay("guide/label_content", nameof(VMGuideWindow.Content), nameof(Text.text))]
        private Text _labelContent;

        protected override void BindData()
        {
            // 仅保留 request 事件绑定  
            var bs = new BindingSet<GuideWindow, VMGuideWindow>(_bindingContext, this);
            bs.Bind(this).For(v => OnReceiveMessage).To(vm => vm.request);
            bs.Build();
        }
    }
}
using DemoForESC._Script.UI.ViewModel;
using DG.Tweening;
using EXUI;
using Loxodon.Framework.Binding.Builder;
using Loxodon.Framework.Interactivity;
using UnityEngine.UI;

namespace DemoForESC._Script.UI.View
{
    public class MaskWindow : BaseView<VMMaskWindow>
    {
        private Image _bg;
        public override UILayer Layer => UILayer.Mask;

        protected override void InitViewComponents()
        {
            _bg = GetComponentByNode<Image>("bg");
        }

        protected override void BindData()
        {
            var bindingSet = new BindingSet<MaskWindow, VMMaskWindow>(_bindingContext, this);
            bindingSet.Bind(this).For(v => OnReceiveMessage).To(vm => vm.request);
            bindingSet.Build();
        }

        protected override void OnReceiveMessage(object sender, InteractionEventArgs args)
        {
            base.OnReceiveMessage(sender, args);
            if (args.Context is not string message) return;
            switch (message)
            {
                case "FadeIn":
                {
                    _bg.DOFade(1, 1f).OnComplete(OnFadeInEnd);
                    break;
                }
                case "FadeOut":
                {
                    _bg.DOFade(0, 1f).OnComplete(OnFadeOutEnd);
                    break;
                }
                case "FadeInNoAnim":
                {
                    _bg.DOFade(1, 0).OnComplete(OnFadeInEnd);
                    break;
                }
                case "FadeOutNoAnim":
                {
                    _bg.DOFade(0, 0).OnComplete(OnFadeOutEnd);
                    break;
                }
            }
        }
        
        private void OnFadeInEnd()
        {
            _vm.InvokeOnOpen();
        }

        private void OnFadeOutEnd()
        {
            _vm.InvokeOnClose();
            Hide();
        }
    }
}
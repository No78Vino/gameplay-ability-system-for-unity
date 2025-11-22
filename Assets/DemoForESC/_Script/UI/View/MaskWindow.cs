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
            if (args.Context is string message)
                switch (message)
                {
                    case "FadeIn":
                    {
                        _bg.DOFade(1, 0.5f);
                        break;
                    }
                    case "FadeOut":
                    {
                        _bg.DOFade(0, 0.5f).OnComplete(Hide);
                        break;
                    }
                    case "FadeInNoAnim":
                    {
                        _bg.DOFade(1, 0);
                        break;
                    }
                    case "FadeOutNoAnim":
                    {
                        _bg.DOFade(0, 0).OnComplete(Hide);
                        break;
                    }
                }
        }
    }
}
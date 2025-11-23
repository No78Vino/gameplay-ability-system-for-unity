using DemoForESC._Script.UI.ViewModel;
using EXUI;
using Loxodon.Framework.Binding.Builder;
using Loxodon.Framework.Interactivity;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace DemoForESC._Script.UI.View
{
    public class MenuWindow : BaseView<VMMenuWindow>
    {
        public Text titleText;
        public Text infoText;
        public Text btnStartText;
        public Text btnGitRepoText;
        public Text btnQuitText;
        public Button btnStart;
        public Button btnGitRepo;
        public Button btnQuit;

        protected override void InitViewComponents()
        {
            // titleText = GetComponentByNode<TextMeshProUGUI>("Title");
            // infoText = GetComponentByNode<Text>("info");
            // btnStart = GetComponentByNode<Button>("Buttons/ButtonStart");
            // btnStartText = GetComponentByNode<TextMeshProUGUI>("Buttons/ButtonStart/Text (TMP)");
            // btnGitRepo = GetComponentByNode<Button>("Buttons/ButtonRepo");
            // btnGitRepoText = GetComponentByNode<TextMeshProUGUI>("Buttons/ButtonRepo/Text (TMP)");
            // btnQuit = GetComponentByNode<Button>("Buttons/ButtonQuit");
            // btnQuitText = GetComponentByNode<TextMeshProUGUI>("Buttons/ButtonQuit/Text (TMP)");
            btnStart.AddHoverListener(PointerEnterBtnStartHandle);
            btnGitRepo.AddHoverListener(PointerEnterBtnRepoHandle);
            btnQuit.AddHoverListener(PointerEnterBtnQuitHandle);
            btnStart.AddHoverOutListener(PointerExitHandle);
            btnGitRepo.AddHoverOutListener(PointerExitHandle);
            btnQuit.AddHoverOutListener(PointerExitHandle);
        }

        protected override void BindData()
        {
            var bindingSet = new BindingSet<MenuWindow, VMMenuWindow>(_bindingContext, this);
            bindingSet.Bind(this).For(v => OnReceiveMessage).To(vm => vm.request);
            bindingSet.Bind(titleText).For(v => v.text).To(vm => vm.title.Value).OneWay();
            bindingSet.Bind(infoText).For(v => v.text).To(vm => vm.info.Value).OneWay();

            bindingSet.Bind(btnStartText).For(v => v.text).To(vm => vm.btnStartText.Value).OneWay();
            bindingSet.Bind(btnGitRepoText).For(v => v.text).To(vm => vm.btnGitRepoText.Value).OneWay();
            bindingSet.Bind(btnQuitText).For(v => v.text).To(vm => vm.btnQuitText.Value).OneWay();

            bindingSet.Bind(btnStart).For(v => v.onClick).To(vm => vm.GameStart);
            bindingSet.Bind(btnGitRepo).For(v => v.onClick).To(vm => vm.OpenGitRepo);
            bindingSet.Bind(btnQuit).For(v => v.onClick).To(vm => vm.Quit);

            bindingSet.Build();
        }

        private void PointerEnterBtnStartHandle(PointerEventData data)
        {
            _vm.OnButtonStartHover();
        }

        private void PointerEnterBtnRepoHandle(PointerEventData data)
        {
            _vm.OnButtonGitRepoHover();
        }

        private void PointerEnterBtnQuitHandle(PointerEventData data)
        {
            _vm.OnButtonQuitHover();
        }
        
        private void PointerExitHandle(PointerEventData data)
        {
            _vm.OnHoverOut();
        }

        protected override void OnReceiveMessage(object sender, InteractionEventArgs args)
        {
            base.OnReceiveMessage(sender, args);
            if (args.Context is string msg)
            {
                if (msg == "close")
                    Hide();
            }
        }
    }
}
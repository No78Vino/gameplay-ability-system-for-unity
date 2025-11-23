using System;

namespace DemoForESC._Script.UI.ViewModel
{
    public class VMMaskWindow : ViewModelCommon
    {
        private Action _onClose;
        private Action _onOpen;

        public void MaskFadeOut(bool anim = true)
        {
            request.Raise(anim ? "FadeOut" : "FadeOutNoAnim");
        }

        public void MaskFadeIn(bool anim = true)
        {
            request.Raise(anim ? "FadeIn" : "FadeInNoAnim");
        }

        public void SetOnClose(Action onClose)
        {
            _onClose = onClose;
        }

        public void SetOnOpen(Action onOpen)
        {
            _onOpen = onOpen;
        }

        public void InvokeOnClose()
        {
            _onClose?.Invoke();
            _onClose = null;
        }

        public void InvokeOnOpen()
        {
            _onOpen?.Invoke();
            _onOpen = null;
        }
    }
}
namespace DemoForESC._Script.UI.ViewModel
{
    public class VMMaskWindow:ViewModelCommon
    {
        public void MaskFadeOut(bool anim = true)
        {
            request.Raise(anim?"FadeOut":"FadeOutNoAnim");
        }
        
        public void MaskFadeIn(bool anim = true)
        {
            request.Raise(anim?"FadeIn":"FadeInNoAnim");
        }
    }
}
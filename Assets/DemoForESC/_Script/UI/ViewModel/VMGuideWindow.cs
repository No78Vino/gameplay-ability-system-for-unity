using Loxodon.Framework.Extension;

namespace DemoForESC._Script.UI.ViewModel
{
    public class VMGuideWindow:ViewModelCommon
    {
        public ObservableVariable<string> Title { get; } = new();  
        public ObservableVariable<string> Content { get; } = new();  
  
        public void UpdateInfo(GuideInfo guideInfo)  
        {  
            Title.Value = guideInfo.title;  
            Content.Value = guideInfo.content;  
        }  
    }
}
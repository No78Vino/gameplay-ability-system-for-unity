using System.IO;
using Sirenix.Utilities;

namespace EXUI.Editor  
{  
    [GlobalConfig("Assets/EXUI/Editor/Resources")]  
    public class XUISettingAsset : GlobalConfig<XUISettingAsset>  
    {  
        public string ViewCodeOutputPath = "Assets/Scripts/UI/View";  
        
        public string ViewModelCodeOutputPath = "Assets/Scripts/UI/ViewModel"; 
        
        public string ViewNamespace = "UI.View";  
        
        public string ViewModelNamespace = "UI.ViewModel"; 
        
        // ✅ 这两个方法与 GlobalConfig/ScriptableSingleton 的选择无关，必须保留  
        public string GetViewCodePath(string windowName)  
            => Path.Combine(ViewCodeOutputPath, $"{windowName}.cs");  
  
        public string GetViewModelCodePath(string windowName)  
            => Path.Combine(ViewModelCodeOutputPath, $"VM{windowName}.cs");  
    } 
}
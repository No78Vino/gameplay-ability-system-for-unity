using DemoForESC._Script.UI.View;
using EXUI;

namespace DemoForESC._Script
{
    public class GuideManager
    {
        private static GuideManager _inst;
        
        // I For GuideManager
        public static GuideManager I => _inst ??= new GuideManager();

        private int _guideType = -1;
        private int _guideIndex = -1;

        public GuideInfo GuideInfo =>
            _guideType >= 0 && _guideIndex >= 0 ? GuideConfig.Data[_guideType][_guideIndex] : null;
        
        /// <summary>
        /// 1.禁止输入
        /// 2.启动当前引导允许的输入
        /// 3.弹出引导提示
        /// </summary>
        public void StartGuide()
        {
            _guideType = 0;
            _guideIndex = 0;
            TriggerGuide();
        }

        public void ContinueGuide()
        {
            if (_guideIndex + 1 >= GuideConfig.Data[_guideType].Count)
            {
                OnGuideEnd();
            }
            else
            {
                _guideIndex++;
                TriggerGuide();
            }
        }

        public void OnGuideEnd()
        {
            
        }

        private void TriggerGuide()
        {
            // 1.刷新 GuideWindow
            var w = XUI.M.OpenWindow<GuideWindow>();
            w.VM.UpdateInfo(GuideInfo);
            // 2.重置Player位置状态
            
            // 3.更新限制按键
        }
    }
}
namespace DemoForESC._Script
{
    public class GuideManager
    {
        private static GuideManager _inst;
        
        // I For GuideManager
        public static GuideManager I => _inst ??= new GuideManager();

        /// <summary>
        /// 1.禁止输入
        /// 2.启动当前引导允许的输入
        /// 3.弹出引导提示
        /// </summary>
        public void TriggerGuide()
        {
            
        }
    }
}
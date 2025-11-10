using GAS.Runtime;
using UnityEngine;

namespace DemoForESC._Script
{
    public class DemoLauncher : MonoBehaviour
    {
        private void Awake()
        {
            XLauncher.Launch();
            GASManager.Run();
            
            // 自身设置为不随场景切换而销毁
            DontDestroyOnLoad(gameObject);
            
            // 事件中心注册
            EventCenter.Register("LoadMainScene", _ => LoadMainScene());
        }
        
        private void LoadMainScene()
        {
            // 示例：加载主场景
            UnityEngine.SceneManagement.SceneManager.LoadScene("DemoForECS");
        }
    }
}
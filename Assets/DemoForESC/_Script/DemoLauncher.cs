using GAS.Runtime;
using UnityEngine;
using XYooAsset;
using YooAsset;

namespace DemoForESC._Script
{
    public class DemoLauncher : MonoBehaviour
    {
        private void Awake()
        {
            XLauncher.Launch();
            GASManager.Run();
            XYooAssetManager.Instance.Initialize("MainPackage");
            
            // 自身设置为不随场景切换而销毁
            DontDestroyOnLoad(gameObject);
            
            // 事件中心注册
            EventCenter.Register("LoadMainScene", _ => LoadMainScene());
        }
        
        private void LoadMainScene()
        {
            //加载主场景
            XYoo.LoadSceneAsync("DemoForECS",OnMainSceneLoaded);
        }

        private void OnMainSceneLoaded(SceneHandle sceneHandle)
        {
            // 主场景加载完成后的逻辑
            Debug.Log("主场景加载完成");
            // 1.加载菜单UI
            // 2.镜头布局
            // 2.场景资源加载
            
        }
    }
}
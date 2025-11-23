using DemoForESC._Script.UI;
using DemoForESC._Script.UI.View;
using DemoForESC._Script.UI.ViewModel;
using EXUI;
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
            XUI.Launch();
            XUI.M.RegisterViewPrefabPath(UIConfig.WindowPathMap,XYooAssetManager.Instance.LoadAssetSync<GameObject>);
            
            // 自身设置为不随场景切换而销毁
            DontDestroyOnLoad(gameObject);
            
            // 事件中心注册
            EventCenter.Register("LoadMainScene", _ => LoadMainScene());
        }
        
        private void LoadMainScene()
        {
            //加载主场景
            XYoo.LoadSingleSceneAsync("DemoForECS",OnMainSceneLoaded);
        }

        private void OnMainSceneLoaded(SceneHandle sceneHandle)
        {
            // 主场景加载完成后的逻辑
            Debug.Log("主场景加载完成");
            XUI.M.OpenWindow<MaskWindow>();
            var vmMaskWindow = XUI.M.VM<VMMaskWindow>();
            vmMaskWindow.SetOnOpen(LoadMenu);
            vmMaskWindow.MaskFadeIn(false);
        }

        /// <summary>
        /// 加载开始主菜单
        /// 1.加载MenuWindow
        /// 2.加载timeline所需的场景，动画等资源
        /// 3.设置好timeline所需参数
        /// 4.播放timeline
        /// 5.关闭MaskWindow
        /// </summary>
        private void LoadMenu()
        {
            // 1.加载MenuWindow
            XUI.M.OpenWindow<MenuWindow>();
            
            // 5.关闭MaskWindow
            XUI.M.VM<VMMaskWindow>().MaskFadeOut();
        }
    }
}
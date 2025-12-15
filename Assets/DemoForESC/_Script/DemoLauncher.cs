using DemoForESC._Script.UI;
using EXUI;
using GAS.Runtime;
using UnityEngine;
using XYooAsset;

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
            XUI.M.RegisterViewPrefabPath(UIConfig.WindowPathMap, XYooAssetManager.Instance.LoadAssetSync<GameObject>);

            // 自身设置为不随场景切换而销毁
            DontDestroyOnLoad(gameObject);

            // 事件中心注册
            EventCenter.Register("LoadMainScene", _ => GameManager.I.LoadMainScene());
            EventCenter.Register("StartGame", _ => GameManager.I.OnStartGameByMenu());
        }
    }
}
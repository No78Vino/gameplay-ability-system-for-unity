using DemoForESC._Script.UI;
using EXUI;
using GAS.Runtime;
using SimpleJSON;
using UnityEngine;
using XYooAsset;

namespace DemoForESC._Script
{
    public class DemoLauncher : MonoBehaviour
    {
        public const string GAME_CONF_DIR = "Assets/DemoForESC/Resources/Tables";

        private void Awake()
        {
            XLauncher.Launch();
            GASManager.Run();
            XYoo.Initialize("MainPackage");
            XYooAssetManager.Instance.RegisterPackageInitComplete(OnXYooAssetInitComplete);
            // 注册 EX-GAS 资源加载器 -> 桥接到 XYoo (YooAsset)  
            GASResourceLoader.Register(
                loadSync: (path, type) => XYooAssetManager.Instance.Package.LoadAssetSync(path, type).AssetObject,
                loadAsync: (path, type, onComplete) =>
                {
                    var handle = XYooAssetManager.Instance.Package.LoadAssetAsync(path, type);
                    handle.Completed += h => onComplete?.Invoke(h.AssetObject);
                },
                release: asset =>
                {
                    // YooAsset 通过 handle.Release() 管理引用计数  
                    // 如果需要精确管理，可以在此维护 handle 映射  
                }
            );

            XUI.Launch();
            //XUI.M.RegisterViewPrefabPath(UIConfig.WindowPathMap, XYooAssetManager.Instance.LoadAssetSync<GameObject>);
            XUI.M.RegisterConfig(new XUIConfigFromYooAsset(UIConfig.WindowPathMap));

            // 自身设置为不随场景切换而销毁
            DontDestroyOnLoad(gameObject);

            // 事件中心注册
            EventCenter.Register("LoadMainScene", _ => GameManager.I.LoadMainScene());
            EventCenter.Register("StartGame", _ => GameManager.I.OnStartGameByMenu());
        }

        private JSONNode GetConfig(string file)
        {
            var textAsset = XYooAssetManager.Instance.LoadAssetSync<TextAsset>($"{GAME_CONF_DIR}/{file}.json");
            return JSON.Parse(textAsset.text);
        }

        private void OnXYooAssetInitComplete()
        {
            XLauncher.InitConfigTables(GetConfig);
            XYooAssetManager.Instance.UnregisterPackageInitComplete(OnXYooAssetInitComplete);
        }
    }
}
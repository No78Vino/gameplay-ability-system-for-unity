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
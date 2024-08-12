using System;
using System.Collections;
using Com.LuisPedroFonseca.ProCamera2D;
using Demo.Script.Element;
using Demo.Script.Map;
using Demo.Script.UI;
using EXMaidForUI.Runtime.EXMaid;
using GAS.Runtime;
using GAS.RuntimeWithECS.Core;
using UnityEngine;

namespace Demo.Script
{
    public class Launcher : MonoBehaviour
    {
        [SerializeField] string prefixOfFguiPackagePath = "Assets/Game/FairyGUI/";
        [SerializeField] private GameObject playerPrefab;
        [SerializeField] private Vector3 playerSpawnPosition;
        public BossRoomManager bossRoomManager;
        public ProCamera2D proCamera2D;
        
        private void Start()
        {
            XUI.Launch(prefixOfFguiPackagePath,LoadResource);
            // GAS的cache初始化
            GasCache.CacheAttributeSetName(GAttrSetLib.TypeToName);
            StartGame();
        }

        private object LoadResource(string path, Type type)
        {
            var split = path.Split('/');
            int indexOfResources = Array.IndexOf(split, "Resources");
            string resourcePath = "";
            for (int i = indexOfResources + 1; i < split.Length; i++)
            {
                resourcePath += split[i];
                if (i != split.Length - 1)
                {
                    resourcePath += "/";
                }
            }
            resourcePath = resourcePath.Substring( 0, resourcePath.LastIndexOf('.'));
            var obj = Resources.Load(resourcePath, type);
            return obj;
        }


        #region GameManager

        public void StartGame()
        {
            XUI.M.OpenWindow<MainUI>();
            XUI.M.OpenWindow<MenuWindow>();
            
            GASManager.Initialize();
            GASManager.Run();
            ResetGameScene();
        }

        public void ResetGameScene()
        {
            // 还原Boss房配置
            bossRoomManager.ResetRoom();
            
            // 加载玩家
            var player = Instantiate(playerPrefab, playerSpawnPosition, Quaternion.identity);
            // 设置摄像机跟随
            proCamera2D.RemoveAllCameraTargets();
            proCamera2D.AddCameraTarget(player.transform);
            
            // 重置UI
            XUI.M.VM<MainUIVM>().ResetUI();
        }
        #endregion

    }
}
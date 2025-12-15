using Cinemachine;
using DemoForESC._Script.UI.View;
using DemoForESC._Script.UI.ViewModel;
using EXUI;
using GAS.Runtime;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;
using XYooAsset;
using YooAsset;

namespace DemoForESC._Script
{
    public class GameManager
    {
        private static GameManager _inst;
        
        // I For Instance
        public static GameManager I => _inst ??= new GameManager();

        public AbilitySystemCell GlobalAsc { get; } = new();

        private PlayableDirector _openingTimeline;

        public void LoadMainScene()
        {
            //加载主场景
            XYoo.LoadSingleSceneAsync("DemoForECS", OnMainSceneLoaded);
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
            // 2.加载timeline所需的场景，动画等资源
            var timelinePath = "Assets/DemoForESC/Resources/Timeline/opening";
            var playableDirector = TimelineHelper.CreateTimeline(timelinePath);
            _openingTimeline = playableDirector;
            var mainCameraBrain = Camera.main.GetComponent<CinemachineBrain>();
            var timeline = playableDirector.playableAsset as TimelineAsset;
            var track = timeline.GetTrackByName("Cinemachine Track");
            playableDirector.SetGenericBinding(track, mainCameraBrain);

            // 4. 播放Timeline（可选，根据需求决定是否自动播放）
            playableDirector.Play();

            // 5.关闭MaskWindow
            XUI.M.VM<VMMaskWindow>().MaskFadeOut();
        }
        
        public void OnStartGameByMenu()
        {
            if (_openingTimeline != null)
                _openingTimeline.gameObject.SetActive(false);

            XUI.M.OpenWindow<MainWindow>();
            // 启动引导,挂上引导类型
            GlobalAsc.AddFixedTag(XTag.State); //AddFixedTag(XTag.Guide_Type1);
        }
    }
}
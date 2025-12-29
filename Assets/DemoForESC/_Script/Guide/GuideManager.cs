using DemoForESC._Script.Controller;
using DemoForESC._Script.UI.View;
using EXUI;
using UnityEngine;
using XYooAsset;

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

        public bool IsInGuide => GuideInfo != null;

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
            GuideInfo.BeginGuide();
            // 1.刷新 GuideWindow
            var w = XUI.M.OpenWindow<GuideWindow>();
            w.VM.UpdateInfo(GuideInfo);
            // 2.重置Player位置状态
            GameManager.I.ResetPlayerStateToGuidePoint();
            // 3.锁定
        }

        #region 引导事件合集

        private GameObject _guideTargetMove;
        public void OnGuideStart_Move()
        {
            var prefab = XYoo.LoadAssetSync<GameObject>("Assets/DemoForESC/Resources/Prefabs/Guide/GuidePoints_Move.prefab");
            _guideTargetMove = Object.Instantiate(prefab);
        }
        
        public void OnGuideFinish_Move()
        {
            EasyInputController.Inst().SetBanInput(true);
            var player = GameManager.I.Player;
            player.StopMove();
            var w = XUI.M.OpenWindow<MaskWindow>();
            w.VM.SetOnOpen(()=>
            {
                player.StopMove();
                GameManager.I.ResetPlayerStateToGuidePoint();
                w.VM.MaskFadeOut();
            });
            w.VM.SetOnClose(()=>EasyInputController.Inst().SetBanInput(false));
            w.VM.MaskFadeIn();
            
            Object.Destroy(_guideTargetMove);
        }

        #endregion
    }
}
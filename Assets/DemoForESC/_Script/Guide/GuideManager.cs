using System.Collections.Generic;
using System.Threading.Tasks;
using DemoForESC._Script.Controller;
using DemoForESC._Script.UI.View;
using EXUI;
using Unity.VisualScripting;
using UnityEngine;
using XYooAsset;

namespace DemoForESC._Script
{
    public class GuideManager
    {
        private static GuideManager _inst;
        public static GuideManager I => _inst ??= new GuideManager();

        private int _guideIndex = -1;
        private List<GuideInfo> _steps;

        public GuideInfo GuideInfo =>
            _steps != null && _guideIndex >= 0 && _guideIndex < _steps.Count
                ? _steps[_guideIndex]
                : null;

        public bool IsInGuide => GuideInfo != null;

        // 引导中的场景对象  
        private GameObject _guideTarget;

        public void StartGuide()
        {
            _steps = GuideConfig.Type1Steps;
            _guideIndex = 0;
            TriggerGuide();
        }

        public void ContinueGuide()
        {
            _guideIndex++;
            if (_guideIndex >= _steps.Count)
                OnGuideEnd();
            else
                TriggerGuide();
        }

        private void TriggerGuide()
        {
            var info = GuideInfo;
            // 注册完成回调，由 GuideInfo 内部监听事件  
            info.BeginGuide(ContinueGuide);

            // 刷新 UI  
            var w = XUI.M.OpenWindow<GuideWindow>();
            w.VM.UpdateInfo(info);

            // 重置玩家位置  
            GameManager.I.ResetPlayerStateToGuidePoint();

            // 根据步骤类型生成引导对象  
            OnGuideStepBegin(info);
        }

        private void OnGuideStepBegin(GuideInfo info)
        {
            // 清理上一步的引导对象  
            if (_guideTarget != null)
            {
                Object.Destroy(_guideTarget);
                _guideTarget = null;
            }

            switch (info.LearningKey)
            {
                case GuideLearningKey.Move:
                    _guideTarget = Object.Instantiate(
                        XYoo.LoadAssetSync<GameObject>(
                            "Assets/DemoForESC/Resources/Prefabs/Guide/GuidePoints_Move.prefab"));
                    break;
                case GuideLearningKey.Run:
                    _guideTarget = Object.Instantiate(
                        XYoo.LoadAssetSync<GameObject>(
                            "Assets/DemoForESC/Resources/Prefabs/Guide/GuidePoints_Run.prefab"));
                    break;
                case GuideLearningKey.MeleeAttack:
                    _guideTarget = Object.Instantiate(
                        XYoo.LoadAssetSync<GameObject>(
                            "Assets/DemoForESC/Resources/Prefabs/Guide/GuidePoints_Attack.prefab"));
                    break;
            }
        }

        // 步骤完成后的过渡动画（统一处理）  
        public async void OnGuideStepFinishTransition(System.Action onTransitionDone)
        {
            EasyInputController.Inst().SetBanInput(true);
            GameManager.I.Player.StopMove();
            GameManager.I.Player.StopRun();

            var w = XUI.M.OpenWindow<MaskWindow>();
            w.VM.SetOnOpen(async () =>
            {
                await Task.Delay(800);
                GameManager.I.ResetPlayerStateToGuidePoint();
                var w2 = XUI.M.OpenWindow<MaskWindow>();
                w2.VM.SetOnClose(() =>
                {
                    EasyInputController.Inst().SetBanInput(false);
                    onTransitionDone?.Invoke();
                });
                w2.VM.MaskFadeOut();
            });
            w.VM.MaskFadeIn();
        }

        public void OnGuideEnd()
        {
            _guideIndex = -1;
            if (_guideTarget != null)
            {
                Object.Destroy(_guideTarget);
                _guideTarget = null;
            }

            var w = XUI.M.Windows<GuideWindow>();
            w.Hide();
            // 移除引导 Tag，通知 GameManager 引导结束  
            GameManager.I.OnGuideComplete();
        }
    }
}
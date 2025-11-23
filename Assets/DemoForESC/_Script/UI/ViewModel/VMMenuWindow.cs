using Loxodon.Framework.Extension;
using UnityEngine;
using UnityEngine.EventSystems;

namespace DemoForESC._Script.UI.ViewModel
{
    public class VMMenuWindow : ViewModelCommon
    {
        private const string exgasUrl = "https://github.com/No78Vino/gameplay-ability-system-for-unity";
        public ObservableVariable<string> btnGitRepoText = new();
        public ObservableVariable<string> btnQuitText = new();
        public ObservableVariable<string> btnStartText = new();
        public ObservableVariable<string> info = new();

        public ObservableVariable<string> title = new();

        public void GameStart()
        {
            // TODO: 开始游戏
        }

        public void Quit()
        {
            Application.Quit();
        }

        public void OpenGitRepo()
        {
            Application.OpenURL(exgasUrl);
        }

        public void OnButtonStartHover()
        {
            info.Value = "开始游玩EX-GAS 2.0 Demo";
        }

        public void OnButtonGitRepoHover()
        {
            info.Value = "访问EX-GAS 2.0的GitHub仓库";
        }

        public void OnButtonQuitHover()
        {
            info.Value = "退出游戏";
        }
        
        public void OnHoverOut() => info.Value = "";

        public override void OnShow()
        {
            base.OnShow();
            title.Value = "DEMO for \n EX-GAS 2.0";
            btnStartText.Value = "开始挑战";
            btnGitRepoText.Value = "浏览Git仓库";
            btnQuitText.Value = "退出游戏";
        }
    }
}
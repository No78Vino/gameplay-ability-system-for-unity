using UnityEngine;

namespace Demo.Script.UI
{
    public class MenuWindowVM:ViewModelCommon
    {
        private const string exgasUrl = "https://github.com/No78Vino/gameplay-ability-system-for-unity";
        
        public void Quit()
        {
            Application.Quit();
        }

        public void StartGame()
        {
            commonRequest.Raise("close");
        }

        public void VisitExgasGithub()
        {
            Application.OpenURL(exgasUrl);
        }
    }
}
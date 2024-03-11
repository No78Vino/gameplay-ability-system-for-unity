using UnityEngine;
using System.Collections;

namespace Com.LuisPedroFonseca.ProCamera2D.TopDownShooter
{
    public class GameOver : MonoBehaviour
    {
        public Canvas GameOverScreen;

        void Awake()
        {
            GameOverScreen.gameObject.SetActive(false);
        }

        public void ShowScreen()
        {
            GameOverScreen.gameObject.SetActive(true);
            Time.timeScale = 0;
        }

        public void PlayAgain()
        {
            Time.timeScale = 1;

            UnityEngine.SceneManagement.SceneManager.LoadScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().name);
        }
    }
}
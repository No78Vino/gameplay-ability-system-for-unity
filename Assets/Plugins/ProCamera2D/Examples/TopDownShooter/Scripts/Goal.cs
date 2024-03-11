using UnityEngine;
using System.Collections;

namespace Com.LuisPedroFonseca.ProCamera2D.TopDownShooter
{
    public class Goal : MonoBehaviour
    {
        public GameOver GameOverScreen;

        void OnTriggerEnter(Collider other)
        {
            GameOverScreen.ShowScreen();
        }
    }
}
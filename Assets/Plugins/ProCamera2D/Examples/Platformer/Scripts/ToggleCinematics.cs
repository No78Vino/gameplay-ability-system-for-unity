using UnityEngine;
using Com.LuisPedroFonseca.ProCamera2D;

namespace Com.LuisPedroFonseca.ProCamera2D.Platformer
{
    public class ToggleCinematics : MonoBehaviour
    {
        public ProCamera2DCinematics Cinematics;

        void OnGUI()
        {
            if (GUI.Button(new Rect(5, 5, 180, 30), (Cinematics.IsPlaying ? "Stop" : "Start") + " Cinematics"))
            {
                if (Cinematics.IsPlaying)
                    Cinematics.Stop();
                else
                    Cinematics.Play();
            }

            if (Cinematics.IsPlaying)
            {
                if (GUI.Button(new Rect(195, 5, 40, 30), ">"))
                {
                    Cinematics.GoToNextTarget();
                }
            }
        }
    }
}
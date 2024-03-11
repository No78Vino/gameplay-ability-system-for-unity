using UnityEngine;
using System.Collections;
using UnityEngine.UI;

namespace Com.LuisPedroFonseca.ProCamera2D.TopDownShooter
{
    public class SwitchCameraProjection : MonoBehaviour
    {
        public string _cameraMode;

        void Awake()
        {
            Switch();
        }

        void OnGUI()
        {
            if (GUI.Button(new Rect(Screen.width - 210, 10, 200, 30), "Switch to " + _cameraMode + " mode"))
            {
                PlayerPrefs.SetInt("orthoCamera", Camera.main.orthographic ? 0 : 1);
                Switch();
            }
        }

        void Switch()
        {
            Camera.main.orthographic = PlayerPrefs.GetInt("orthoCamera", 0) == 1;

            _cameraMode = Camera.main.orthographic ? "perspective" : "orthographic";
        }
    }
}
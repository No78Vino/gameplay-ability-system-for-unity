using UnityEngine;
using System.Collections;

namespace Com.LuisPedroFonseca.ProCamera2D
{
    public class DollyZoomExample : MonoBehaviour
    {
        [Range(0.1f, 179.9f)]
        public float TargetFOV = 30f;

        [Range(0f, 10f)]
        public float Duration = 2f;

        public EaseType EaseType;

        [Range(-1f, 1f)]
        public float ZoomAmount = -.2f;

        void OnGUI()
        {
            GUI.Label(new Rect(5, 5, 100, 30), "Target FOV", new GUIStyle(){});
            TargetFOV = GUI.HorizontalSlider(new Rect(100, 5, 100, 30), TargetFOV, .1f, 179.9f);

            GUI.Label(new Rect(5, 35, 100, 30), "Duration", new GUIStyle(){});
            Duration = GUI.HorizontalSlider(new Rect(100, 35, 100, 30), Duration, 0f, 10f);

            GUI.Label(new Rect(5, 65, 100, 30), "Zoom Amount", new GUIStyle(){});
            ZoomAmount = GUI.HorizontalSlider(new Rect(100, 65, 100, 30), ZoomAmount, -1f, 1f);

            if (GUI.Button(new Rect(5, 95, 150, 30), ("Dolly Zoom")))
            {
                ProCamera2D.Instance.DollyZoom(TargetFOV, Duration, EaseType);
                ProCamera2D.Instance.Zoom(ZoomAmount, Duration, EaseType);
            }
        }
    }
}
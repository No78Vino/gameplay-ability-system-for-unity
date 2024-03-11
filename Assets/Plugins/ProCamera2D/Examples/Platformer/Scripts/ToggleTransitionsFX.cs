using UnityEngine;
using Com.LuisPedroFonseca.ProCamera2D;

namespace Com.LuisPedroFonseca.ProCamera2D.Platformer
{
    public class ToggleTransitionsFX : MonoBehaviour
    {
        void OnGUI()
        {
            if (GUI.Button(new Rect(5, 5, 180, 30), ("Transition Enter")))
            {
                ProCamera2DTransitionsFX.Instance.TransitionEnter();
            }

            if (GUI.Button(new Rect(5, 45, 180, 30), ("Transition Exit")))
            {
                ProCamera2DTransitionsFX.Instance.TransitionExit();
            }
        }
    }
}
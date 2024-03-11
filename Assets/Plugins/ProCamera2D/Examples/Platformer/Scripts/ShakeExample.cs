using UnityEngine;

namespace Com.LuisPedroFonseca.ProCamera2D
{
    public class ShakeExample : MonoBehaviour
    {
        bool _constantShaking;

        void OnGUI()
        {
            if (GUI.Button(new Rect(5, 5, 150, 30), "Shake"))
            {
                var shakePreset = ProCamera2DShake.Instance.ShakePresets[Random.Range(0, ProCamera2DShake.Instance.ShakePresets.Count)];
                Debug.Log("Shake: " + shakePreset.name);

                ProCamera2DShake.Instance.Shake(shakePreset);
            }

            if (GUI.Button(new Rect(5, 45, 150, 30), _constantShaking ? "Stop Constant Shake" : "Constant Shake"))
            {
                if (_constantShaking)
                {
                    _constantShaking = false;
                    ProCamera2DShake.Instance.StopConstantShaking();
                }
                else
                {
                    _constantShaking = true;

                    var constantShakePreset = ProCamera2DShake.Instance.ConstantShakePresets[Random.Range(0, ProCamera2DShake.Instance.ConstantShakePresets.Count)];
                    Debug.Log("ConstantShake: " + constantShakePreset.name);

                    ProCamera2DShake.Instance.ConstantShake(constantShakePreset);
                }
            }
        }
    }
}
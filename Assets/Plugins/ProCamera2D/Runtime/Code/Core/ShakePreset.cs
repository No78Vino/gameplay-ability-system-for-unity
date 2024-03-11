using UnityEngine;

namespace Com.LuisPedroFonseca.ProCamera2D
{
    [System.Serializable]
    [CreateAssetMenu(menuName = "ProCamera2D/Shake Preset")]
    public class ShakePreset : ScriptableObject
    {
        public Vector3 Strength = new Vector2(10, 10);

        [Range(.02f, 3f)]
        public float Duration = .5f;

        [Range(1, 100)]
        public int Vibrato = 10;

        [Range(0f, 1f)]
        public float Randomness = .1f;

        [Range(0f, .5f)]
        public float Smoothness = .1f;

        public bool UseRandomInitialAngle = true;

        [Range(0f, 360f)]
        public float InitialAngle;

        public Vector3 Rotation;

        public bool IgnoreTimeScale;
    }
}
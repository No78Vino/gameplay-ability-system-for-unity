using UnityEngine;
using System.Collections;

namespace Com.LuisPedroFonseca.ProCamera2D.TopDownShooter
{
    public class ObjectMove : MonoBehaviour
    {
        public float Amplitude = 1;
        public float Frequency = 1;

        Transform _transform;

        void Awake()
        {
            _transform = transform;
        }

        void LateUpdate()
        {
            _transform.position += Amplitude * (Mathf.Sin(2 * Mathf.PI * Frequency * Time.time) - Mathf.Sin(2 * Mathf.PI * Frequency * (Time.time - Time.deltaTime))) * Vector3.up;
        }
    }
}
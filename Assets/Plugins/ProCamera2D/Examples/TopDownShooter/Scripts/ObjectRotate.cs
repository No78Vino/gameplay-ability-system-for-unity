using UnityEngine;
using System.Collections;

namespace Com.LuisPedroFonseca.ProCamera2D.TopDownShooter
{
    public class ObjectRotate : MonoBehaviour
    {
        public Vector3 Rotation = Vector3.one;

        Transform _transform;

        void Awake()
        {
            _transform = transform;
        }

        void LateUpdate()
        {
            _transform.Rotate(Rotation);
        }
    }
}
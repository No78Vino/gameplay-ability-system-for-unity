using UnityEngine;
using System.Collections;

namespace Com.LuisPedroFonseca.ProCamera2D.TopDownShooter
{
    public class RotateTowardsMouse : MonoBehaviour
    {
        public float Ease = .15f;

        Transform _transform;

        void Awake()
        {
            _transform = transform;
        }

        void Update()
        {
            var mouse = Input.mousePosition;
            var screenPoint = Camera.main.WorldToScreenPoint(_transform.localPosition);
            var offset = new Vector2(mouse.x - screenPoint.x, mouse.y - screenPoint.y);
            var angle = Mathf.Atan2(offset.y, offset.x) * Mathf.Rad2Deg;
            _transform.rotation = Quaternion.Slerp(_transform.rotation, Quaternion.Euler(0, -angle, 0), Ease);
        }
    }
}
using UnityEngine;
using System.Collections;

namespace Com.LuisPedroFonseca.ProCamera2D.TopDownShooter
{
    public class Bullet : MonoBehaviour
    {
        public float BulletDuration = 1f;
        public float BulletSpeed = 50f;
        public float SkinWidth = .1f;

        public LayerMask CollisionMask;

        public float BulletDamage = 10f;

        Transform _transform;

        RaycastHit _raycastHit;
        Vector2 _collisionPoint;

        float _startTime;
        bool _exploding;
        Vector3 _lastPos;

        void Awake()
        {
            _transform = this.transform;
        }

        void OnEnable()
        {
            _exploding = false;
            _startTime = Time.time;
        }

        void Update()
        {
            if (_exploding)
                return;

            _lastPos = _transform.position;
            _transform.Translate(Vector3.right * BulletSpeed * Time.deltaTime);

            if(Physics.Raycast(_lastPos, _transform.position - _lastPos, out _raycastHit, (_lastPos - _transform.position).magnitude + SkinWidth, CollisionMask))
            {
                _collisionPoint = _raycastHit.point;

                _transform.up = _raycastHit.normal;

                Collide();
            }

            if (Time.time - _startTime > BulletDuration)
                Disable();
        }

        void Collide()
        {
            _exploding = true;
            _transform.position = _collisionPoint;

            _raycastHit.collider.SendMessageUpwards("Hit", BulletDamage, SendMessageOptions.DontRequireReceiver);

            Disable();
        }

        void Disable()
        {
            gameObject.SetActive(false);
        }
    }
}
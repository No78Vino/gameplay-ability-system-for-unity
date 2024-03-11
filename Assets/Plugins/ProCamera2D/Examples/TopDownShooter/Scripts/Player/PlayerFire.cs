using UnityEngine;
using System.Collections;

namespace Com.LuisPedroFonseca.ProCamera2D.TopDownShooter
{
    public class PlayerFire : MonoBehaviour
    {
        public Pool BulletPool;
        public Transform WeaponTip;

        public float FireRate = .3f;

        public float FireShakeForce = .2f;
        public float FireShakeDuration = .05f;

        Transform _transform;

        void Awake()
        {
            _transform = transform;
        }

        void Update()
        {
            if (Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(0))
            {
                StartCoroutine(Fire());
            }
        }

        IEnumerator Fire()
        {
            while (Input.GetKey(KeyCode.Space) || Input.GetMouseButton(0))
            {
                var bullet = BulletPool.nextThing; 
                bullet.transform.position = WeaponTip.position;
                bullet.transform.rotation = _transform.rotation;

                var angle = _transform.rotation.eulerAngles.y - 90;
                var radians = angle * Mathf.Deg2Rad;
                var vForce = new Vector2((float)Mathf.Sin(radians), (float)Mathf.Cos(radians)) * FireShakeForce;

                ProCamera2DShake.Instance.ApplyShakesTimed(new Vector2[]{ vForce }, new Vector3[]{Vector3.zero}, new float[]{ FireShakeDuration });

                yield return new WaitForSeconds(FireRate);
            }
        }
    }
}
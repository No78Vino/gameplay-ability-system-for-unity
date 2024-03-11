using UnityEngine;
using System.Collections;
using UnityEngine.AI;

namespace Com.LuisPedroFonseca.ProCamera2D.TopDownShooter
{
    public class EnemyAttack : MonoBehaviour
    {
        public float RotationSpeed = 2f;

        public Pool BulletPool;
        public Transform WeaponTip;
        public float FireRate = .3f;
        public float FireAngleRandomness = 10f;

        bool _hasTarget;
        Transform _target;
        NavMeshAgent _navMeshAgent;

        Transform _transform;

        void Awake()
        {
            _transform = transform;

            _navMeshAgent = this.GetComponentInChildren<NavMeshAgent>();
        }

        public void Attack(Transform target)
        {
            _navMeshAgent.updateRotation = false;
            _target = target;
            _hasTarget = true;

            StartCoroutine(LookAtTarget());
            StartCoroutine(FollowTarget());
            StartCoroutine(Fire());
        }

        public void StopAttack()
        {
            _navMeshAgent.updateRotation = true;
            _hasTarget = false;
        }

        IEnumerator LookAtTarget()
        {
            while (_hasTarget)
            {
                var lookAtPos = new Vector3(_target.position.x, _transform.position.y, _target.position.z);

                var diff = lookAtPos - _transform.position;
                var newRotation = Quaternion.LookRotation(diff, Vector3.up);

                _transform.rotation = Quaternion.Slerp(_transform.rotation, newRotation, RotationSpeed * Time.deltaTime);

                yield return null;
            }
        }

        IEnumerator FollowTarget()
        {
            while (_hasTarget)
            {
                var rnd = Random.insideUnitCircle;
                _navMeshAgent.destination = _target.position - (_target.position - _transform.position).normalized * 5f + new Vector3(rnd.x, 0, rnd.y);

                yield return null;
            }
        }

        IEnumerator Fire()
        {
            while (_hasTarget)
            {
                var bullet = BulletPool.nextThing; 
                bullet.transform.position = WeaponTip.position;
                bullet.transform.rotation = _transform.rotation * Quaternion.Euler(new Vector3(0, -90 + Random.Range(-FireAngleRandomness, FireAngleRandomness), 0));

                yield return new WaitForSeconds(FireRate);
            }
        }

        public static Vector2 RandomOnUnitCircle2(float radius) 
        {
            Vector2 randomPointOnCircle = Random.insideUnitCircle;
            randomPointOnCircle.Normalize();
            randomPointOnCircle *= radius;
            return randomPointOnCircle;
        }
    }
}
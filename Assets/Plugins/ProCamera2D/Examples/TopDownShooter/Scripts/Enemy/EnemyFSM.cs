using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Com.LuisPedroFonseca.ProCamera2D.TopDownShooter
{
    [RequireComponent(typeof(EnemySight))]
    [RequireComponent(typeof(EnemyAttack))]
    [RequireComponent(typeof(EnemyWander))]
    public class EnemyFSM : MonoBehaviour
    {
        public int Health = 100;

        public Color AttackColor = Color.red;

        public DoorKey Key;

        EnemySight _sight;
        EnemyAttack _attack;
        EnemyWander _wander;

        Renderer[] _renderers;
        Color _originalColor;
        Color _currentColor;

        void Awake()
        {
            _sight = GetComponent<EnemySight>();
            _attack = GetComponent<EnemyAttack>();
            _wander = GetComponent<EnemyWander>();

            _renderers = GetComponentsInChildren<Renderer>();
            _originalColor = _renderers[0].material.color;
            _currentColor = _originalColor;

            _sight.OnPlayerInSight += OnPlayerInSight;
            _sight.OnPlayerOutOfSight += OnPlayerOutOfSight;

            if(Key != null)
                Key.gameObject.SetActive(false);
        }

        void Start()
        {
            _wander.StartWandering();
        }

        void OnDestroy()
        {
            _sight.OnPlayerInSight -= OnPlayerInSight;
            _sight.OnPlayerOutOfSight -= OnPlayerOutOfSight;
        }

        void Hit(int damage)
        {
            if (Health <= 0)
                return;
            
            Health -= damage;

            StartCoroutine(HitAnim());

            if (Health <= 0)
            {
                Die();
            }
        }

        IEnumerator HitAnim()
        {
            Colorize(Color.white);

            yield return new WaitForSeconds(.05f);
            
            Colorize(_currentColor);
        }

        void OnPlayerInSight (Transform obj)
        {
            _wander.StopWandering();
            _attack.Attack(obj);

            ProCamera2D.Instance.AddCameraTarget(this.transform);

            _currentColor = AttackColor;
            Colorize(_currentColor);
        }

        void OnPlayerOutOfSight ()
        {
            _wander.StartWandering();
            _attack.StopAttack();

            ProCamera2D.Instance.RemoveCameraTarget(this.transform, 2);

            _currentColor = _originalColor;
            Colorize(_currentColor);
        }

        void Colorize(Color color)
        {
            for (int i = 0; i < _renderers.Length; i++)
            {
                _renderers[i].material.color = color;
            }
        }

        void DropLoot()
        {
            if (Key != null)
            {
                Key.gameObject.SetActive(true);
                Key.transform.position = transform.position + new Vector3(0, 3, 0);
            }
        }

        void Die()
        {
            ProCamera2DShake.Instance.Shake("SmallExplosion");

            OnPlayerOutOfSight();

            DropLoot();

            Destroy(gameObject, .2f);
        }
    }
}
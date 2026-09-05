using UnityEngine;

namespace EXProceduralMachine
{
    /// <summary>
    ///     可被 <see cref="Gun"/> 命中的目标接口。
    /// </summary>
    public interface IDamageable
    {
        void TakeDamage(float amount);
    }

    /// <summary>
    ///     简易命中式（Hitscan）枪械：按射速发射射线，命中后结算伤害，可选命中特效与后坐表现。
    /// </summary>
    public class Gun : MonoBehaviour
    {
        [Tooltip("枪口（发射起点与朝向）")]
        public Transform muzzle;

        [Tooltip("每秒射速")]
        public float fireRate = 8f;

        [Tooltip("最大射程")]
        public float range = 100f;

        [Tooltip("单发伤害")]
        public float damage = 10f;

        [Tooltip("命中检测 Layer")]
        public LayerMask hitMask = ~0;

        [Tooltip("命中特效预制体（可选）")]
        public GameObject impactEffect;

        [Tooltip("后坐强度（0~1）")]
        public float recoilStrength = 0.15f;

        [Tooltip("后坐恢复速度")]
        public float recoilRecovery = 4f;

        private float _cooldown;
        private float _recoilAmount;
        private Quaternion _baseRotation;

        public bool CanFire => _cooldown <= 0f;

        private void Start()
        {
            if (muzzle != null)
                _baseRotation = muzzle.localRotation;
        }

        private void Update()
        {
            if (_cooldown > 0f)
                _cooldown -= Time.deltaTime;

            // 后坐恢复
            _recoilAmount = Mathf.MoveTowards(_recoilAmount, 0f, Time.deltaTime * recoilRecovery);
            if (muzzle != null)
                muzzle.localRotation = _baseRotation * Quaternion.Euler(-_recoilAmount * 90f, 0f, 0f);
        }

        /// <summary>开火；冷却中返回 false。</summary>
        public bool Fire()
        {
            if (!CanFire)
                return false;

            _cooldown = 1f / Mathf.Max(0.001f, fireRate);

            var origin = muzzle != null ? muzzle.position : transform.position;
            var direction = muzzle != null ? muzzle.forward : transform.forward;

            if (Physics.Raycast(origin, direction, out var hit, range, hitMask))
            {
                ApplyDamage(hit.collider, damage);
                if (impactEffect != null)
                    Instantiate(impactEffect, hit.point, Quaternion.LookRotation(hit.normal));
            }

            _recoilAmount = Mathf.Min(1f, _recoilAmount + recoilStrength);
            return true;
        }

        private static void ApplyDamage(Collider collider, float amount)
        {
            var damageable = collider != null ? collider.GetComponentInParent<IDamageable>() : null;
            damageable?.TakeDamage(amount);
        }
    }
}
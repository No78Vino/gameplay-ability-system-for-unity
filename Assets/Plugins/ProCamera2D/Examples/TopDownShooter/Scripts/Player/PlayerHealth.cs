using UnityEngine;
using System.Collections;

namespace Com.LuisPedroFonseca.ProCamera2D.TopDownShooter
{
    public class PlayerHealth : MonoBehaviour
    {
        public int Health = 100;

        Renderer[] _renderers;
        Color _originalColor;

        void Awake()
        {
            _renderers = GetComponentsInChildren<Renderer>();
            _originalColor = _renderers[0].material.color;
        }

        void Hit(int damage)
        {
            Health -= damage;

            StartCoroutine(HitAnim());

            if (Health <= 0)
            {
                // Do something here
            }
        }

        IEnumerator HitAnim()
        {
            ProCamera2DShake.Instance.Shake("PlayerHit");

            for (int i = 0; i < _renderers.Length; i++)
            {
                _renderers[i].material.color = Color.white;
            }

            yield return new WaitForSeconds(.05f);

            for (int i = 0; i < _renderers.Length; i++)
            {
                _renderers[i].material.color = _originalColor;
            }
        }
    }
}
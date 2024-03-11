using UnityEngine;
using System;
using System.Collections;

namespace Com.LuisPedroFonseca.ProCamera2D.TopDownShooter
{
    public class EnemySight : MonoBehaviour
    {
        public Action<Transform> OnPlayerInSight;
        public Action OnPlayerOutOfSight;

        public float RefreshRate = 1f;
        public float fieldOfViewAngle = 110f;
        public float ViewDistance = 30f;
        public bool playerInSight;
        public Transform player;

        public LayerMask LayerMask;

        RaycastHit _hit;

        void Awake()
        {
            RefreshRate += UnityEngine.Random.Range(-RefreshRate * .2f, RefreshRate * .2f);
        }

        IEnumerator Start()
        {
            while (true)
            {
                Vector3 direction = player.position - transform.position;
                float angle = Vector3.Angle(direction, transform.forward);

                if (angle < fieldOfViewAngle * 0.5f &&
                    Physics.Raycast(transform.position + transform.up, direction.normalized, out _hit, ViewDistance, LayerMask) &&
                    _hit.collider.transform.GetInstanceID() == player.GetInstanceID())
                {
                    if (!playerInSight)
                    {
                        playerInSight = true;

                        if (OnPlayerInSight != null)
                            OnPlayerInSight(_hit.collider.transform);
                    }
                }
                else
                {
                    if (playerInSight)
                    {
                        playerInSight = false;

                        if (OnPlayerOutOfSight != null)
                            OnPlayerOutOfSight();
                    }
                }

                yield return new WaitForSeconds(RefreshRate);
            }
        }
    }
}
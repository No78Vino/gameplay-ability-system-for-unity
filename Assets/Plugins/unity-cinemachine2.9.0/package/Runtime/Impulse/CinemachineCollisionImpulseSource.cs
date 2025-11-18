#if !UNITY_2019_3_OR_NEWER
#define CINEMACHINE_PHYSICS
#define CINEMACHINE_PHYSICS_2D
#endif

using Cinemachine.Utility;
using UnityEngine;

namespace Cinemachine
{
#if !(CINEMACHINE_PHYSICS || CINEMACHINE_PHYSICS_2D)
    // Workaround for Unity scripting bug
    [AddComponentMenu("")] // Hide in menu
    public class CinemachineCollisionImpulseSource : CinemachineImpulseSource {}
#else
    /// <summary>
    /// Generate an Impulse Event this object's Collider collides with something
    /// or its trigger zone is entered.
    ///
    /// This component should be attached to a GameObject with a Collider or a Collider2D.
    /// Objects colliding with this (or entering its trigger zone if it's a trigger) will be
    /// filtered according to the layer and tag settings defined here, and if they
    /// pass the filter, they will cause an impulse event to be generated.
    /// </summary>
    [DocumentationSorting(DocumentationSortingAttribute.Level.UserRef)]
    [SaveDuringPlay]
    [HelpURL(Documentation.BaseURL + "manual/CinemachineCollisionImpulseSource.html")]
    public class CinemachineCollisionImpulseSource : CinemachineImpulseSource
    {
        /// <summary>Only collisions with objects on these layers will generate Impulse events.</summary>
        [Header("Trigger Object Filter")]
        [Tooltip("Only collisions with objects on these layers will generate Impulse events")]
        public LayerMask m_LayerMask = 1;

        /// <summary>No Impulse evemts will be generated for collisions with objects having these tags</summary>
        [TagField]
        [Tooltip("No Impulse evemts will be generated for collisions with objects having these tags")]
        public string m_IgnoreTag = string.Empty;

        /// <summary>If checked, signal direction will be affected by the direction of impact</summary>
        [Header("How To Generate The Impulse")]
        [Tooltip("If checked, signal direction will be affected by the direction of impact")]
        public bool m_UseImpactDirection = false;

        /// <summary>If checked, signal amplitude will be multiplied by the mass of the impacting object</summary>
        [Tooltip("If checked, signal amplitude will be multiplied by the mass of the impacting object")]
        public bool m_ScaleImpactWithMass = false;

        /// <summary>If checked, signal amplitude will be multiplied by the speed of the impacting object</summary>
        [Tooltip("If checked, signal amplitude will be multiplied by the speed of the impacting object")]
        public bool m_ScaleImpactWithSpeed = false;

#if CINEMACHINE_PHYSICS
        Rigidbody mRigidBody;
#endif
#if CINEMACHINE_PHYSICS_2D
        Rigidbody2D mRigidBody2D;
#endif

        private void Start()
        {
#if CINEMACHINE_PHYSICS
            mRigidBody = GetComponent<Rigidbody>();
#endif
#if CINEMACHINE_PHYSICS_2D
            mRigidBody2D = GetComponent<Rigidbody2D>();
#endif
        }

        private void OnEnable() {} // For the Enabled checkbox

#if CINEMACHINE_PHYSICS
        private void OnCollisionEnter(Collision c)
        {
            GenerateImpactEvent(c.collider, c.relativeVelocity);
        }

        private void OnTriggerEnter(Collider c)
        {
            GenerateImpactEvent(c, Vector3.zero);
        }

        private float GetMassAndVelocity(Collider other, ref Vector3 vel)
        {
            bool getVelocity = vel == Vector3.zero;
            float mass = 1;
            if (m_ScaleImpactWithMass || m_ScaleImpactWithSpeed || m_UseImpactDirection)
            {
                if (mRigidBody != null)
                {
                    if (m_ScaleImpactWithMass)
                        mass *= mRigidBody.mass;
                    if (getVelocity)
                        vel = -mRigidBody.velocity;
                }
                var rb = other != null ? other.attachedRigidbody : null;
                if (rb != null)
                {
                    if (m_ScaleImpactWithMass)
                        mass *= rb.mass;
                    if (getVelocity)
                        vel += rb.velocity;
                }
            }
            return mass;
        }

        private void GenerateImpactEvent(Collider other, Vector3 vel)
        {
            // Check the filters
            if (!enabled)
                return;

            if (other != null)
            {
                int layer = other.gameObject.layer;
                if (((1 << layer) & m_LayerMask) == 0)
                    return;
                if (m_IgnoreTag.Length != 0 && other.CompareTag(m_IgnoreTag))
                    return;
            }

            // Calculate the signal direction and magnitude
            float mass = GetMassAndVelocity(other, ref vel);
            if (m_ScaleImpactWithSpeed)
                mass *= Mathf.Sqrt(vel.magnitude);
            Vector3 dir = m_DefaultVelocity;
            if (m_UseImpactDirection && !vel.AlmostZero())
                dir = -vel.normalized * dir.magnitude;

            // Fire it off!
            GenerateImpulseWithVelocity(dir * mass);
        }
#endif

#if CINEMACHINE_PHYSICS_2D
        private void OnCollisionEnter2D(Collision2D c)
        {
            GenerateImpactEvent2D(c.collider, c.relativeVelocity);
        }

        private void OnTriggerEnter2D(Collider2D c)
        {
            GenerateImpactEvent2D(c, Vector3.zero);
        }

        private float GetMassAndVelocity2D(Collider2D other2d, ref Vector3 vel)
        {
            bool getVelocity = vel == Vector3.zero;
            float mass = 1;
            if (m_ScaleImpactWithMass || m_ScaleImpactWithSpeed || m_UseImpactDirection)
            {
                if (mRigidBody2D != null)
                {
                    if (m_ScaleImpactWithMass)
                        mass *= mRigidBody2D.mass;
                    if (getVelocity)
                        vel = -mRigidBody2D.velocity;
                }

                var rb2d = other2d != null ? other2d.attachedRigidbody : null;
                if (rb2d != null)
                {
                    if (m_ScaleImpactWithMass)
                        mass *= rb2d.mass;
                    if (getVelocity)
                    {
                        Vector3 v = rb2d.velocity;
                        vel += v;
                    }
                }
            }
            return mass;
        }

        private void GenerateImpactEvent2D(Collider2D other2d, Vector3 vel)
        {
            // Check the filters
            if (!enabled)
                return;

            if (other2d != null)
            {
                int layer = other2d.gameObject.layer;
                if (((1 << layer) & m_LayerMask) == 0)
                    return;
                if (m_IgnoreTag.Length != 0 && other2d.CompareTag(m_IgnoreTag))
                    return;
            }

            // Calculate the signal direction and magnitude
            float mass = GetMassAndVelocity2D(other2d, ref vel);
            if (m_ScaleImpactWithSpeed)
                mass *= Mathf.Sqrt(vel.magnitude);
            Vector3 dir = m_DefaultVelocity;
            if (m_UseImpactDirection && !vel.AlmostZero())
                dir = -vel.normalized * dir.magnitude;

            // Fire it off!
            GenerateImpulseWithVelocity(dir * mass);
        }
#endif
    }
#endif
}

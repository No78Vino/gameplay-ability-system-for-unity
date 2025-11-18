using System;
using UnityEngine;

/// <summary>
/// Simple script that makes this transform look at a target.
/// </summary>
public class BossLookAt : MonoBehaviour
{
    [Tooltip("Look at this transform")]
    public Transform m_LookAt;
    [Tooltip("Lock the camera's X rotation to this value (in angles)")]
    public float m_RotationX = 0;
    [Tooltip("Lock the camera's Z rotation to this value (in angles)")]
    public float m_RotationZ = 0;

    void Update()
    {
        transform.LookAt(m_LookAt);
        var euler = transform.rotation.eulerAngles;
        transform.rotation = Quaternion.Euler(m_RotationX, euler.y, m_RotationZ);
    }
}

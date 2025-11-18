#if !UNITY_2019_3_OR_NEWER
#define CINEMACHINE_PHYSICS
#define CINEMACHINE_PHYSICS_2D
#endif

using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

namespace Cinemachine.Editor
{
#if CINEMACHINE_PHYSICS
    [CustomEditor(typeof(CinemachineCollider))]
    [CanEditMultipleObjects]
    internal sealed class CinemachineColliderEditor : BaseEditor<CinemachineCollider>
    {
        /// <summary>Get the property names to exclude in the inspector.</summary>
        /// <param name="excluded">Add the names to this list</param>
        protected override void GetExcludedPropertiesInInspector(List<string> excluded)
        {
            base.GetExcludedPropertiesInInspector(excluded);
            if (!Target.m_AvoidObstacles)
            {
                excluded.Add(FieldPath(x => x.m_DistanceLimit));
                excluded.Add(FieldPath(x => x.m_CameraRadius));
                excluded.Add(FieldPath(x => x.m_Strategy));
                excluded.Add(FieldPath(x => x.m_MaximumEffort));
                excluded.Add(FieldPath(x => x.m_Damping));
                excluded.Add(FieldPath(x => x.m_DampingWhenOccluded));
                excluded.Add(FieldPath(x => x.m_SmoothingTime));
            }
            else if (Target.m_Strategy == CinemachineCollider.ResolutionStrategy.PullCameraForward)
            {
                excluded.Add(FieldPath(x => x.m_MaximumEffort));
            }
        }

        public override void OnInspectorGUI()
        {
            BeginInspector();

            if (Target.m_AvoidObstacles && Target.VirtualCamera != null
                    && !Target.VirtualCamera.State.HasLookAt)
                EditorGUILayout.HelpBox(
                    "Avoid Obstacles requires a LookAt target.",
                    MessageType.Warning);

            DrawRemainingPropertiesInInspector();
        }

        [DrawGizmo(GizmoType.Active | GizmoType.Selected, typeof(CinemachineCollider))]
        private static void DrawColliderGizmos(CinemachineCollider collider, GizmoType type)
        {
            CinemachineVirtualCameraBase vcam = (collider != null) ? collider.VirtualCamera : null;
            if (vcam != null && collider.enabled)
            {
                Color oldColor = Gizmos.color;
                Vector3 pos = vcam.State.FinalPosition;
                if (collider.m_AvoidObstacles && vcam.State.HasLookAt)
                {
                    Gizmos.color = CinemachineColliderPrefs.FeelerColor;
                    if (collider.m_CameraRadius > 0)
                        Gizmos.DrawWireSphere(pos, collider.m_CameraRadius);

                    Vector3 forwardFeelerVector = (vcam.State.ReferenceLookAt - pos).normalized;
                    float distance = collider.m_DistanceLimit;
                    Gizmos.DrawLine(pos, pos + forwardFeelerVector * distance);

                    // Show the avoidance path, for debugging
                    List<List<Vector3>> debugPaths = collider.DebugPaths;
                    foreach (var path in debugPaths)
                    {
                        Gizmos.color = CinemachineColliderPrefs.FeelerHitColor;
                        Vector3 p0 = vcam.State.ReferenceLookAt;
                        foreach (var p in path)
                        {
                            Gizmos.DrawLine(p0, p);
                            p0 = p;
                        }
                        Gizmos.DrawLine(p0, pos);
                    }
                }
                Gizmos.color = oldColor;
            }
        }
    }
#endif
}

using System;
using UnityEngine;

namespace EXProceduralMachine
{
    /// <summary>
    ///     可视化辅助：在场景视图中绘制 Box / Sphere 标记。
    /// </summary>
    public class XVisualAid : MonoBehaviour
    {
        public enum GizmoShape
        {
            Box,
            Sphere
        }

        [Header("Display Settings")] public bool showGizmo = true;

        [Header("Gizmo Settings")] public GizmoShape shape = GizmoShape.Box;
        public Color gizmoColor = Color.yellow;

        [Header("Dimensions")] public Vector3 size = Vector3.one;
        public float radius = 0.5f;

        [Header("Rotation")] public bool syncRotation = true;

#if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            if (!showGizmo)
                return;

            Gizmos.color = gizmoColor;

            switch (shape)
            {
                case GizmoShape.Box:
                    DrawRotatedGizmo(() =>
                    {
                        Gizmos.DrawCube(Vector3.zero, size);
                        Gizmos.DrawWireCube(Vector3.zero, size);
                    }, transform.position);
                    break;
                case GizmoShape.Sphere:
                    Gizmos.DrawSphere(transform.position, radius);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void DrawRotatedGizmo(System.Action drawAction, Vector3 position)
        {
            var originalMatrix = Gizmos.matrix;
            try
            {
                var rotation = syncRotation ? transform.rotation : Quaternion.identity;
                Gizmos.matrix = Matrix4x4.TRS(position, rotation, Vector3.one);
                drawAction();
            }
            finally
            {
                Gizmos.matrix = originalMatrix;
            }
        }
#endif
    }
}
using System;
using UnityEngine;

namespace EXProceduralMachine
{
#if UNITY_EDITOR
    [ExecuteInEditMode]
#endif
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
            // 如果不显示Gizmo则直接返回
            if (!showGizmo) return;

            // 设置颜色
            Gizmos.color = gizmoColor;

            // 获取变换信息
            var position = transform.position;

            switch (shape)
            {
                case GizmoShape.Box:
                    DrawRotatedGizmo(() =>
                    {
                        Gizmos.DrawCube(Vector3.zero, size);
                        Gizmos.DrawWireCube(Vector3.zero, size); // 添加边框便于观察
                    }, position);
                    break;

                case GizmoShape.Sphere:
                    // 球体无需旋转
                    Gizmos.DrawSphere(position, radius);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        /// <summary>
        /// 绘制可旋转的Gizmo
        /// </summary>
        private void DrawRotatedGizmo(System.Action drawAction, Vector3 position)
        {
            if (syncRotation && shape != GizmoShape.Sphere) // 球体不需要旋转
            {
                // 保存当前矩阵
                Matrix4x4 originalMatrix = Gizmos.matrix;

                try
                {
                    // 设置新的矩阵以匹配游戏对象的旋转
                    Matrix4x4 rotationMatrix = Matrix4x4.TRS(position, transform.rotation, Vector3.one);
                    Gizmos.matrix = rotationMatrix;

                    // 执行绘制操作
                    drawAction();
                }
                finally
                {
                    // 恢复原始矩阵
                    Gizmos.matrix = originalMatrix;
                }
            }
            else
            {
                // 不使用旋转，直接在目标位置绘制
                Matrix4x4 originalMatrix = Gizmos.matrix;

                try
                {
                    Gizmos.matrix = Matrix4x4.TRS(position, Quaternion.identity, Vector3.one);
                    drawAction();
                }
                finally
                {
                    Gizmos.matrix = originalMatrix;
                }
            }
        }

        // 避免在构建版本中产生警告
        private void Start()
        {
        }

        private void Update()
        {
        }
#endif
    }
}
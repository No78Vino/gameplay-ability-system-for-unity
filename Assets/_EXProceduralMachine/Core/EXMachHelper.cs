using UnityEngine;

namespace EXProceduralMachine
{
    /// <summary>
    ///     程序化动画通用数学与地面检测工具。
    /// </summary>
    public static class EXMachHelper
    {
        /// <summary>
        ///     从指定点向下发射射线获取地面交点；未命中时返回最大距离点。
        /// </summary>
        public static Vector3 GetGroundPoint(Vector3 origin, float maxDistance, LayerMask layerMask, Vector3 direction)
        {
            GetGroundInfo(origin, maxDistance, layerMask, direction, out var point, out _);
            return point;
        }

        /// <summary>
        ///     从指定点沿方向发射射线，返回地面交点与法线。
        ///     未命中时返回最大距离点，法线取反方向（向下投射时为 +Y）。
        /// </summary>
        public static bool GetGroundInfo(Vector3 origin, float maxDistance, LayerMask layerMask, Vector3 direction,
            out Vector3 point, out Vector3 normal)
        {
            if (Physics.Raycast(origin, direction, out var hit, maxDistance, layerMask))
            {
                point = hit.point;
                normal = hit.normal;
                return true;
            }

            point = origin + direction.normalized * maxDistance;
            normal = -direction.normalized;
            return false;
        }

        /// <summary>
        ///     计算点 A 相对点 B 在方向 N 上的带符号投影距离（沿 N 方向为正）。
        /// </summary>
        public static float CalculateProjectionDistance(Vector3 a, Vector3 b, Vector3 n)
        {
            if (n.sqrMagnitude < Mathf.Epsilon)
                return 0f;

            return Vector3.Dot(a - b, n.normalized);
        }

        /// <summary>
        ///     由三点确定平面，计算躯干旋转（前向投影到平面上）。
        /// </summary>
        public static Quaternion CalculateBodyRotation(
            Vector3 bodyForward,
            Vector3 pointA,
            Vector3 pointB,
            Vector3 pointC)
        {
            var planeNormal = CalculatePlaneNormal(pointA, pointB, pointC);

            var projectedForward = ProjectVectorOnPlane(planeNormal, bodyForward);
            if (projectedForward.sqrMagnitude < 0.0001f)
                projectedForward = ProjectVectorOnPlane(planeNormal, Vector3.forward);

            return Quaternion.LookRotation(projectedForward.normalized, planeNormal);
        }

        /// <summary>
        ///     计算三点确定平面的法线（右手法则并保证朝上）；三点共线时回退为 Vector3.up。
        /// </summary>
        public static Vector3 CalculatePlaneNormal(Vector3 a, Vector3 b, Vector3 c)
        {
            var normal = Vector3.Cross(b - a, c - a);
            if (normal.sqrMagnitude < 1e-6f)
                return Vector3.up;

            normal.Normalize();
            if (Vector3.Dot(normal, Vector3.up) < 0f)
                normal = -normal;

            return normal;
        }

        /// <summary>
        ///     将向量投影到平面上：v' = v - (v·n)n。
        /// </summary>
        public static Vector3 ProjectVectorOnPlane(Vector3 planeNormal, Vector3 vector)
        {
            return vector - Vector3.Dot(vector, planeNormal) * planeNormal;
        }
    }
}
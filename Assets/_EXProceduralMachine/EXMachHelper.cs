using UnityEngine;

namespace EXProceduralMachine
{
    public static class EXMachHelper
    {
        /// <summary>
        /// 从指定点向下发射射线获取地面交点
        /// </summary>
        /// <param name="origin">发射点位置</param>
        /// <param name="maxDistance">最大检测距离</param>
        /// <param name="layerMask">要检测的层级</param>
        /// <param name="direction">发射方向</param>
        /// <returns>与地面的交点位置</returns>
        public static Vector3 GetGroundPoint(Vector3 origin, float maxDistance, LayerMask layerMask, Vector3 direction)
        {
            // 执行射线检测
            if (Physics.Raycast(origin, direction, out RaycastHit hit, maxDistance, layerMask))
                return hit.point;

            // 没有找到交点或超过最大距离，使用最大距离点
            var fallbackPoint = origin + direction * maxDistance;
            return fallbackPoint;
        }
        
        /// <summary>
        /// 计算A、B两点相对于C点在方向向量N上的投影距离（带正负）
        /// </summary>
        /// <param name="A">点A</param>
        /// <param name="B">参考点B</param>
        /// <param name="N">方向向量N（非零）</param>
        /// <return>A相对于B的投影距离（输出）</return>
        public static float CalculateProjectionDistance(Vector3 A, Vector3 B, Vector3 N)
        {
            // 检查方向向量是否为零
            if (N.sqrMagnitude < Mathf.Epsilon)
                return 0;
            
            // 2. 构造从C指向A、C指向B的向量
            var ab = A - B;

            // 3. 计算标量投影（点积）：结果带正负，绝对值为投影距离
            var projectionA = Vector3.Dot(ab, N.normalized);
            return projectionA;
        }

    }
}
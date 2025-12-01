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
        public static Vector3 GetGroundPoint(Vector3 origin, float maxDistance, LayerMask layerMask,Vector3 direction)
        {
            // 执行射线检测
            if (Physics.Raycast(origin, direction, out RaycastHit hit, maxDistance, layerMask))
                return hit.point;
            
            // 没有找到交点或超过最大距离，使用最大距离点
            var fallbackPoint = origin + direction * maxDistance;
            return fallbackPoint;
        }
    }
}
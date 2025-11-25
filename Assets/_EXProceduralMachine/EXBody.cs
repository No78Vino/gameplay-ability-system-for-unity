using System;
using System.Collections.Generic;
using UnityEngine;

namespace EXProceduralMachine
{
    public class EXBody : MonoBehaviour
    {
        public List<Transform> castPoints;
        public List<Transform> groundPoints;
        public float maxDistance = 10f;
        public float moveStep = 3f;
        
        private void Update()
        {
            for (var i = 0; i < castPoints.Count; i++)
            {
                var cast = castPoints[i];
                var gPos = GetGroundPoint(cast.position, maxDistance, LayerMask.GetMask("Terrain"));
                if((gPos-groundPoints[i].position).magnitude > moveStep)
                    groundPoints[i].position = gPos;
            }
        }

        /// <summary>
        /// 从指定点向下发射垂直射线获取地面交点
        /// </summary>
        /// <param name="origin">发射点位置</param>
        /// <param name="maxDistance">最大检测距离</param>
        /// <param name="layerMask">要检测的层级</param>
        /// <returns>与地面的交点位置</returns>
        public static Vector3 GetGroundPoint(Vector3 origin, float maxDistance, LayerMask layerMask)
        {
            // 射线方向（向下）
            var direction = Vector3.down;
        
            // 执行射线检测
            if (Physics.Raycast(origin, direction, out RaycastHit hit, maxDistance, layerMask))
                return hit.point;
            
            // 没有找到交点或超过最大距离，使用最大距离点
            var fallbackPoint = origin + direction * maxDistance;
            return fallbackPoint;
        }

        private List<Vector3> GetCastPoints()
        {
            var points = new List<Vector3>()
            {
                
            };
            return points;
        }
    }
}
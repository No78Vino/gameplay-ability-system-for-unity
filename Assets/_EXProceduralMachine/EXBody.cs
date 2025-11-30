using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace EXProceduralMachine
{
    public class EXBody : MonoBehaviour
    {
        private Vector3 _lastPos;
        public Transform Zigzag1;
        public Transform Zigzag2;
        
        public List<Transform> castPoints;
        public List<Transform> groundPoints;
        public List<ParabolaMover> movers;
        
        public float maxDistance = 10f;
        public float moveStep = 3f;
        public float moveMaxStep = 3f;
        public float stepMoveTime = 0.1f;
        public float stepHeight = 0.5f;

        private void Awake()
        {
            _lastPos = transform.position;
        }

        private void Update()
        {
            if ((transform.position - _lastPos).magnitude > moveStep)
            {
                _lastPos = transform.position;
                var zigzagDist1 = Vector3.Distance(Zigzag1.position, _lastPos);
                var zigzagDist2 = Vector3.Distance(Zigzag2.position, _lastPos);
                if (zigzagDist1 > zigzagDist2)
                {
                    if(!movers[1].IsMoving||(Zigzag1.position - _lastPos).magnitude > moveMaxStep)
                        Zigzag1.position = _lastPos;
                }
                else
                {
                    if(!movers[0].IsMoving||(Zigzag2.position - _lastPos).magnitude > moveMaxStep)
                        Zigzag2.position = _lastPos;
                }
            }

            for (var i = 0; i < castPoints.Count; i++)
            {
                var cast = castPoints[i];
                var gPos = GetGroundPoint(cast.position, maxDistance, LayerMask.GetMask("Terrain"));
                if ((gPos - groundPoints[i].position).magnitude > moveStep)
                {
                    groundPoints[i].position = gPos;
                    movers[i].MoveToParabola(gPos,stepMoveTime,stepHeight);
                }
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
    }
}
using System.Collections.Generic;
using UnityEngine;

namespace EXProceduralMachine
{
    public static class EXMachHelper
    {
        /// <summary>
        ///     从指定点向下发射射线获取地面交点
        /// </summary>
        /// <param name="origin">发射点位置</param>
        /// <param name="maxDistance">最大检测距离</param>
        /// <param name="layerMask">要检测的层级</param>
        /// <param name="direction">发射方向</param>
        /// <returns>与地面的交点位置</returns>
        public static Vector3 GetGroundPoint(Vector3 origin, float maxDistance, LayerMask layerMask, Vector3 direction)
        {
            // 执行射线检测
            if (Physics.Raycast(origin, direction, out var hit, maxDistance, layerMask))
                return hit.point;

            // 没有找到交点或超过最大距离，使用最大距离点
            var fallbackPoint = origin + direction * maxDistance;
            return fallbackPoint;
        }

        /// <summary>
        ///     计算A、B两点相对于C点在方向向量N上的投影距离（带正负）
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


        /// <summary>
        /// 通过三点确定平面，计算身体旋转
        /// </summary>
        /// <param name="bodyForward">期望朝向</param>
        /// <param name="pointA">点A（世界坐标）</param>
        /// <param name="pointB">点B（世界坐标）</param>
        /// <param name="pointC">点C（世界坐标）</param>
        /// <returns>计算出的旋转四元数</returns>
        public static Quaternion CalculateBodyRotation(
            Vector3 bodyForward,
            Vector3 pointA,
            Vector3 pointB,
            Vector3 pointC)
        {
            // 1. 计算三点确定的平面的法线
            Vector3 planeNormal = CalculatePlaneNormal(pointA, pointB, pointC);

            // 2. 将期望前向方向投影到平面上
            Vector3 projectedForward = ProjectVectorOnPlane(planeNormal, bodyForward);

            // 如果投影后的前向太小（几乎与法线平行），使用默认前向
            if (projectedForward.magnitude < 0.01f)
                projectedForward = ProjectVectorOnPlane(planeNormal, Vector3.forward);
            
            // 3. 创建旋转
            return Quaternion.LookRotation(projectedForward.normalized, planeNormal);
        }

        /// <summary>
        /// 计算三点确定的平面的法线（右手法则，保证正面朝上）
        /// </summary>
        public static Vector3 CalculatePlaneNormal(Vector3 a, Vector3 b, Vector3 c)
        {
            // 计算两个向量
            Vector3 ab = b - a;
            Vector3 ac = c - a;

            // 叉积得到法线
            Vector3 normal = Vector3.Cross(ab, ac);

            // 归一化并确保朝上（与重力方向相反）
            normal.Normalize();

            // 确保法线方向大致朝上（与重力反方向夹角小于90度）
            if (Vector3.Dot(normal, Vector3.up) < 0)
            {
                normal = -normal;
            }

            return normal;
        }

        /// <summary>
        /// 将向量投影到平面上
        /// </summary>
        public static Vector3 ProjectVectorOnPlane(Vector3 planeNormal, Vector3 vector)
        {
            // 计算公式：v_projected = v - (v·n)*n
            float dot = Vector3.Dot(vector, planeNormal);
            return vector - dot * planeNormal;
        }

        /// <summary>
        /// 平滑版本：使用多个帧平滑过渡
        /// </summary>
        public static Vector3 CalculateBodyRotationAngle(
            Vector3 bodyForward,
            Vector3 pointA,
            Vector3 pointB,
            Vector3 pointC)
        {
            Quaternion targetRotation = CalculateBodyRotation(bodyForward, pointA, pointB, pointC);

            var angle = targetRotation.eulerAngles;
            angle.y = 0;
            return angle;
        }
    }
}
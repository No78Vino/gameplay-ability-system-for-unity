using UnityEngine;

namespace GAS.General
{
    public static class DebugExtension
    {
        public static void DebugBox(Vector2 center, Vector2 size, float angle, Color color, float showTime)
        {
            var halfSize = size / 2;
            var p1 = new Vector2(center.x - halfSize.x, center.y - halfSize.y);
            var p2 = new Vector2(center.x + halfSize.x, center.y - halfSize.y);
            var p3 = new Vector2(center.x + halfSize.x, center.y + halfSize.y);
            var p4 = new Vector2(center.x - halfSize.x, center.y + halfSize.y);
            // p1 绕center旋转angle角度
            p1 = RotatePoint(center, p1, angle);
            p2 = RotatePoint(center, p2, angle);
            p3 = RotatePoint(center, p3, angle);
            p4 = RotatePoint(center, p4, angle);
            Debug.DrawLine(p1, p2, color, showTime);
            Debug.DrawLine(p2, p3, color, showTime);
            Debug.DrawLine(p3, p4, color, showTime);
            Debug.DrawLine(p4, p1, color, showTime);
        }
        
        public static Vector2 RotatePoint(Vector2 center, Vector2 point, float angle)
        {
            var cos = Mathf.Cos(angle * Mathf.Deg2Rad);
            var sin = Mathf.Sin(angle * Mathf.Deg2Rad);
            var x = cos * (point.x - center.x) - sin * (point.y - center.y) + center.x;
            var y = sin * (point.x - center.x) + cos * (point.y - center.y) + center.y;
            return new Vector2(x, y);
        }
        
        public static void DebugDrawCircle (Vector2 center, float radius, Color color, float showTime,float segments = 120)
        {
            var step = 360f / segments;
            var from = center + new Vector2(radius, 0);
            for (var i = 0; i < segments; i++)
            {
                var to = center + new Vector2(radius * Mathf.Cos((i + 1) * step * Mathf.Deg2Rad),
                    radius * Mathf.Sin((i + 1) * step * Mathf.Deg2Rad));
                Debug.DrawLine(from, to, color, showTime);
                from = to;
            }
        }
        
        /// <summary>
        /// 绘制一个圆形
        /// </summary>
        /// <param name="position">圆心位置</param>
        /// <param name="normal">圆面法线</param>
        /// <param name="radius">圆半径</param>
        /// <param name="segments">圆的分段数</param>
        /// <param name="color">圆的颜色</param>
        /// <param name="duration">绘制时长(0为1帧)</param>
        public static void DrawCircle(Vector3 position, Vector3 normal, float radius, int segments = 36,
            Color? color = null,
            float duration = 0f)
        {
            var actualColor = color ?? Color.cyan;

            normal.Normalize();

            // 计算垂直于法线的一个向量
            var up = Vector3.Cross(normal, normal == Vector3.forward ? Vector3.up : Vector3.forward).normalized;

            // 计算圆的每个点的位置
            var angleStep = 360f / segments;
            var lastPoint = position + up * radius;
            for (int i = 1; i <= segments; i++)
            {
                var angle = i * angleStep;
                var rotation = Quaternion.AngleAxis(angle, normal);
                var nextPoint = position + rotation * up * radius;
                Debug.DrawLine(lastPoint, nextPoint, actualColor, duration);
                lastPoint = nextPoint;
            }
        }

        /// <summary>
        /// 绘制一个矩形
        /// </summary>
        /// <param name="position">矩形中心点</param>
        /// <param name="size">矩形的宽高</param>
        /// <param name="rotation">矩形的旋转（四元数）</param>
        /// <param name="color">矩形的颜色</param>
        /// <param name="duration">绘制时长(0为1帧)</param>
        public static void DrawRectangle(Vector3 position, Vector2 size, Quaternion rotation, Color? color = null,
            float duration = 0f)
        {
            var actualColor = color ?? Color.cyan;
            // 计算矩形的四个角点在局部坐标系中的位置
            var halfSize = new Vector3(size.x / 2, 0, size.y / 2);
            var corners = new Vector3[4];
            corners[0] = new Vector3(-halfSize.x, 0, -halfSize.z); // 左下角
            corners[1] = new Vector3(halfSize.x, 0, -halfSize.z); // 右下角
            corners[2] = new Vector3(halfSize.x, 0, halfSize.z); // 右上角
            corners[3] = new Vector3(-halfSize.x, 0, halfSize.z); // 左上角

            // 旋转角点并转换到世界坐标系
            for (int i = 0; i < corners.Length; i++)
            {
                corners[i] = position + rotation * corners[i];
            }

            // 绘制矩形的四条边
            Debug.DrawLine(corners[0], corners[1], actualColor, duration); // 底边
            Debug.DrawLine(corners[1], corners[2], actualColor, duration); // 右边
            Debug.DrawLine(corners[2], corners[3], actualColor, duration); // 顶边
            Debug.DrawLine(corners[3], corners[0], actualColor, duration); // 左边
        }

        /// <summary>
        /// 绘制一个立方体
        /// </summary>
        /// <param name="position">立方体中心点</param>
        /// <param name="size">立方体的尺寸</param>
        /// <param name="rotation">立方体的旋转（四元数）</param>
        /// <param name="color">立方体的颜色</param>
        /// <param name="duration">绘制时长(0为1帧)</param>
        public static void DrawCube(Vector3 position, Vector3 size, Quaternion rotation, Color? color = null,
            float duration = 0f)
        {
            var actualColor = color ?? Color.cyan;
            // 计算立方体的八个顶点在局部坐标系中的位置
            var halfSize = size / 2;
            var vertices = new Vector3[8];

            // 下底面四个顶点
            vertices[0] = new Vector3(-halfSize.x, -halfSize.y, -halfSize.z);
            vertices[1] = new Vector3(halfSize.x, -halfSize.y, -halfSize.z);
            vertices[2] = new Vector3(halfSize.x, -halfSize.y, halfSize.z);
            vertices[3] = new Vector3(-halfSize.x, -halfSize.y, halfSize.z);

            // 上顶面四个顶点
            vertices[4] = new Vector3(-halfSize.x, halfSize.y, -halfSize.z);
            vertices[5] = new Vector3(halfSize.x, halfSize.y, -halfSize.z);
            vertices[6] = new Vector3(halfSize.x, halfSize.y, halfSize.z);
            vertices[7] = new Vector3(-halfSize.x, halfSize.y, halfSize.z);

            // 旋转顶点并转换到世界坐标系
            for (int i = 0; i < vertices.Length; i++)
            {
                vertices[i] = position + rotation * vertices[i];
            }

            // 绘制立方体的十二条边
            Debug.DrawLine(vertices[0], vertices[1], actualColor, duration); // 底面边
            Debug.DrawLine(vertices[1], vertices[2], actualColor, duration);
            Debug.DrawLine(vertices[2], vertices[3], actualColor, duration);
            Debug.DrawLine(vertices[3], vertices[0], actualColor, duration);

            Debug.DrawLine(vertices[4], vertices[5], actualColor, duration); // 顶面边
            Debug.DrawLine(vertices[5], vertices[6], actualColor, duration);
            Debug.DrawLine(vertices[6], vertices[7], actualColor, duration);
            Debug.DrawLine(vertices[7], vertices[4], actualColor, duration);

            Debug.DrawLine(vertices[0], vertices[4], actualColor, duration); // 竖向边
            Debug.DrawLine(vertices[1], vertices[5], actualColor, duration);
            Debug.DrawLine(vertices[2], vertices[6], actualColor, duration);
            Debug.DrawLine(vertices[3], vertices[7], actualColor, duration);
        }

        /// <summary>
        /// 绘制一个扇形
        /// </summary>
        /// <param name="position">扇形的中心点</param>
        /// <param name="forward">扇形的前进方向</param>
        /// <param name="angle">扇形的角度（度）</param>
        /// <param name="radius">扇形的半径</param>
        /// <param name="segments">扇形的分段数(多少块)</param>
        /// <param name="color">扇形的颜色</param>
        /// <param name="duration">绘制时长(0为1帧)</param>
        public static void DrawSector(Vector3 position, Vector3 forward, float angle, float radius, int segments = 12,
            Color? color = null, float duration = 0f)
        {
            var actualColor = color ?? Color.cyan;
            forward.Normalize();
            var angleStep = angle / segments;
            float currentAngle = -angle / 2;

            // 绘制扇形的边缘
            for (int i = 0; i <= segments; i++)
            {
                // 计算当前段的方向
                var currentDirection = Quaternion.Euler(0, currentAngle, 0) * forward;

                // 计算当前段的终点
                var currentPoint = position + currentDirection * radius;

                // 绘制从中心到当前段终点的线段
                Debug.DrawLine(position, currentPoint, actualColor, duration);

                // 如果不是第一段，则绘制上一段终点到当前段终点的线段
                if (i > 0)
                {
                    var previousDirection = Quaternion.Euler(0, currentAngle - angleStep, 0) * forward;
                    var previousPoint = position + previousDirection * radius;
                    Debug.DrawLine(previousPoint, currentPoint, actualColor, duration);
                }

                currentAngle += angleStep;
            }
        }
    }
}
using UnityEngine;

namespace GAS.General
{
    public static class DebugExtension
    {
        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        public static void DebugBox(Vector2 center, Vector2 size, float angle, Color color, float showTime)
        {
            var halfSize = size * 0.5f;
            var p1 = new Vector2(center.x - halfSize.x, center.y - halfSize.y);
            var p2 = new Vector2(center.x + halfSize.x, center.y - halfSize.y);
            var p3 = new Vector2(center.x + halfSize.x, center.y + halfSize.y);
            var p4 = new Vector2(center.x - halfSize.x, center.y + halfSize.y);
            // p1 绕center旋转angle角度
            p1 = RotatePoint(center, p1, angle);
            p2 = RotatePoint(center, p2, angle);
            p3 = RotatePoint(center, p3, angle);
            p4 = RotatePoint(center, p4, angle);
            DrawLine(p1, p2, color, showTime);
            DrawLine(p2, p3, color, showTime);
            DrawLine(p3, p4, color, showTime);
            DrawLine(p4, p1, color, showTime);
        }

        public static Vector2 RotatePoint(Vector2 center, Vector2 point, float angle)
        {
            var cos = Mathf.Cos(angle * Mathf.Deg2Rad);
            var sin = Mathf.Sin(angle * Mathf.Deg2Rad);
            var x = cos * (point.x - center.x) - sin * (point.y - center.y) + center.x;
            var y = sin * (point.x - center.x) + cos * (point.y - center.y) + center.y;
            return new Vector2(x, y);
        }

        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        public static void DebugDrawCircle(Vector2 center, float radius, Color color, float showTime, float segments = 120)
        {
            var step = 360f / segments;
            var from = center + new Vector2(radius, 0);
            for (var i = 0; i < segments; i++)
            {
                var to = center + new Vector2(radius * Mathf.Cos((i + 1) * step * Mathf.Deg2Rad),
                    radius * Mathf.Sin((i + 1) * step * Mathf.Deg2Rad));
                DrawLine(from, to, color, showTime);
                from = to;
            }
        }

        /// <summary>
        /// 绘制一个圆形
        /// </summary>
        /// <param name="position">位置, 圆心</param>
        /// <param name="rotation">旋转</param>
        /// <param name="radius">半径</param>
        /// <param name="segments">分段数(建议与角度适配, 如每10°分一段: segments = Mathf.CeilToInt(angle / 10))</param>
        /// <param name="color">颜色</param>
        /// <param name="duration">绘制时长(0为1帧)</param>
        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        public static void DrawCircle(in Vector3 position, in Quaternion rotation, float radius, int segments = 36,
            in Color? color = null, float duration = 0f)
        {
            DrawArc(position, rotation, radius, 360, segments, color, duration);
        }

        public static void DrawCircle(in Vector3 position, in Vector3 forward, float radius, int segments = 36,
            in Color? color = null, float duration = 0f)
        {
            DrawCircle(position, Quaternion.LookRotation(forward), radius, segments, color, duration);
        }

        /// <summary>
        /// 绘制一个圆弧
        /// </summary>
        /// <param name="position">位置</param>
        /// <param name="rotation">旋转</param>
        /// <param name="radius">半径</param>
        /// <param name="angle">角度</param>
        /// <param name="segments">分段数(建议与角度适配, 如每10°分一段: segments = Mathf.CeilToInt(angle / 10))(建议与角度适配, 如每10°分一段: segments = Mathf.CeilToInt(angle / 10))</param>
        /// <param name="color">颜色</param>
        /// <param name="duration">绘制时长(0为1帧)</param>
        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        public static void DrawArc(in Vector3 position, in Quaternion rotation, float radius, float angle, int segments = 12,
            in Color? color = null, float duration = 0f)
        {
            if (angle <= 0) return;
            if (radius <= 0) return;
            if (segments <= 0) return;

            var angleStep = angle / segments;
            var lastPoint = position + rotation * (Vector3.right * radius);
            for (int i = 1; i <= segments; i++)
            {
                var currentAngle = i * angleStep;
                var nextPoint = position + rotation * (Quaternion.Euler(0, currentAngle, 0) * Vector3.right * radius);
                DrawLine(lastPoint, nextPoint, color, duration);
                lastPoint = nextPoint;
            }
        }

        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        public static void DrawArc(in Vector3 position, in Vector3 forward, float radius, float angle, int segments = 12,
            in Color? color = null, float duration = 0f)
        {
            DrawArc(position, Quaternion.LookRotation(forward), radius, angle, segments, color, duration);
        }

        /// <summary>
        /// 绘制一个圆环
        /// </summary>
        /// <param name="position">位置, 圆心</param>
        /// <param name="rotation">旋转</param>
        /// <param name="outerRadius">外圈半径</param>
        /// <param name="innerRadius">内圈半径</param>
        /// <param name="segments">分段数(建议与角度适配, 如每10°分一段: segments = Mathf.CeilToInt(angle / 10))</param>
        /// <param name="color">颜色</param>
        /// <param name="duration">绘制时长(0为1帧)</param>
        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        public static void DrawRing(in Vector3 position, in Quaternion rotation, float outerRadius, float innerRadius, int segments = 36,
            in Color? color = null, float duration = 0f)
        {
            if (outerRadius <= 0) return;
            if (segments <= 0) return;

            innerRadius = Mathf.Clamp(innerRadius, 0, outerRadius);

            // 计算圆的每个点的位置
            var angleStep = 360f / segments;
            var lastOuterPoint = position + rotation * (Vector3.right * outerRadius);
            var lastInnerPoint = position + rotation * (Vector3.right * innerRadius);
            for (int i = 1; i <= segments; i++)
            {
                var angle = i * angleStep;
                var nextOuterPoint = position + rotation * (Quaternion.Euler(0, angle, 0) * Vector3.right * outerRadius);
                var nextInnerPoint = position + rotation * (Quaternion.Euler(0, angle, 0) * Vector3.right * innerRadius);
                DrawLine(lastOuterPoint, nextOuterPoint, color, duration);
                DrawLine(lastInnerPoint, nextInnerPoint, color, duration);
                DrawLine(nextOuterPoint, nextInnerPoint, color, duration);
                lastOuterPoint = nextOuterPoint;
                lastInnerPoint = nextInnerPoint;
            }
        }

        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        public static void DrawRing(in Vector3 position, in Vector3 forward, float outerRadius, float innerRadius, int segments = 36,
            in Color? color = null, float duration = 0f)
        {
            DrawRing(position, Quaternion.LookRotation(forward), outerRadius, innerRadius, segments, color, duration);
        }

        /// <summary>
        /// 绘制一个矩形
        /// </summary>
        /// <param name="position">位置, 矩形中心点</param>
        /// <param name="rotation">旋转</param>
        /// <param name="size">矩形的宽长, 宽:左右(垂直于朝向), 长:前后(与朝向一致)</param>
        /// <param name="color">颜色</param>
        /// <param name="duration">绘制时长(0为1帧)</param>
        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        public static void DrawRectangle(in Vector3 position, in Quaternion rotation, in Vector2 size,
            in Color? color = null, float duration = 0f)
        {
            // 计算矩形的四个角点在局部坐标系中的位置
            var halfSize = new Vector3(size.x * 0.5f, 0, size.y * 0.5f);
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
            DrawLine(corners[0], corners[1], color, duration); // 底边
            DrawLine(corners[1], corners[2], color, duration); // 右边
            DrawLine(corners[2], corners[3], color, duration); // 顶边
            DrawLine(corners[3], corners[0], color, duration); // 左边
        }

        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        public static void DrawRectangle(in Vector3 position, in Vector3 forward, in Vector2 size,
            in Color? color = null, float duration = 0f)
        {
            DrawRectangle(position, Quaternion.LookRotation(forward), size, color, duration);
        }

        /// <summary>
        /// 基于位置和朝向, 在其前方绘制一个矩形(如: 基于技能释放者前方的矩形攻击范围)
        /// </summary>
        /// <param name="position">位置</param>
        /// <param name="rotation">旋转</param>
        /// <param name="size">矩形的宽长(宽:左右, 长:前后)</param>
        /// <param name="color">颜色</param>
        /// <param name="duration">绘制时长(0为1帧)</param>
        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        public static void DrawFrontRectangle(in Vector3 position, in Quaternion rotation, in Vector2 size,
            in Color? color = null, float duration = 0f)
        {
            // 计算矩形的四个角点在局部坐标系中的位置
            var halfX = size.x * 0.5f;
            var corners = new Vector3[4];
            corners[0] = new Vector3(-halfX, 0, 0); // 左下角
            corners[1] = new Vector3(halfX, 0, 0); // 右下角
            corners[2] = new Vector3(halfX, 0, size.y); // 右上角
            corners[3] = new Vector3(-halfX, 0, size.y); // 左上角

            // 旋转角点并转换到世界坐标系
            for (int i = 0; i < corners.Length; i++)
            {
                corners[i] = position + rotation * corners[i];
            }

            // 绘制矩形的四条边
            DrawLine(corners[0], corners[1], color, duration); // 底边
            DrawLine(corners[1], corners[2], color, duration); // 右边
            DrawLine(corners[2], corners[3], color, duration); // 顶边
            DrawLine(corners[3], corners[0], color, duration); // 左边
        }

        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        public static void DrawFrontRectangle(in Vector3 position, in Vector3 forward, in Vector2 size,
            in Color? color = null, float duration = 0f)
        {
            DrawFrontRectangle(position, Quaternion.LookRotation(forward), size, color, duration);
        }

        /// <summary>
        /// 绘制一个立方体
        /// </summary>
        /// <param name="position">立方体中心点</param>
        /// <param name="rotation">立方体的朝向</param>
        /// <param name="size">立方体的尺寸</param>
        /// <param name="color">颜色</param>
        /// <param name="duration">绘制时长(0为1帧)</param>
        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        public static void DrawCube(in Vector3 position, in Quaternion rotation, in Vector3 size,
            in Color? color = null, float duration = 0f)
        {
            // 计算立方体的八个顶点在局部坐标系中的位置
            var halfSize = size * 0.5f;
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
            DrawLine(vertices[0], vertices[1], color, duration); // 底面边
            DrawLine(vertices[1], vertices[2], color, duration);
            DrawLine(vertices[2], vertices[3], color, duration);
            DrawLine(vertices[3], vertices[0], color, duration);

            DrawLine(vertices[4], vertices[5], color, duration); // 顶面边
            DrawLine(vertices[5], vertices[6], color, duration);
            DrawLine(vertices[6], vertices[7], color, duration);
            DrawLine(vertices[7], vertices[4], color, duration);

            DrawLine(vertices[0], vertices[4], color, duration); // 竖向边
            DrawLine(vertices[1], vertices[5], color, duration);
            DrawLine(vertices[2], vertices[6], color, duration);
            DrawLine(vertices[3], vertices[7], color, duration);
        }

        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        public static void DrawCube(in Vector3 position, in Vector3 forward, in Vector3 size,
            in Color? color = null, float duration = 0f)
        {
            DrawCube(position, Quaternion.LookRotation(forward), size, color, duration);
        }

        /// <summary>
        /// 绘制一个圆柱体
        /// </summary>
        /// <param name="position"></param>
        /// <param name="rotation">朝向, 以圆柱的侧面为forward/right, 上下面为up</param>
        /// <param name="radius"></param>
        /// <param name="height"></param>
        /// <param name="segments"> </param>
        /// <param name="color"></param>
        /// <param name="duration"></param>
        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        public static void DrawCylinder(in Vector3 position, in Quaternion rotation, float radius, float height, int segments = 12,
            in Color? color = null, float duration = 0f)
        {
            var offset = rotation * Vector3.up * (height * 0.5f);
            var topCenter = position + offset;
            var bottomCenter = position - offset;
            DrawCylinder(topCenter, bottomCenter, radius, segments, color, duration);
        }

        /// <summary>
        /// 绘制一个圆柱体
        /// </summary>
        /// <param name="position"></param>
        /// <param name="forward">朝向, forward是圆柱体的侧面(其实对于圆柱体而言forward/right都一样)</param>
        /// <param name="radius"></param>
        /// <param name="height"></param>
        /// <param name="segments"></param>
        /// <param name="color"></param>
        /// <param name="duration"></param>
        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        public static void DrawCylinder(in Vector3 position, in Vector3 forward, float radius, float height, int segments = 12,
            in Color? color = null, float duration = 0f)
        {
            DrawCylinder(position, Quaternion.LookRotation(forward), radius, height, segments, in color, duration);
        }

        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        public static void DrawCylinder(in Vector3 topCenter, in Vector3 bottomCenter, float radius, int segments = 12,
            in Color? color = null, float duration = 0f)
        {
            if (radius <= 0) return;
            if (segments <= 0) return;

            if (Vector3.Distance(topCenter, bottomCenter) <= Mathf.Epsilon) return;

            var direction = (topCenter - bottomCenter).normalized;

            // 计算一个垂直于direction的向量作为forward
            var forward = Vector3.Cross(direction, Vector3.up).normalized;

            // 如果forward和direction平行（即direction和Vector3.up共线），则选择一个不同的方向作为forward
            if (forward == Vector3.zero)
            {
                forward = Vector3.Cross(direction, Vector3.right).normalized;
            }

            var rotation = Quaternion.LookRotation(forward, direction);

            var angleStep = 360f / segments;
            var offset = rotation * (Vector3.right * radius);
            var lastTopPoint = topCenter + offset;
            var lastBottomPoint = bottomCenter + offset;
            for (int i = 1; i <= segments; i++)
            {
                var currentAngle = i * angleStep;
                offset = rotation * (Quaternion.Euler(0, currentAngle, 0) * (Vector3.right * radius));
                var nextTopPoint = topCenter + offset;
                var nextBottomPoint = bottomCenter + offset;
                DrawLine(lastTopPoint, nextTopPoint, color, duration);
                DrawLine(lastBottomPoint, nextBottomPoint, color, duration);
                DrawLine(nextTopPoint, nextBottomPoint, color, duration);
                lastTopPoint = nextTopPoint;
                lastBottomPoint = nextBottomPoint;
            }
        }

        /// <summary>
        /// 绘制一个扇形
        /// </summary>
        /// <param name="position">位置, 扇形所在圆的圆心</param>
        /// <param name="rotation">旋转</param>
        /// <param name="radius">扇形的半径</param>
        /// <param name="angle">扇形的角度（度）</param>
        /// <param name="segments">分段数(建议与角度适配, 如每10°分一段: segments = Mathf.CeilToInt(angle / 10))</param>
        /// <param name="color">颜色</param>
        /// <param name="duration">绘制时长(0为1帧)</param>
        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        public static void DrawSector(Vector3 position, Quaternion rotation, float radius, float angle, int segments = 12,
            Color? color = null, float duration = 0f)
        {
            DrawAnnularSector(position, rotation, radius, 0, angle, segments, color, duration);
        }

        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        public static void DrawSector(in Vector3 position, in Vector3 forward, float radius, float angle, int segments = 12,
            in Color? color = null, float duration = 0f)
        {
            DrawSector(position, Quaternion.LookRotation(forward), radius, angle, segments, color, duration);
        }


        /// <summary>
        /// 绘制一个环形扇区
        /// </summary>
        /// <param name="position">位置, 扇形所在圆的圆心</param>
        /// <param name="rotation">旋转</param>
        /// <param name="outerRadius">扇形的外半径</param>
        /// <param name="innerRadius">扇形的内半径, 为0时为标准扇形</param>
        /// <param name="angle">扇形的角度（度）</param>
        /// <param name="segments">分段数(建议与角度适配, 如每10°分一段: segments = Mathf.CeilToInt(angle / 10))</param>
        /// <param name="color">颜色</param>
        /// <param name="duration">绘制时长(0为1帧)</param>
        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        public static void DrawAnnularSector(in Vector3 position, in Quaternion rotation, float outerRadius, float innerRadius, float angle, int segments = 12,
            in Color? color = null, float duration = 0f)
        {
            if (outerRadius <= 0) return;
            if (segments <= 0) return;
            if (angle <= 0) return;

            innerRadius = Mathf.Clamp(innerRadius, 0, outerRadius);

            var angleStep = angle / segments;
            var currentAngle = -angle * 0.5f;

            var outerPoints = new Vector3[segments + 1];
            var innerPoints = new Vector3[segments + 1];

            // 计算内外扇形的点
            for (var i = 0; i <= segments; i++)
            {
                var currentDirection = rotation * Quaternion.Euler(0, currentAngle, 0) * Vector3.forward;
                outerPoints[i] = position + currentDirection * outerRadius;
                innerPoints[i] = position + currentDirection * innerRadius;
                currentAngle += angleStep;
            }

            // 绘制内外扇形的边缘
            for (var i = 0; i <= segments; i++)
            {
                // 绘制内扇形的边缘
                if (i > 0)
                {
                    DrawLine(innerPoints[i - 1], innerPoints[i], color, duration);
                    DrawLine(outerPoints[i - 1], outerPoints[i], color, duration);
                }

                // 绘制从中心到内外扇形点的线段
                DrawLine(innerPoints[i], outerPoints[i], color, duration);
            }

            // 绘制内扇形的两侧边缘
            DrawLine(position, innerPoints[0], color, duration);
            DrawLine(position, innerPoints[segments], color, duration);
        }

        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        public static void DrawAnnularSector(in Vector3 position, in Vector3 forward, float outerRadius, float innerRadius, float angle, int segments = 12, in Color? color = null, float duration = 0f)
        {
            DrawAnnularSector(position, Quaternion.LookRotation(forward), outerRadius, innerRadius, angle, segments, color, duration);
        }

        /// <summary>
        /// 绘制一条线段
        /// </summary>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <param name="color">颜色</param>
        /// <param name="duration">绘制时长(0为1帧)</param>
        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        public static void DrawLine(in Vector3 start, in Vector3 end, in Color? color = null, float duration = 0f)
        {
            Debug.DrawLine(start, end, color ?? Color.cyan, duration);
        }

        /// <summary>
        /// 绘制一个箭头
        /// </summary>
        /// <param name="origin"></param>
        /// <param name="destination"></param>
        /// <param name="color">颜色</param>
        /// <param name="duration">绘制时长(0为1帧)</param>
        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        public static void DrawArrow(in Vector3 origin, in Vector3 destination, in Color? color = null, float duration = 0f)
        {
            DrawLine(origin, destination, color, duration);
            var direction = destination - origin;
            var right = Quaternion.LookRotation(direction) * Quaternion.Euler(0, 180 + 30, 0) * Vector3.forward;
            var left = Quaternion.LookRotation(direction) * Quaternion.Euler(0, 180 - 30, 0) * Vector3.forward;
            DrawLine(destination, destination + right, color, duration);
            DrawLine(destination, destination + left, color, duration);
        }


        /// <summary>
        /// 绘制一个箭头
        /// </summary>
        /// <param name="origin"></param>
        /// <param name="rotation"></param>
        /// <param name="length">长度</param>
        /// <param name="color">颜色</param>
        /// <param name="duration">绘制时长(0为1帧)</param>
        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        public static void DrawArrow(in Vector3 origin, in Quaternion rotation, float length, in Color? color = null, float duration = 0f)
        {
            var direction = rotation * Vector3.forward;
            var destination = origin + direction * length;
            DrawArrow(origin, destination, color, duration);
        }

        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        public static void DrawArrow(in Vector3 origin, in Vector3 direction, float length, in Color? color = null, float duration = 0f)
        {
            var destination = origin + direction.normalized * length;
            DrawArrow(origin, destination, color, duration);
        }
    }
}
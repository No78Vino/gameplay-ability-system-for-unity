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
    }
}
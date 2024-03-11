using System.Collections.Generic;
using UnityEngine;

namespace Com.LuisPedroFonseca.ProCamera2D
{
    public enum EaseType
    {
        EaseInOut,
        EaseOut,
        EaseIn,
        Linear
    }

    public static class Utils
    {
        public static float EaseFromTo(float start, float end, float value, EaseType type = EaseType.EaseInOut)
        {
            value = Mathf.Clamp01(value);

            switch (type)
            {
                case EaseType.EaseInOut:
                    return Mathf.Lerp(start, end, value * value * (3.0f - 2.0f * value));

                case EaseType.EaseOut:
                    return Mathf.Lerp(start, end, Mathf.Sin(value * Mathf.PI * 0.5f));

                case EaseType.EaseIn:
                    return Mathf.Lerp(start, end, 1.0f - Mathf.Cos(value * Mathf.PI * 0.5f));

                default:
                    return Mathf.Lerp(start, end, value);
            }
        }

        public static float SmoothApproach(float pastPosition, float pastTargetPosition, float targetPosition, float speed, float deltaTime)
        {
            float t = deltaTime * speed;
            float v = (targetPosition - pastTargetPosition) / t;
            float f = pastPosition - pastTargetPosition + v;
            return targetPosition - v + f * Mathf.Exp(-t);
        }

        public static float Remap(this float value, float from1, float to1, float from2, float to2)
        {
            return Mathf.Clamp((value - from1) / (to1 - from1) * (to2 - from2) + from2, from2, to2);
        }

        public static void DrawArrowForGizmo(Vector3 pos, Vector3 direction, float arrowHeadLength = 0.25f, float arrowHeadAngle = 20.0f)
        {
            Gizmos.DrawRay(pos, direction);
            DrawArrowEnd(true, pos, direction, Gizmos.color, arrowHeadLength, arrowHeadAngle);
        }

        public static void DrawArrowForGizmo(Vector3 pos, Vector3 direction, Color color, float arrowHeadLength = 0.25f, float arrowHeadAngle = 20.0f)
        {
            Gizmos.DrawRay(pos, direction);
            DrawArrowEnd(true, pos, direction, color, arrowHeadLength, arrowHeadAngle);
        }

        public static void DrawArrowForDebug(Vector3 pos, Vector3 direction, float arrowHeadLength = 0.25f, float arrowHeadAngle = 20.0f)
        {
            Debug.DrawRay(pos, direction);
            DrawArrowEnd(false, pos, direction, Gizmos.color, arrowHeadLength, arrowHeadAngle);
        }

        public static void DrawArrowForDebug(Vector3 pos, Vector3 direction, Color color, float arrowHeadLength = 0.25f, float arrowHeadAngle = 20.0f)
        {
            Debug.DrawRay(pos, direction, color);
            DrawArrowEnd(false, pos, direction, color, arrowHeadLength, arrowHeadAngle);
        }

        static void DrawArrowEnd(bool gizmos, Vector3 pos, Vector3 direction, Color color, float arrowHeadLength = 0.25f, float arrowHeadAngle = 20.0f)
        {
            if (direction == Vector3.zero)
                return;

            Vector3 right = Quaternion.LookRotation(direction) * Quaternion.Euler(arrowHeadAngle, 0, 0) * Vector3.back;
            Vector3 left = Quaternion.LookRotation(direction) * Quaternion.Euler(-arrowHeadAngle, 0, 0) * Vector3.back;
            Vector3 up = Quaternion.LookRotation(direction) * Quaternion.Euler(0, arrowHeadAngle, 0) * Vector3.back;
            Vector3 down = Quaternion.LookRotation(direction) * Quaternion.Euler(0, -arrowHeadAngle, 0) * Vector3.back;
            if (gizmos)
            {
                Gizmos.color = color;
                Gizmos.DrawRay(pos + direction, right * arrowHeadLength);
                Gizmos.DrawRay(pos + direction, left * arrowHeadLength);
                Gizmos.DrawRay(pos + direction, up * arrowHeadLength);
                Gizmos.DrawRay(pos + direction, down * arrowHeadLength);
            }
            else
            {
                Debug.DrawRay(pos + direction, right * arrowHeadLength, color);
                Debug.DrawRay(pos + direction, left * arrowHeadLength, color);
                Debug.DrawRay(pos + direction, up * arrowHeadLength, color);
                Debug.DrawRay(pos + direction, down * arrowHeadLength, color);
            }
        }

        public static bool AreNearlyEqual(float a, float b, float tolerance = .02f)
        {
            return Mathf.Abs(a - b) < tolerance;
        }

        public static Vector2 GetScreenSizeInWorldCoords(Camera gameCamera, float distance = 10f)
        {
            float width = 0f;
            float height = 0f;

            if (gameCamera.orthographic)
            {
                if (gameCamera.orthographicSize <= .001f)
                    return Vector2.zero;

                var p1 = gameCamera.ViewportToWorldPoint(new Vector3(0, 0, gameCamera.nearClipPlane));
                var p2 = gameCamera.ViewportToWorldPoint(new Vector3(1, 0, gameCamera.nearClipPlane));
                var p3 = gameCamera.ViewportToWorldPoint(new Vector3(1, 1, gameCamera.nearClipPlane));
 
                width = (p2 - p1).magnitude;
                height = (p3 - p2).magnitude;
            }
            else
            {
                height = 2.0f * Mathf.Abs(distance) * Mathf.Tan(gameCamera.fieldOfView * 0.5f * Mathf.Deg2Rad);
                width = height * gameCamera.aspect;
            }
 
            return new Vector2(width, height);
        }

        public static Vector3 GetVectorsSum(IList<Vector3> input)
        {
            Vector3 output = Vector3.zero;
            for (int i = 0; i < input.Count; i++)
            {
                output += input[i];
            }
            return output;
        }

        public static float AlignToGrid(float input, float gridSize)
        {
            return Mathf.Round((Mathf.Round(input / gridSize) * gridSize) / gridSize) * gridSize;
        }

        public static bool IsInsideRectangle(float x, float y, float width, float height, float pointX, float pointY)
        {
            if (pointX >= x - width * .5f &&
                pointX <= x + width * .5f &&
                pointY >= y - height * .5f &&
                pointY <= y + height * .5f)
                return true;

            return false;
        }

        public static bool IsInsideCircle(float x, float y, float radius, float pointX, float pointY)
        {
            return (pointX - x) * (pointX - x) + (pointY - y) * (pointY - y) < radius * radius;
        }
    }
}
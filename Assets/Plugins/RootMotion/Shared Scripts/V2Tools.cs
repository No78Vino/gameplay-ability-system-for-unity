using UnityEngine;
using System.Collections;

namespace RootMotion
{

    /// <summary>
    /// Helper methods for dealing with 2-dimensional vectors.
    /// </summary>
    public static class V2Tools
    {
        /// <summary>
        /// Converts Vector3 to Vector2 on the XZ plane
        /// </summary>
        public static Vector2 XZ(Vector3 v)
        {
            return new Vector2(v.x, v.z);
        }

        /// <summary>
        /// Returns delta angle from 'dir1' to 'dir2' in degrees.
        /// </summary>
        public static float DeltaAngle(Vector2 dir1, Vector2 dir2)
        {
            float angle1 = Mathf.Atan2(dir1.x, dir1.y) * Mathf.Rad2Deg;
            float angle2 = Mathf.Atan2(dir2.x, dir2.y) * Mathf.Rad2Deg;
            return Mathf.DeltaAngle(angle1, angle2);
        }

        /// <summary>
        /// Returns delta angle from Vector3 'dir1' to Vector3 'dir2' on the XZ plane in degrees.
        /// </summary>
        public static float DeltaAngleXZ(Vector3 dir1, Vector3 dir2)
        {
            float angle1 = Mathf.Atan2(dir1.x, dir1.z) * Mathf.Rad2Deg;
            float angle2 = Mathf.Atan2(dir2.x, dir2.z) * Mathf.Rad2Deg;
            return Mathf.DeltaAngle(angle1, angle2);
        }

        /// <summary>
        /// Returns true if a line from 'p1' to 'p2' intersects a circle with position 'c' and radius 'r'.
        /// </summary>
        public static bool LineCircleIntersect(Vector2 p1, Vector2 p2, Vector2 c, float r)
        {
            Vector2 d = p2 - p1;
            Vector2 f = c - p1;

            float a = Vector2.Dot(d, d);
            float b = 2f * Vector2.Dot(f, d);
            float z = Vector2.Dot(f, f) - r * r;

            float discr = b * b - 4f * a * z;
            if (discr < 0f) return false;

            discr = Mathf.Sqrt(discr);
            float a2 = 2f * a;
            float t1 = (b - discr) / a2;
            float t2 = (b + discr) / a2;

            if (t1 >= 0f && t1 <= 1f) return true;
            if (t2 >= 0f && t2 <= 1f) return true;
            return false;
        }

        /// <summary>
        /// Returns true if an infinite ray 'dir' from position 'p1' intersects a circle with position 'c' and radius 'r'.
        /// </summary>
        public static bool RayCircleIntersect(Vector2 p1, Vector2 dir, Vector2 c, float r)
        {
            Vector2 p2 = p1 + dir;
            p1 -= c;
            p2 -= c;
            float dx = p2.x - p1.x;
            float dy = p2.y - p1.y;
            float dr = Mathf.Sqrt(Mathf.Pow(dx, 2f) + Mathf.Pow(dy, 2f));
            float D = p1.x * p2.y - p2.x * p1.y;

            float discr = Mathf.Pow(r, 2f) * Mathf.Pow(dr, 2f) - Mathf.Pow(D, 2f);
            return discr >= 0f;
        }
    }
}

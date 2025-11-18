using System;
using UnityEngine;

namespace Cinemachine.Utility
{
    /// <summary>Extensions to the Vector3 class, used by Cinemachine</summary>
    public static class UnityVectorExtensions
    {
        /// <summary>A useful Epsilon</summary>
        public const float Epsilon = 0.0001f;

        /// <summary>
        /// Checks if the Vector2 contains NaN for x or y.
        /// </summary>
        /// <param name="v">Vector2 to check for NaN</param>
        /// <returns>True, if any components of the vector are NaN</returns>
        public static bool IsNaN(this Vector2 v)
        {
            return float.IsNaN(v.x) || float.IsNaN(v.y);
        }
        
        /// <summary>
        /// Checks if the Vector2 contains NaN for x or y.
        /// </summary>
        /// <param name="v">Vector2 to check for NaN</param>
        /// <returns>True, if any components of the vector are NaN</returns>
        public static bool IsNaN(this Vector3 v)
        {
            return float.IsNaN(v.x) || float.IsNaN(v.y) || float.IsNaN(v.z);
        }
        

        /// <summary>
        /// Get the closest point on a line segment.
        /// </summary>
        /// <param name="p">A point in space</param>
        /// <param name="s0">Start of line segment</param>
        /// <param name="s1">End of line segment</param>
        /// <returns>The interpolation parameter representing the point on the segment, with 0==s0, and 1==s1</returns>
        public static float ClosestPointOnSegment(this Vector3 p, Vector3 s0, Vector3 s1)
        {
            Vector3 s = s1 - s0;
            float len2 = Vector3.SqrMagnitude(s);
            if (len2 < Epsilon)
                return 0; // degenrate segment
            return Mathf.Clamp01(Vector3.Dot(p - s0, s) / len2);
        }

        /// <summary>
        /// Get the closest point on a line segment.
        /// </summary>
        /// <param name="p">A point in space</param>
        /// <param name="s0">Start of line segment</param>
        /// <param name="s1">End of line segment</param>
        /// <returns>The interpolation parameter representing the point on the segment, with 0==s0, and 1==s1</returns>
        public static float ClosestPointOnSegment(this Vector2 p, Vector2 s0, Vector2 s1)
        {
            Vector2 s = s1 - s0;
            float len2 = Vector2.SqrMagnitude(s);
            if (len2 < Epsilon)
                return 0; // degenrate segment
            return Mathf.Clamp01(Vector2.Dot(p - s0, s) / len2);
        }

        /// <summary>
        /// Returns a non-normalized projection of the supplied vector onto a plane
        /// as described by its normal
        /// </summary>
        /// <param name="vector"></param>
        /// <param name="planeNormal">The normal that defines the plane.  Must have a length of 1.</param>
        /// <returns>The component of the vector that lies in the plane</returns>
        public static Vector3 ProjectOntoPlane(this Vector3 vector, Vector3 planeNormal)
        {
            return (vector - Vector3.Dot(vector, planeNormal) * planeNormal);
        }
        
        /// <summary>
        /// Normalized the vector onto the unit square instead of the unit circle
        /// </summary>
        /// <param name="v">The vector to normalize</param>
        /// <returns>The normalized vector, or the zero vector if its magnitude 
        /// was too small to normalize</returns>
        public static Vector2 SquareNormalize(this Vector2 v)
        {
            var d = Mathf.Max(Mathf.Abs(v.x), Mathf.Abs(v.y));
            return d < Epsilon ? Vector2.zero : v / d;
        }

        /// <summary>
        /// Calculates the intersection point defined by line_1 [p1, p2], and line_2 [q1, q2].
        /// </summary>
        /// <param name="p1">line_1 is defined by (p1, p2)</param>
        /// <param name="p2">line_1 is defined by (p1, p2)</param>
        /// <param name="q1">line_2 is defined by (q1, q2)</param>
        /// <param name="q2">line_2 is defined by (q1, q2)</param>
        /// <param name="intersection">If lines intersect at a single point, 
        /// then this will hold the intersection point. 
        /// Otherwise, it will be Vector2.positiveInfinity.</param>
        /// <returns>
        ///     0 = no intersection, 
        ///     1 = lines intersect, 
        ///     2 = segments intersect, 
        ///     3 = lines are colinear, segments do not touch, 
        ///     4 = lines are colinear, segments touch (at one or at multiple points)
        /// </returns>
        public static int FindIntersection(
            in Vector2 p1, in Vector2 p2, in Vector2 q1, in Vector2 q2, 
            out Vector2 intersection)
        {
            var p = p2 - p1;
            var q = q2 - q1;
            var pq = q1 - p1;
            var pXq = p.Cross(q);
            if (Mathf.Abs(pXq) < 0.00001f)
            {
                // The lines are parallel (or close enough to it)
                intersection = Vector2.positiveInfinity;
                if (Mathf.Abs(pq.Cross(p)) < 0.00001f)
                {
                    // The lines are colinear.  Do the segments touch?
                    var dotPQ = Vector2.Dot(q, p);

                    if (dotPQ > 0 && (p1 - q2).sqrMagnitude < 0.001f)
                    {
                        // q points to start of p
                        intersection = q2;
                        return 4;
                    }
                    if (dotPQ < 0 && (p2 - q2).sqrMagnitude < 0.001f)
                    {
                        // p and q point at the same point
                        intersection = p2;
                        return 4;
                    }

                    var dot = Vector2.Dot(pq, p);
                    if (0 <= dot && dot <= Vector2.Dot(p, p))
                    {
                        if (dot < 0.0001f)
                        {
                            if (dotPQ <= 0 && (p1 - q1).sqrMagnitude < 0.001f)
                                intersection = p1; // p and q start at the same point and point away
                        }
                        else if (dotPQ > 0 && (p2 - q1).sqrMagnitude < 0.001f)
                            intersection = p2; // p points at start of q

                        return 4;   // colinear segments touch
                    }

                    dot = Vector2.Dot(p1 - q1, q);
                    if (0 <= dot && dot <= Vector2.Dot(q, q))
                        return 4;   // colinear segments overlap

                    return 3;   // colinear segments don't touch
                }
                return 0; // the lines are parallel and not colinear
            }

            var t = pq.Cross(q) / pXq;
            intersection = p1 + t * p;

            var u = pq.Cross(p) / pXq;
            if (0 <= t && t <= 1 && 0 <= u && u <= 1)
                return 2;   // segments touch

            return 1;   // segments don't touch but lines intersect
        }

        private static float Cross(this Vector2 v1, Vector2 v2) { return (v1.x * v2.y) - (v1.y * v2.x); }
        
        /// <summary>
        /// Component-wise absolute value
        /// </summary>
        /// <param name="v">Input vector</param>
        /// <returns>Component-wise absolute value of the input vector</returns>
        public static Vector2 Abs(this Vector2 v)
        {
            return new Vector2(Mathf.Abs(v.x), Mathf.Abs(v.y));
        }

        /// <summary>
        /// Component-wise absolute value
        /// </summary>
        /// <param name="v">Input vector</param>
        /// <returns>Component-wise absolute value of the input vector</returns>
        public static Vector3 Abs(this Vector3 v)
        {
            return new Vector3(Mathf.Abs(v.x), Mathf.Abs(v.y), Mathf.Abs(v.z));
        }

        /// <summary>
        /// Checks whether the vector components are the same value.
        /// </summary>
        /// <param name="v">Vector to check</param>
        /// <returns>True, if the vector elements are the same. False, otherwise.</returns>
        public static bool IsUniform(this Vector2 v)
        {
            return Math.Abs(v.x - v.y) < Epsilon;
        }
        
        /// <summary>
        /// Checks whether the vector components are the same value.
        /// </summary>
        /// <param name="v">Vector to check</param>
        /// <returns>True, if the vector elements are the same. False, otherwise.</returns>
        public static bool IsUniform(this Vector3 v)
        {
            return Math.Abs(v.x - v.y) < Epsilon && Math.Abs(v.x - v.z) < Epsilon;
        }

        /// <summary>Is the vector within Epsilon of zero length?</summary>
        /// <param name="v"></param>
        /// <returns>True if the square magnitude of the vector is within Epsilon of zero</returns>
        public static bool AlmostZero(this Vector3 v)
        {
            return v.sqrMagnitude < (Epsilon * Epsilon);
        }

        /// <summary>Much more stable for small angles than Unity's native implementation</summary>
        /// <param name="v1">The first vector</param>
        /// <param name="v2">The second vector</param>
        /// <returns>Angle between the vectors, in degrees</returns>
        public static float Angle(Vector3 v1, Vector3 v2)
        {
#if false // Maybe this version is better?  to test....
            float a = v1.magnitude;
            v1 *= v2.magnitude;
            v2 *= a;
            return Mathf.Atan2((v1 - v2).magnitude, (v1 + v2).magnitude) * Mathf.Rad2Deg * 2;
#else            
            v1.Normalize();
            v2.Normalize();
            return Mathf.Atan2((v1 - v2).magnitude, (v1 + v2).magnitude) * Mathf.Rad2Deg * 2;
#endif
        }

        /// <summary>Much more stable for small angles than Unity's native implementation</summary>
        /// <param name="v1">The first vector</param>
        /// <param name="v2">The second vector</param>
        /// <param name="up">Definition of up (used to determine the sign)</param>
        /// <returns>Signed angle between the vectors, in degrees</returns>
        public static float SignedAngle(Vector3 v1, Vector3 v2, Vector3 up)
        {
            float angle = Angle(v1, v2);
            if (Mathf.Sign(Vector3.Dot(up, Vector3.Cross(v1, v2))) < 0)
                return -angle;
            return angle;
        }

        /// <summary>Much more stable for small angles than Unity's native implementation</summary>
        /// <param name="v1">The first vector</param>
        /// <param name="v2">The second vector</param>
        /// <param name="up">Definition of up (used to determine the sign)</param>
        /// <returns>Rotation between the vectors</returns>
        public static Quaternion SafeFromToRotation(Vector3 v1, Vector3 v2, Vector3 up)
        {
            var axis = Vector3.Cross(v1, v2);
            if (axis.AlmostZero())
                axis = up; // in case they are pointing in opposite directions
            return Quaternion.AngleAxis(Angle(v1, v2), axis);
        }

        /// <summary>This is a slerp that mimics a camera operator's movement in that
        /// it chooses a path that avoids the lower hemisphere, as defined by
        /// the up param</summary>
        /// <param name="vA">First direction</param>
        /// <param name="vB">Second direction</param>
        /// <param name="t">Interpolation amoun t</param>
        /// <param name="up">Defines the up direction</param>
        /// <returns>Interpolated vector</returns>
        public static Vector3 SlerpWithReferenceUp(
            Vector3 vA, Vector3 vB, float t, Vector3 up)
        {
            float dA = vA.magnitude;
            float dB = vB.magnitude;
            if (dA < Epsilon || dB < Epsilon)
                return Vector3.Lerp(vA, vB, t);

            Vector3 dirA = vA / dA;
            Vector3 dirB = vB / dB;
            Quaternion qA = Quaternion.LookRotation(dirA, up);
            Quaternion qB = Quaternion.LookRotation(dirB, up);
            Quaternion q = UnityQuaternionExtensions.SlerpWithReferenceUp(qA, qB, t, up);
            Vector3 dir = q * Vector3.forward;
            return dir * Mathf.Lerp(dA, dB, t);
        }
    }

    /// <summary>Extensions to the Quaternion class, usen in various places by Cinemachine</summary>
    public static class UnityQuaternionExtensions
    {
        /// <summary>This is a slerp that mimics a camera operator's movement in that
        /// it chooses a path that avoids the lower hemisphere, as defined by
        /// the up param</summary>
        /// <param name="qA">First direction</param>
        /// <param name="qB">Second direction</param>
        /// <param name="t">Interpolation amount</param>
        /// <param name="up">Defines the up direction.  Must have a length of 1.</param>
        /// <returns>Interpolated quaternion</returns>
        public static Quaternion SlerpWithReferenceUp(
            Quaternion qA, Quaternion qB, float t, Vector3 up)
        {
            var dirA = (qA * Vector3.forward).ProjectOntoPlane(up);
            var dirB = (qB * Vector3.forward).ProjectOntoPlane(up);
            if (dirA.AlmostZero() || dirB.AlmostZero())
                return Quaternion.Slerp(qA, qB, t);

            // Work on the plane, in eulers
            var qBase = Quaternion.LookRotation(dirA, up);
            var qBaseInv = Quaternion.Inverse(qBase);
            Quaternion qA1 = qBaseInv * qA;
            Quaternion qB1 = qBaseInv * qB;
            var eA = qA1.eulerAngles;
            var eB = qB1.eulerAngles;
            return qBase * Quaternion.Euler(
                Mathf.LerpAngle(eA.x, eB.x, t),
                Mathf.LerpAngle(eA.y, eB.y, t),
                Mathf.LerpAngle(eA.z, eB.z, t));
        }

        /// <summary>Normalize a quaternion</summary>
        /// <param name="q"></param>
        /// <returns>The normalized quaternion.  Unit length is 1.</returns>
        public static Quaternion Normalized(this Quaternion q)
        {
            Vector4 v = new Vector4(q.x, q.y, q.z, q.w).normalized;
            return new Quaternion(v.x, v.y, v.z, v.w);
        }

        /// <summary>
        /// Get the rotations, first about world up, then about (travelling) local right,
        /// necessary to align the quaternion's forward with the target direction.
        /// This represents the tripod head movement needed to look at the target.
        /// This formulation makes it easy to interpolate without introducing spurious roll.
        /// </summary>
        /// <param name="orient"></param>
        /// <param name="lookAtDir">The worldspace target direction in which we want to look</param>
        /// <param name="worldUp">Which way is up.  Must have a length of 1.</param>
        /// <returns>Vector2.y is rotation about worldUp, and Vector2.x is second rotation,
        /// about local right.</returns>
        public static Vector2 GetCameraRotationToTarget(
            this Quaternion orient, Vector3 lookAtDir, Vector3 worldUp)
        {
            if (lookAtDir.AlmostZero())
                return Vector2.zero;  // degenerate

            // Work in local space
            Quaternion toLocal = Quaternion.Inverse(orient);
            Vector3 up = toLocal * worldUp;
            lookAtDir = toLocal * lookAtDir;

            // Align yaw based on world up
            float angleH = 0;
            {
                Vector3 targetDirH = lookAtDir.ProjectOntoPlane(up);
                if (!targetDirH.AlmostZero())
                {
                    Vector3 currentDirH = Vector3.forward.ProjectOntoPlane(up);
                    if (currentDirH.AlmostZero())
                    {
                        // We're looking at the north or south pole
                        if (Vector3.Dot(currentDirH, up) > 0)
                            currentDirH = Vector3.down.ProjectOntoPlane(up);
                        else
                            currentDirH = Vector3.up.ProjectOntoPlane(up);
                    }
                    angleH = UnityVectorExtensions.SignedAngle(currentDirH, targetDirH, up);
                }
            }
            Quaternion q = Quaternion.AngleAxis(angleH, up);

            // Get local vertical angle
            float angleV = UnityVectorExtensions.SignedAngle(
                q * Vector3.forward, lookAtDir, q * Vector3.right);

            return new Vector2(angleV, angleH);
        }

        /// <summary>
        /// Apply rotations, first about world up, then about (travelling) local right.
        /// rot.y is rotation about worldUp, and rot.x is second rotation, about local right.
        /// </summary>
        /// <param name="orient"></param>
        /// <param name="rot">Vector2.y is rotation about worldUp, and Vector2.x is second rotation,
        /// about local right.</param>
        /// <param name="worldUp">Which way is up</param>
        /// <returns>Result rotation after the input is applied to the input quaternion</returns>
        public static Quaternion ApplyCameraRotation(
            this Quaternion orient, Vector2 rot, Vector3 worldUp)
        {
            Quaternion q = Quaternion.AngleAxis(rot.x, Vector3.right);
            return (Quaternion.AngleAxis(rot.y, worldUp) * orient) * q;
        }
    }

    /// <summary>Ad-hoc xxtentions to the Rect structure, used by Cinemachine</summary>
    public static class UnityRectExtensions
    {
        /// <summary>Inflate a rect</summary>
        /// <param name="r"></param>
        /// <param name="delta">x and y are added/subtracted fto/from the edges of
        /// the rect, inflating it in all directions</param>
        /// <returns>The inflated rect</returns>
        public static Rect Inflated(this Rect r, Vector2 delta)
        {
            return new Rect(
                r.xMin - delta.x, r.yMin - delta.y,
                r.width + delta.x * 2, r.height + delta.y * 2);
        }
    }
}

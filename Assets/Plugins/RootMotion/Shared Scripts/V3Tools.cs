using UnityEngine;
using System.Collections;

namespace RootMotion {
	
	/// <summary>
	/// Helper methods for dealing with 3-dimensional vectors.
	/// </summary>
	public static class V3Tools {

        /// <summary>
        /// Returns yaw angle (-180 - 180) of 'forward' vector.
        /// </summary>
        public static float GetYaw(Vector3 forward)
        {
            if (forward.x == 0f && forward.z == 0f) return 0f;
            if (float.IsInfinity(forward.x) || float.IsInfinity(forward.z)) return 0;
            return Mathf.Atan2(forward.x, forward.z) * Mathf.Rad2Deg;
        }

        /// <summary>
        /// Returns pitch angle (-90 - 90) of 'forward' vector.
        /// </summary>
        public static float GetPitch(Vector3 forward)
        {
            forward = forward.normalized; // Asin range -1 - 1
            return -Mathf.Asin(forward.y) * Mathf.Rad2Deg;
        }

        /// <summary>
        /// Returns bank angle (-180 - 180) of 'forward' and 'up' vectors.
        /// </summary>
        public static float GetBank(Vector3 forward, Vector3 up)
        {
            Quaternion q = Quaternion.Inverse(Quaternion.LookRotation(Vector3.up, forward));
            up = q * up;
            float result = Mathf.Atan2(up.x, up.z) * Mathf.Rad2Deg;
            return Mathf.Clamp(result, -180f, 180f);
        }

        /// <summary>
        /// Returns yaw angle (-180 - 180) of 'forward' vector relative to rotation space defined by spaceForward and spaceUp axes.
        /// </summary>
        public static float GetYaw(Vector3 spaceForward, Vector3 spaceUp, Vector3 forward)
        {
            Quaternion space = Quaternion.Inverse(Quaternion.LookRotation(spaceForward, spaceUp));
            Vector3 dirLocal = space * forward;
            if (dirLocal.x == 0f && dirLocal.z == 0f) return 0f;
            if (float.IsInfinity(dirLocal.x) || float.IsInfinity(dirLocal.z)) return 0;
            return Mathf.Atan2(dirLocal.x, dirLocal.z) * Mathf.Rad2Deg;
        }

        /// <summary>
        /// Returns pitch angle (-90 - 90) of 'forward' vector relative to rotation space defined by spaceForward and spaceUp axes.
        /// </summary>
        public static float GetPitch(Vector3 spaceForward, Vector3 spaceUp, Vector3 forward)
        {
            Quaternion space = Quaternion.Inverse(Quaternion.LookRotation(spaceForward, spaceUp));
            Vector3 dirLocal = space * forward;
            forward.Normalize();
            return -Mathf.Asin(dirLocal.y) * Mathf.Rad2Deg;
        }

        /// <summary>
        /// Returns bank angle (-180 - 180) of 'forward' and 'up' vectors relative to rotation space defined by spaceForward and spaceUp axes.
        /// </summary>
        public static float GetBank(Vector3 spaceForward, Vector3 spaceUp, Vector3 forward, Vector3 up)
        {
            Quaternion space = Quaternion.Inverse(Quaternion.LookRotation(spaceForward, spaceUp));
            forward = space * forward;
            up = space * up;

            Quaternion q = Quaternion.Inverse(Quaternion.LookRotation(spaceUp, forward));
            up = q * up;
            float result = Mathf.Atan2(up.x, up.z) * Mathf.Rad2Deg;
            return Mathf.Clamp(result, -180f, 180f);
        }

        /// <summary>
        /// Optimized Vector3.Lerp
        /// </summary>
        public static Vector3 Lerp(Vector3 fromVector, Vector3 toVector, float weight) {
			if (weight <= 0f) return fromVector;
			if (weight >= 1f) return toVector;

			return Vector3.Lerp(fromVector, toVector, weight);
		}

		/// <summary>
		/// Optimized Vector3.Slerp
		/// </summary>
		public static Vector3 Slerp(Vector3 fromVector, Vector3 toVector, float weight) {
			if (weight <= 0f) return fromVector;
			if (weight >= 1f) return toVector;

			return Vector3.Slerp(fromVector, toVector, weight);
		}

        /// <summary>
        /// Returns vector projection on axis multiplied by weight.
        /// </summary>
        public static Vector3 ExtractVertical(Vector3 v, Vector3 verticalAxis, float weight)
        {
            if (weight <= 0f) return Vector3.zero;
            if (verticalAxis == Vector3.up) return Vector3.up * v.y * weight;
            return Vector3.Project(v, verticalAxis) * weight;
        }

        /// <summary>
        /// Returns vector projected to a plane and multiplied by weight.
        /// </summary>
        public static Vector3 ExtractHorizontal(Vector3 v, Vector3 normal, float weight)
        {
            if (weight <= 0f) return Vector3.zero;
            if (normal == Vector3.up) return new Vector3(v.x, 0f, v.z) * weight;
            Vector3 tangent = v;
            Vector3.OrthoNormalize(ref normal, ref tangent);
            return Vector3.Project(v, tangent) * weight;
        }

        /// <summary>
        /// Flattens a vector on a plane defined by 'normal'.
        /// </summary>
        public static Vector3 Flatten(Vector3 v, Vector3 normal)
        {
            if (normal == Vector3.up) return new Vector3(v.x, 0f, v.z);
            return v - Vector3.Project(v, normal);
        }

        /// <summary>
        /// Clamps the direction to clampWeight from normalDirection, clampSmoothing is the number of sine smoothing iterations applied on the result.
        /// </summary>
        public static Vector3 ClampDirection(Vector3 direction, Vector3 normalDirection, float clampWeight, int clampSmoothing)
        {
            if (clampWeight <= 0) return direction;

            if (clampWeight >= 1f) return normalDirection;

            // Getting the angle between direction and normalDirection
            float angle = Vector3.Angle(normalDirection, direction);
            float dot = 1f - (angle / 180f);

            if (dot > clampWeight) return direction;
           
            // Clamping the target
            float targetClampMlp = clampWeight > 0 ? Mathf.Clamp(1f - ((clampWeight - dot) / (1f - dot)), 0f, 1f) : 1f;

            // Calculating the clamp multiplier
            float clampMlp = clampWeight > 0 ? Mathf.Clamp(dot / clampWeight, 0f, 1f) : 1f;

            // Sine smoothing iterations
            for (int i = 0; i < clampSmoothing; i++)
            {
                float sinF = clampMlp * Mathf.PI * 0.5f;
                clampMlp = Mathf.Sin(sinF);
            }

            // Slerping the direction (don't use Lerp here, it breaks it)
            return Vector3.Slerp(normalDirection, direction, clampMlp * targetClampMlp);
        }

        /// <summary>
        /// Clamps the direction to clampWeight from normalDirection, clampSmoothing is the number of sine smoothing iterations applied on the result.
        /// </summary>
        public static Vector3 ClampDirection(Vector3 direction, Vector3 normalDirection, float clampWeight, int clampSmoothing, out bool changed) {
			changed = false;

			if (clampWeight <= 0) return direction;

			if (clampWeight >= 1f) {
				changed = true;
				return normalDirection;
			}
			
			// Getting the angle between direction and normalDirection
			float angle = Vector3.Angle(normalDirection, direction);
			float dot = 1f - (angle / 180f);

			if (dot > clampWeight) return direction;
			changed = true;
			
			// Clamping the target
			float targetClampMlp = clampWeight > 0? Mathf.Clamp(1f - ((clampWeight - dot) / (1f - dot)), 0f, 1f): 1f;
			
			// Calculating the clamp multiplier
			float clampMlp = clampWeight > 0? Mathf.Clamp(dot / clampWeight, 0f, 1f): 1f;
			
			// Sine smoothing iterations
			for (int i = 0; i < clampSmoothing; i++) {
				float sinF = clampMlp * Mathf.PI * 0.5f;
				clampMlp = Mathf.Sin(sinF);
			}
			
			// Slerping the direction (don't use Lerp here, it breaks it)
			return Vector3.Slerp(normalDirection, direction, clampMlp * targetClampMlp);
		}

		/// <summary>
		/// Clamps the direction to clampWeight from normalDirection, clampSmoothing is the number of sine smoothing iterations applied on the result.
		/// </summary>
		public static Vector3 ClampDirection(Vector3 direction, Vector3 normalDirection, float clampWeight, int clampSmoothing, out float clampValue) {
			clampValue = 1f;
			
			if (clampWeight <= 0) return direction;
			
			if (clampWeight >= 1f) {
				return normalDirection;
			}
			
			// Getting the angle between direction and normalDirection
			float angle = Vector3.Angle(normalDirection, direction);
			float dot = 1f - (angle / 180f);
			
			if (dot > clampWeight) {
				clampValue = 0f;
				return direction;
			}

			// Clamping the target
			float targetClampMlp = clampWeight > 0? Mathf.Clamp(1f - ((clampWeight - dot) / (1f - dot)), 0f, 1f): 1f;
			
			// Calculating the clamp multiplier
			float clampMlp = clampWeight > 0? Mathf.Clamp(dot / clampWeight, 0f, 1f): 1f;
			
			// Sine smoothing iterations
			for (int i = 0; i < clampSmoothing; i++) {
				float sinF = clampMlp * Mathf.PI * 0.5f;
				clampMlp = Mathf.Sin(sinF);
			}
			
			// Slerping the direction (don't use Lerp here, it breaks it)
			float slerp = clampMlp * targetClampMlp;
			clampValue = 1f - slerp;
			return Vector3.Slerp(normalDirection, direction, slerp);
		}

		/// <summary>
		/// Get the intersection point of line and plane
		/// </summary>
		public static Vector3 LineToPlane(Vector3 origin, Vector3 direction, Vector3 planeNormal, Vector3 planePoint) {
			float dot = Vector3.Dot(planePoint - origin, planeNormal);
			float normalDot = Vector3.Dot(direction, planeNormal);
			
			if (normalDot == 0.0f) return Vector3.zero;
			
			float dist = dot / normalDot;
			return origin + direction.normalized * dist;
		}

		/// <summary>
		/// Projects a point to a plane.
		/// </summary>
		public static Vector3 PointToPlane(Vector3 point, Vector3 planePosition, Vector3 planeNormal) {
			if (planeNormal == Vector3.up) {
				return new Vector3(point.x, planePosition.y, point.z);
			}

			Vector3 tangent = point - planePosition;
			Vector3 normal = planeNormal;
			Vector3.OrthoNormalize(ref normal, ref tangent);

			return planePosition + Vector3.Project(point - planePosition, tangent);
		}

        /// <summary>
        /// Same as Transform.TransformPoint(), but not using scale.
        /// </summary>
        public static Vector3 TransformPointUnscaled(Transform t, Vector3 point)
        {
            return t.position + t.rotation * point;
        }

        /// <summary>
        /// Same as Transform.InverseTransformPoint(), but not using scale.
        /// </summary>
        public static Vector3 InverseTransformPointUnscaled(Transform t, Vector3 point)
        {
            return Quaternion.Inverse(t.rotation) * (point - t.position);
        }

        /// <summary>
        /// Same as Transform.InverseTransformPoint();
        /// </summary>
        public static Vector3 InverseTransformPoint(Vector3 tPos, Quaternion tRot, Vector3 tScale, Vector3 point)
        {
            return Div(Quaternion.Inverse(tRot) * (point - tPos), tScale);
        }

        /// <summary>
        /// Same as Transform.TransformPoint()
        /// </summary>
        public static Vector3 TransformPoint(Vector3 tPos, Quaternion tRot, Vector3 tScale, Vector3 point)
        {
            return tPos + Vector3.Scale(tRot * point, tScale);
        }

        /// <summary>
        /// Divides the values of v1 by the values of v2.
        /// </summary>
        public static Vector3 Div(Vector3 v1, Vector3 v2)
        {
            return new Vector3(v1.x / v2.x, v1.y / v2.y, v1.z / v2.z);
        }
    }
}

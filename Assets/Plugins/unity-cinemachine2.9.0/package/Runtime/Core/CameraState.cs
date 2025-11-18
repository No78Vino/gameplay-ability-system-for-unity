using UnityEngine;
using Cinemachine.Utility;
using System.Collections.Generic;

namespace Cinemachine
{
    /// <summary>
    /// The output of the Cinemachine engine for a specific virtual camera.  The information
    /// in this struct can be blended, and provides what is needed to calculate an
    /// appropriate camera position, orientation, and lens setting.
    /// 
    /// Raw values are what the Cinemachine behaviours generate.  The correction channel
    /// holds perturbations to the raw values - e.g. noise or smoothing, or obstacle
    /// avoidance corrections.  Coirrections are not considered when making time-based
    /// calculations such as damping.
    /// 
    /// The Final position and orientation is the comination of the raw values and
    /// their corrections.
    /// </summary>
    public struct CameraState
    {
        /// <summary>
        /// Camera Lens Settings.
        /// </summary>
        public LensSettings Lens;

        /// <summary>
        /// Which way is up.  World space unit vector.  Must have a length of 1.
        /// </summary>
        public Vector3 ReferenceUp;

        /// <summary>
        /// The world space focus point of the camera.  What the camera wants to look at.
        /// There is a special constant define to represent "nothing".  Be careful to 
        /// check for that (or check the HasLookAt property).
        /// </summary>
        public Vector3 ReferenceLookAt;

        /// <summary>
        /// Returns true if this state has a valid ReferenceLookAt value.
        /// </summary>
        #pragma warning disable 1718 // comparison made to same variable
        public bool HasLookAt => ReferenceLookAt == ReferenceLookAt; // will be false if NaN

        /// <summary>
        /// This constant represents "no point in space" or "no direction".
        /// </summary>
        public static Vector3 kNoPoint = new Vector3(float.NaN, float.NaN, float.NaN);

        /// <summary>
        /// Raw (un-corrected) world space position of this camera
        /// </summary>
        public Vector3 RawPosition;

        /// <summary>
        /// Raw (un-corrected) world space orientation of this camera
        /// </summary>
        public Quaternion RawOrientation;

        /// <summary>This is a way for the Body component to bypass aim damping,
        /// useful for when the body needs to rotate its point of view, but does not
        /// want interference from the aim damping.  The value is the camera
        /// rotation, in Euler degrees.</summary>
        public Vector3 PositionDampingBypass;

        /// <summary>
        /// Subjective estimation of how "good" the shot is.
        /// Larger values mean better quality.  Default is 1.
        /// </summary>
        public float ShotQuality;

        /// <summary>
        /// Position correction.  This will be added to the raw position.
        /// This value doesn't get fed back into the system when calculating the next frame.
        /// Can be noise, or smoothing, or both, or something else.
        /// </summary>
        public Vector3 PositionCorrection;

        /// <summary>
        /// Orientation correction.  This will be added to the raw orientation.
        /// This value doesn't get fed back into the system when calculating the next frame.
        /// Can be noise, or smoothing, or both, or something else.
        /// </summary>
        public Quaternion OrientationCorrection;

        /// <summary>
        /// Position with correction applied.
        /// </summary>
        public Vector3 CorrectedPosition => RawPosition + PositionCorrection;

        /// <summary>
        /// Orientation with correction applied.
        /// </summary>
        public Quaternion CorrectedOrientation => RawOrientation * OrientationCorrection;

        /// <summary>
        /// Position with correction applied.  This is what the final camera gets.
        /// </summary>
        public Vector3 FinalPosition => RawPosition + PositionCorrection;

        /// <summary>
        /// Orientation with correction and dutch applied.  This is what the final camera gets.
        /// </summary>
        public Quaternion FinalOrientation
        {
            get
            {
                if (Mathf.Abs(Lens.Dutch) > UnityVectorExtensions.Epsilon)
                    return CorrectedOrientation * Quaternion.AngleAxis(Lens.Dutch, Vector3.forward);
                return CorrectedOrientation;
            }
        }

        /// <summary>
        /// These hints can be or'ed toether to influence how blending is done, and how state
        /// is applied to the camera
        /// </summary>
        public enum BlendHintValue
        {
            /// <summary>Normal state blending</summary>
            Nothing = 0,
            /// <summary>This state does not affect the camera position</summary>
            NoPosition = 1,
            /// <summary>This state does not affect the camera rotation</summary>
            NoOrientation = 2,
            /// <summary>Combination of NoPosition and NoOrientation</summary>
            NoTransform = NoPosition | NoOrientation,
            /// <summary>Spherical blend about the LookAt target (if any)</summary>
            SphericalPositionBlend = 4,
            /// <summary>Cylindrical blend about the LookAt target (if any)</summary>
            CylindricalPositionBlend = 8,
            /// <summary>Radial blend when the LookAt target changes(if any)</summary>
            RadialAimBlend = 16,
            /// <summary>Ignore the LookAt target and just slerp the orientation</summary>
            IgnoreLookAtTarget = 32,
            /// <summary>This state does not affect the lens</summary>
            NoLens = 64,
        }

        /// <summary>
        /// These hints can be or'ed toether to influence how blending is done, and how state
        /// is applied to the camera
        /// </summary>
        public BlendHintValue BlendHint;

        /// <summary>
        /// State with default values
        /// </summary>
        public static CameraState Default
        {
            get
            {
                CameraState state = new CameraState();
                state.Lens = LensSettings.Default;
                state.ReferenceUp = Vector3.up;
                state.ReferenceLookAt = kNoPoint;
                state.RawPosition = Vector3.zero;
                state.RawOrientation = Quaternion.identity;
                state.ShotQuality = 1;
                state.PositionCorrection = Vector3.zero;
                state.OrientationCorrection = Quaternion.identity;
                state.PositionDampingBypass = Vector3.zero;
                state.BlendHint = BlendHintValue.Nothing;
                return state;
            }
        }

        /// <summary>Opaque structure represent extra blendable stuff and its weight.
        /// The base system ignores this data - it is intended for extension modules</summary>
        public struct CustomBlendable 
        { 
            /// <summary>The custom stuff that the extension module will consider</summary>
            public Object m_Custom; 
            /// <summary>The weight of the custom stuff.  Must be 0...1</summary>
            public float m_Weight; 

            /// <summary>Constructor with specific values</summary>
            /// <param name="custom">The custom stuff that the extension module will consider</param>
            /// <param name="weight">The weight of the custom stuff.  Must be 0...1</param>
            public CustomBlendable(Object custom, float weight) 
                { m_Custom = custom; m_Weight = weight; }
        };

        // This is to avoid excessive GC allocs
        CustomBlendable mCustom0;
        CustomBlendable mCustom1;
        CustomBlendable mCustom2;
        CustomBlendable mCustom3;
        List<CustomBlendable> m_CustomOverflow;

        /// <summary>The number of custom blendables that will be applied to the camera.  
        /// The base system manages but otherwise ignores this data - it is intended for 
        /// extension modules</summary>
        public int NumCustomBlendables { get; private set; }

        /// <summary>Get a custom blendable that will be applied to the camera.  
        /// The base system manages but otherwise ignores this data - it is intended for 
        /// extension modules</summary>
        /// <param name="index">Which one to get.  Must be in range [0...NumCustomBlendables)</param>
        /// <returns>The custom blendable at the specified index.</returns>
        public CustomBlendable GetCustomBlendable(int index)
        {
            switch (index)
            {
                case 0: return mCustom0;
                case 1: return mCustom1;
                case 2: return mCustom2;
                case 3: return mCustom3;
                default: 
                {
                    index -= 4;
                    if (m_CustomOverflow != null && index < m_CustomOverflow.Count)
                        return m_CustomOverflow[index];
                    return new CustomBlendable(null, 0);
                }
            }
        }

        int FindCustomBlendable(Object custom)
        {
            if (mCustom0.m_Custom == custom)
                return 0;
            if (mCustom1.m_Custom == custom)
                return 1;
            if (mCustom2.m_Custom == custom)
                return 2;
            if (mCustom3.m_Custom == custom)
                return 3;
            if (m_CustomOverflow != null)
            {
                for (int i = 0; i < m_CustomOverflow.Count; ++i)
                    if (m_CustomOverflow[i].m_Custom == custom)
                        return i + 4;
            }
            return -1;
        }

        /// <summary>Add a custom blendable to the pot for eventual application to the camera.
        /// The base system manages but otherwise ignores this data - it is intended for 
        /// extension modules</summary>
        /// <param name="b">The custom blendable to add.  If b.m_Custom is the same as an 
        /// already-added custom blendable, then they will be merged and the weights combined.</param>
        public void AddCustomBlendable(CustomBlendable b)
        {
            // Attempt to merge common blendables to avoid growth
            int index = FindCustomBlendable(b.m_Custom);
            if (index >= 0)
                b.m_Weight += GetCustomBlendable(index).m_Weight;
            else
            {
                index = NumCustomBlendables;
                NumCustomBlendables = index + 1;
            }
            switch (index)
            {
                case 0: mCustom0 = b; break;
                case 1: mCustom1 = b; break;
                case 2: mCustom2 = b; break;
                case 3: mCustom3 = b; break;
                default: 
                {
                    if (m_CustomOverflow == null)
                        m_CustomOverflow = new List<CustomBlendable>();
                    m_CustomOverflow.Add(b);
                    break;
                }
            }
         }

        /// <summary>Intelligently blend the contents of two states.</summary>
        /// <param name="stateA">The first state, corresponding to t=0</param>
        /// <param name="stateB">The second state, corresponding to t=1</param>
        /// <param name="t">How much to interpolate.  Internally clamped to 0..1</param>
        /// <returns>Linearly interpolated CameraState</returns>
        public static CameraState Lerp(CameraState stateA, CameraState stateB, float t)
        {
            t = Mathf.Clamp01(t);
            float adjustedT = t;

            CameraState state = new CameraState();

            // Combine the blend hints intelligently
            if (((stateA.BlendHint & stateB.BlendHint) & BlendHintValue.NoPosition) != 0)
                state.BlendHint |= BlendHintValue.NoPosition;
            if (((stateA.BlendHint & stateB.BlendHint) & BlendHintValue.NoOrientation) != 0)
                state.BlendHint |= BlendHintValue.NoOrientation;
            if (((stateA.BlendHint & stateB.BlendHint) & BlendHintValue.NoLens) != 0)
                state.BlendHint |= BlendHintValue.NoLens;
            if (((stateA.BlendHint | stateB.BlendHint) & BlendHintValue.SphericalPositionBlend) != 0)
                state.BlendHint |= BlendHintValue.SphericalPositionBlend;
            if (((stateA.BlendHint | stateB.BlendHint) & BlendHintValue.CylindricalPositionBlend) != 0)
                state.BlendHint |= BlendHintValue.CylindricalPositionBlend;

            if (((stateA.BlendHint | stateB.BlendHint) & BlendHintValue.NoLens) == 0)
                state.Lens = LensSettings.Lerp(stateA.Lens, stateB.Lens, t);
            else if (((stateA.BlendHint & stateB.BlendHint) & BlendHintValue.NoLens) == 0)
            {
                if ((stateA.BlendHint & BlendHintValue.NoLens) != 0)
                    state.Lens = stateB.Lens;
                else
                    state.Lens = stateA.Lens;
            }
            state.ReferenceUp = Vector3.Slerp(stateA.ReferenceUp, stateB.ReferenceUp, t);
            state.ShotQuality = Mathf.Lerp(stateA.ShotQuality, stateB.ShotQuality, t);

            state.PositionCorrection = ApplyPosBlendHint(
                stateA.PositionCorrection, stateA.BlendHint,
                stateB.PositionCorrection, stateB.BlendHint, 
                state.PositionCorrection, 
                Vector3.Lerp(stateA.PositionCorrection, stateB.PositionCorrection, t));

            state.OrientationCorrection = ApplyRotBlendHint(
                stateA.OrientationCorrection, stateA.BlendHint,
                stateB.OrientationCorrection, stateB.BlendHint, 
                state.OrientationCorrection, 
                Quaternion.Slerp(stateA.OrientationCorrection, stateB.OrientationCorrection, t));

            // LookAt target
            if (!stateA.HasLookAt || !stateB.HasLookAt)
                state.ReferenceLookAt = kNoPoint;
            else
            {
                // Re-interpolate FOV to preserve target composition, if possible
                float fovA = stateA.Lens.FieldOfView;
                float fovB = stateB.Lens.FieldOfView;
                if (((stateA.BlendHint | stateB.BlendHint) & BlendHintValue.NoLens) == 0
                    && !state.Lens.Orthographic && !Mathf.Approximately(fovA, fovB))
                {
                    LensSettings lens = state.Lens;
                    lens.FieldOfView = InterpolateFOV(
                            fovA, fovB,
                            Mathf.Max((stateA.ReferenceLookAt - stateA.CorrectedPosition).magnitude, stateA.Lens.NearClipPlane),
                            Mathf.Max((stateB.ReferenceLookAt - stateB.CorrectedPosition).magnitude, stateB.Lens.NearClipPlane), t);
                    state.Lens = lens;

                    // Make sure we preserve the screen composition through FOV changes
                    adjustedT = Mathf.Abs((lens.FieldOfView - fovA) / (fovB - fovA));
                }
                // Linear interpolation of lookAt target point
                state.ReferenceLookAt = Vector3.Lerp(
                        stateA.ReferenceLookAt, stateB.ReferenceLookAt, adjustedT);
            }
            
            // Raw position
            state.RawPosition = ApplyPosBlendHint(
                stateA.RawPosition, stateA.BlendHint,
                stateB.RawPosition, stateB.BlendHint, 
                state.RawPosition, state.InterpolatePosition(
                    stateA.RawPosition, stateA.ReferenceLookAt,
                    stateB.RawPosition, stateB.ReferenceLookAt,
                    t));

            // Interpolate the LookAt in Screen Space if requested
            if (state.HasLookAt 
                && ((stateA.BlendHint | stateB.BlendHint) & BlendHintValue.RadialAimBlend) != 0)
            {
                state.ReferenceLookAt = state.RawPosition + Vector3.Slerp(
                        stateA.ReferenceLookAt - state.RawPosition, 
                        stateB.ReferenceLookAt - state.RawPosition, adjustedT);
            }

            // Clever orientation interpolation
            Quaternion newOrient = state.RawOrientation;
            if (((stateA.BlendHint | stateB.BlendHint) & BlendHintValue.NoOrientation) == 0)
            {
                Vector3 dirTarget = Vector3.zero;
                if (state.HasLookAt)//&& ((stateA.BlendHint | stateB.BlendHint) & BlendHintValue.RadialAimBlend) == 0)
                {
                    // If orientations are different, use LookAt to blend them
                    float angle = Quaternion.Angle(stateA.RawOrientation, stateB.RawOrientation);
                    if (angle > UnityVectorExtensions.Epsilon)
                        dirTarget = state.ReferenceLookAt - state.CorrectedPosition;
                }
                
                if (dirTarget.AlmostZero() 
                    || ((stateA.BlendHint | stateB.BlendHint) & BlendHintValue.IgnoreLookAtTarget) != 0)
                {
                    // Don't know what we're looking at - can only slerp
                    newOrient = UnityQuaternionExtensions.SlerpWithReferenceUp(
                            stateA.RawOrientation, stateB.RawOrientation, t, state.ReferenceUp);
                }
                else
                {
                    var blendUp = Vector3.Slerp(
                        stateA.RawOrientation * Vector3.up, stateB.RawOrientation * Vector3.up, t);

                    // Rotate while preserving our lookAt target
                    if (Vector3.Cross(dirTarget, blendUp).AlmostZero())
                    {
                        // Looking up or down at the pole
                        newOrient = UnityQuaternionExtensions.SlerpWithReferenceUp(
                                stateA.RawOrientation, stateB.RawOrientation, t, blendUp);
                    }
                    else
                    {
                        // Put the target in the center
                        newOrient = Quaternion.LookRotation(dirTarget, blendUp);

                        // Blend the desired offsets from center
                        Vector2 deltaA = -stateA.RawOrientation.GetCameraRotationToTarget(
                                stateA.ReferenceLookAt - stateA.CorrectedPosition, blendUp);
                        Vector2 deltaB = -stateB.RawOrientation.GetCameraRotationToTarget(
                                stateB.ReferenceLookAt - stateB.CorrectedPosition, blendUp);
                        newOrient = newOrient.ApplyCameraRotation(
                                Vector2.Lerp(deltaA, deltaB, adjustedT), blendUp);
                    }
                }
            }
            state.RawOrientation = ApplyRotBlendHint(
                stateA.RawOrientation, stateA.BlendHint,
                stateB.RawOrientation, stateB.BlendHint, 
                state.RawOrientation, newOrient);

            // Accumulate the custom blendables and apply the weights
            for (int i = 0; i < stateA.NumCustomBlendables; ++i)
            {
                CustomBlendable b = stateA.GetCustomBlendable(i);
                b.m_Weight *= (1-t);
                if (b.m_Weight > 0)
                    state.AddCustomBlendable(b);
            }
            for (int i = 0; i < stateB.NumCustomBlendables; ++i)
            {
                CustomBlendable b = stateB.GetCustomBlendable(i);
                b.m_Weight *= t;
                if (b.m_Weight > 0)
                    state.AddCustomBlendable(b);
            }
            return state;
        }

        static float InterpolateFOV(float fovA, float fovB, float dA, float dB, float t)
        {
            // We interpolate shot height
            float hA = dA * 2f * Mathf.Tan(fovA * Mathf.Deg2Rad / 2f);
            float hB = dB * 2f * Mathf.Tan(fovB * Mathf.Deg2Rad / 2f);
            float h = Mathf.Lerp(hA, hB, t);
            float fov = 179f;
            float d = Mathf.Lerp(dA, dB, t);
            if (d > UnityVectorExtensions.Epsilon)
                fov = 2f * Mathf.Atan(h / (2 * d)) * Mathf.Rad2Deg;
            return Mathf.Clamp(fov, Mathf.Min(fovA, fovB), Mathf.Max(fovA, fovB));
        }

        static Vector3 ApplyPosBlendHint(
            Vector3 posA, BlendHintValue hintA, 
            Vector3 posB, BlendHintValue hintB, 
            Vector3 original, Vector3 blended)
        {
            if (((hintA | hintB) & BlendHintValue.NoPosition) == 0)
                return blended;
            if (((hintA & hintB) & BlendHintValue.NoPosition) != 0)
                return original;
            if ((hintA & BlendHintValue.NoPosition) != 0)
                return posB;
            return posA;
        }

        static Quaternion ApplyRotBlendHint(
            Quaternion rotA, BlendHintValue hintA, 
            Quaternion rotB, BlendHintValue hintB, 
            Quaternion original, Quaternion blended)
        {
            if (((hintA | hintB) & BlendHintValue.NoOrientation) == 0)
                return blended;
            if (((hintA & hintB) & BlendHintValue.NoOrientation) != 0)
                return original;
            if ((hintA & BlendHintValue.NoOrientation) != 0)
                return rotB;
            return rotA;
        }

        Vector3 InterpolatePosition(
            Vector3 posA, Vector3 pivotA,
            Vector3 posB, Vector3 pivotB,
            float t)
        {
            if (pivotA == pivotA && pivotB == pivotB) // check for NaN
            {
                if ((BlendHint & BlendHintValue.CylindricalPositionBlend) != 0)
                {
                    // Cylindrical interpolation about pivot
                    var a = Vector3.ProjectOnPlane(posA - pivotA, ReferenceUp);
                    var b = Vector3.ProjectOnPlane(posB - pivotB, ReferenceUp);
                    var c = Vector3.Slerp(a, b, t);
                    posA = (posA - a) + c;
                    posB = (posB - b) + c;
                }
                else if ((BlendHint & BlendHintValue.SphericalPositionBlend) != 0)
                {
                    // Spherical interpolation about pivot
                    var c = Vector3.Slerp(posA - pivotA, posB - pivotB, t);
                    posA = pivotA + c;
                    posB = pivotB + c;
                }
            }
            return Vector3.Lerp(posA, posB, t);
        }
    }
}

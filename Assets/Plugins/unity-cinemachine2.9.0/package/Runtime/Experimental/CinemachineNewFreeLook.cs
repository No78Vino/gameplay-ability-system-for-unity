#if CINEMACHINE_EXPERIMENTAL_VCAM
using UnityEngine;
using Cinemachine.Utility;
using System;

namespace Cinemachine
{
    /// <summary>
    ///
    /// NOTE: THIS CLASS IS EXPERIMENTAL, AND NOT FOR PUBLIC USE
    ///
    /// Lighter-weight version of the CinemachineFreeLook, with extra radial axis.
    ///
    /// A Cinemachine Camera geared towards a 3rd person camera experience.
    /// The camera orbits around its subject with three separate camera rigs defining
    /// rings around the target. Each rig has its own radius, height offset, composer,
    /// and lens settings.
    /// Depending on the camera's position along the spline connecting these three rigs,
    /// these settings are interpolated to give the final camera position and state.
    /// </summary>
    [DocumentationSorting(DocumentationSortingAttribute.Level.UserRef)]
    [DisallowMultipleComponent]
    [ExecuteAlways]
    [AddComponentMenu("Cinemachine/CinemachineNewFreeLook")]
    [SaveDuringPlay]
    public class CinemachineNewFreeLook : CinemachineNewVirtualCamera
    {
        /// <summary>The Vertical axis.  Value is 0..1.  Chooses how to blend the child rigs</summary>
        [Tooltip("The Vertical axis.  Value is 0..1.  0.5 is the middle rig.  Chooses how to blend the child rigs")]
        [AxisStateProperty]
        public AxisState m_VerticalAxis = new AxisState(0, 1, false, true, 2f, 0.2f, 0.1f, "Mouse Y", false);

        [Tooltip("The Radial axis.  Value is the base radius of the orbits")]
        [AxisStateProperty]
        public AxisState m_RadialAxis = new AxisState(1, 1, false, false, 100, 0f, 0f, "Mouse ScrollWheel", false);

        /// <summary>Defines the height and radius for an orbit</summary>
        [Serializable]
        public struct Orbit
        {
            /// <summary>Height relative to target</summary>
            public float m_Height;

            /// <summary>Radius of orbit</summary>
            public float m_Radius;
        }

        /// <summary>Order is Top, Middle, Bottom</summary>
        public Orbit[] m_Orbits = new Orbit[3];

        /// <summary></summary>
        [Tooltip("Controls how taut is the line that connects the rigs' orbits, which determines final placement on the Y axis")]
        [Range(0f, 1f)]
        public float m_SplineCurvature;

        /// <summary>Identifiers for accessing override settings for top and bottom rigs</summary>
        public enum RigID { Top, Bottom };

        /// <summary>Override settings for top and bottom rigs</summary>
        [Serializable]
        public class Rig
        {
            public bool m_CustomLens;
            public LensSettings m_Lens;
            public bool m_CustomBody;
            public TransposerSettings m_Body;
            public bool m_CustomAim;
            public ComposerSettings m_Aim;
            public bool m_CustomNoise;
            public PerlinNoiseSettings m_Noise;

            public void Validate()
            {
                if (m_Lens.FieldOfView == 0)
                    m_Lens = LensSettings.Default;
                m_Lens.Validate();
            }

            /// <summary>Blendable settings for Transposer Transposer</summary>
            [Serializable] public class TransposerSettings
            {
                [Range(0f, 20f)] public float m_XDamping;
                [Range(0f, 20f)] public float m_YDamping;
                [Range(0f, 20f)] public float m_ZDamping;
                [Range(0f, 20f)] public float m_PitchDamping;
                [Range(0f, 20f)] public float m_YawDamping;
                [Range(0f, 20f)] public float m_RollDamping;

                internal void Lerp(CinemachineTransposer o, float t)
                {
                    o.m_XDamping = Mathf.Lerp(o.m_XDamping, m_XDamping, t);
                    o.m_YDamping = Mathf.Lerp(o.m_YDamping, m_YDamping, t);
                    o.m_ZDamping = Mathf.Lerp(o.m_ZDamping, m_ZDamping, t);
                    o.m_PitchDamping = Mathf.Lerp(o.m_PitchDamping, m_PitchDamping, t);
                    o.m_YawDamping = Mathf.Lerp(o.m_YawDamping, m_YawDamping, t);
                    o.m_RollDamping = Mathf.Lerp(o.m_RollDamping, m_RollDamping, t);
                }

                internal void PullFrom(CinemachineTransposer o)
                {
                    m_XDamping = o.m_XDamping;
                    m_YDamping = o.m_YDamping;
                    m_ZDamping = o.m_ZDamping;
                    m_PitchDamping = o.m_PitchDamping;
                    m_YawDamping = o.m_YawDamping;
                    m_RollDamping = o.m_RollDamping;
                }

                internal void PushTo(CinemachineTransposer o)
                {
                    o.m_XDamping = m_XDamping;
                    o.m_YDamping = m_YDamping;
                    o.m_ZDamping = m_ZDamping;
                    o.m_PitchDamping =m_PitchDamping;
                    o.m_YawDamping = m_YawDamping;
                    o.m_RollDamping = m_RollDamping;
                }
            }

            /// <summary>Blendable settings for Composer</summary>
            [Serializable] public class ComposerSettings
            {
                public Vector3 m_LookAtOffset;
                [Space]
                [Range(0f, 20)] public float m_HorizontalDamping;
                [Range(0f, 20)] public float m_VerticalDamping;
                [Space]
                [Range(0f, 1f)] public float m_ScreenX;
                [Range(0f, 1f)] public float m_ScreenY;
                [Range(0f, 1f)] public float m_DeadZoneWidth;
                [Range(0f, 1f)] public float m_DeadZoneHeight;
                [Range(0f, 2f)] public float m_SoftZoneWidth;
                [Range(0f, 2f)] public float m_SoftZoneHeight;
                [Range(-0.5f, 0.5f)] public float m_BiasX;
                [Range(-0.5f, 0.5f)] public float m_BiasY;

                internal void Lerp(CinemachineComposer c, float t)
                {
                    c.m_TrackedObjectOffset = Vector3.Lerp(c.m_TrackedObjectOffset, m_LookAtOffset, t);
                    c.m_HorizontalDamping = Mathf.Lerp(c.m_HorizontalDamping, m_HorizontalDamping, t);
                    c.m_VerticalDamping = Mathf.Lerp(c.m_VerticalDamping, m_VerticalDamping, t);
                    c.m_ScreenX = Mathf.Lerp(c.m_ScreenX, m_ScreenX, t);
                    c.m_ScreenY = Mathf.Lerp(c.m_ScreenY, m_ScreenY, t);
                    c.m_DeadZoneWidth = Mathf.Lerp(c.m_DeadZoneWidth, m_DeadZoneWidth, t);
                    c.m_DeadZoneHeight = Mathf.Lerp(c.m_DeadZoneHeight, m_DeadZoneHeight, t);
                    c.m_SoftZoneWidth = Mathf.Lerp(c.m_SoftZoneWidth, m_SoftZoneWidth, t);
                    c.m_SoftZoneHeight = Mathf.Lerp(c.m_SoftZoneHeight, m_SoftZoneHeight, t);
                    c.m_BiasX = Mathf.Lerp(c.m_BiasX, m_BiasX, t);
                    c.m_BiasY = Mathf.Lerp(c.m_BiasY, m_BiasY, t);
                }

                internal void PullFrom(CinemachineComposer c)
                {
                    m_LookAtOffset = c.m_TrackedObjectOffset;
                    m_HorizontalDamping = c.m_HorizontalDamping;
                    m_VerticalDamping = c.m_VerticalDamping;
                    m_ScreenX = c.m_ScreenX;
                    m_ScreenY = c.m_ScreenY;
                    m_DeadZoneWidth = c.m_DeadZoneWidth;
                    m_DeadZoneHeight = c.m_DeadZoneHeight;
                    m_SoftZoneWidth = c.m_SoftZoneWidth;
                    m_SoftZoneHeight = c.m_SoftZoneHeight;
                    m_BiasX = c.m_BiasX;
                    m_BiasY = c.m_BiasY;
                }

                internal void PushTo(CinemachineComposer c)
                {
                    c.m_TrackedObjectOffset = m_LookAtOffset;
                    c.m_HorizontalDamping = m_HorizontalDamping;
                    c.m_VerticalDamping = m_VerticalDamping;
                    c.m_ScreenX = m_ScreenX;
                    c.m_ScreenY = m_ScreenY;
                    c.m_DeadZoneWidth = m_DeadZoneWidth;
                    c.m_DeadZoneHeight = m_DeadZoneHeight;
                    c.m_SoftZoneWidth = m_SoftZoneWidth;
                    c.m_SoftZoneHeight = m_SoftZoneHeight;
                    c.m_BiasX = m_BiasX;
                    c.m_BiasY = m_BiasY;
                }
            }

            /// <summary>Blendable settings for CinemachineBasicMultiChannelPerlin</summary>
            [Serializable] public class PerlinNoiseSettings
            {
                public float m_AmplitudeGain;
                public float m_FrequencyGain;

                internal void Lerp(CinemachineBasicMultiChannelPerlin p, float t)
                {
                    p.m_AmplitudeGain = Mathf.Lerp(p.m_AmplitudeGain, m_AmplitudeGain, t);
                    p.m_FrequencyGain =Mathf.Lerp(p.m_FrequencyGain, m_FrequencyGain, t);
                }

                internal void PullFrom(CinemachineBasicMultiChannelPerlin p)
                {
                    m_AmplitudeGain = p.m_AmplitudeGain;
                    m_FrequencyGain = p.m_FrequencyGain;
                }

                internal void PushTo(CinemachineBasicMultiChannelPerlin p)
                {
                    p.m_AmplitudeGain = m_AmplitudeGain;
                    p.m_FrequencyGain = m_FrequencyGain;
                }
            }
        }

        [SerializeField]
        internal Rig[] m_Rigs = new Rig[2] { new Rig(), new Rig() };

        /// <summary>Accessor for rig override settings</summary>
        public Rig RigSettings(RigID rig) { return m_Rigs[(int)rig]; }

        /// Easy access to the transposer (may be null)
        CinemachineTransposer Transposer
        {
            get { return ComponentCache[(int)CinemachineCore.Stage.Body] as CinemachineTransposer; }
        }

        /// <summary>Enforce bounds for fields, when changed in inspector.</summary>
        protected override void OnValidate()
        {
            base.OnValidate();
            for (int i = 0; i < m_Rigs.Length; ++i)
                m_Rigs[i].Validate();
        }

        private void Awake()
        {
            m_VerticalAxis.HasRecentering = true;
            m_RadialAxis.HasRecentering = false;
        }

        /// <summary>Updates the child rig cache</summary>
        protected override void OnEnable()
        {
            base.OnEnable();
            UpdateInputAxisProvider();
        }
        
        /// <summary>
        /// API for the inspector.  Internal use only
        /// </summary>
        public void UpdateInputAxisProvider()
        {
            m_VerticalAxis.SetInputAxisProvider(0, null);
            m_RadialAxis.SetInputAxisProvider(1, null);
            var provider = GetInputAxisProvider();
            if (provider != null)
            {
                m_VerticalAxis.SetInputAxisProvider(1, provider);
                m_RadialAxis.SetInputAxisProvider(2, provider);
            }
        }
        
        void Reset()
        {
            DestroyComponents();
#if UNITY_EDITOR
            var orbital = UnityEditor.Undo.AddComponent<CinemachineOrbitalTransposer>(gameObject);
            UnityEditor.Undo.AddComponent<CinemachineComposer>(gameObject);
#else
            var orbital = gameObject.AddComponent<CinemachineOrbitalTransposer>();
            gameObject.AddComponent<CinemachineComposer>();
#endif
            orbital.HideOffsetInInspector = true;
            orbital.m_BindingMode = CinemachineTransposer.BindingMode.SimpleFollowWithWorldUp;

            InvalidateComponentCache();
            m_Rigs = new Rig[2] { new Rig(), new Rig() };

            // Default orbits
            m_Orbits = new Orbit[3];
            m_Orbits[0].m_Height = 10;  m_Orbits[0].m_Radius = 4;
            m_Orbits[1].m_Height = 2.5f;  m_Orbits[1].m_Radius = 8;
            m_Orbits[2].m_Height = -0.5f; m_Orbits[2].m_Radius = 5;

            m_SplineCurvature = 0.5f;
        }

        /// <summary>
        /// Force the virtual camera to assume a given position and orientation.  
        /// Procedural placement then takes over
        /// </summary>
        /// <param name="pos">Worldspace pposition to take</param>
        /// <param name="rot">Worldspace orientation to take</param>
        public override void ForceCameraPosition(Vector3 pos, Quaternion rot)
        {
            base.ForceCameraPosition(pos, rot);
            m_VerticalAxis.Value = GetYAxisClosestValue(pos, State.ReferenceUp);
        }
        
        /// <summary>If we are transitioning from another FreeLook, grab the axis values from it.</summary>
        /// <param name="fromCam">The camera being deactivated.  May be null.</param>
        /// <param name="worldUp">Default world Up, set by the CinemachineBrain</param>
        /// <param name="deltaTime">Delta time for time-based effects (ignore if less than or equal to 0)</param>
        public override void OnTransitionFromCamera(
            ICinemachineCamera fromCam, Vector3 worldUp, float deltaTime)
        {
            base.OnTransitionFromCamera(fromCam, worldUp, deltaTime);
            InvokeOnTransitionInExtensions(fromCam, worldUp, deltaTime);
            m_VerticalAxis.m_Recentering.DoRecentering(ref m_VerticalAxis, -1, 0.5f);
            m_VerticalAxis.m_Recentering.CancelRecentering();
            if (fromCam != null && m_Transitions.m_InheritPosition
                 && !CinemachineCore.Instance.IsLiveInBlend(this))
            {
                // Note: horizontal axis already taken care of by base class
                var cameraPos = fromCam.State.RawPosition;

                // Special handling for FreeLook: get an undamped outgoing position
                if (fromCam is CinemachineNewFreeLook)
                {
                    var orbital = (fromCam as CinemachineNewFreeLook).Transposer;
                    if (orbital != null)
                        cameraPos = orbital.GetTargetCameraPosition(worldUp);
                }
                ForceCameraPosition(cameraPos, fromCam.State.FinalOrientation);
            }
        }

        float GetYAxisClosestValue(Vector3 cameraPos, Vector3 up)
        {
            if (Follow != null)
            {
                // Rotate the camera pos to the back
                Quaternion q = Quaternion.FromToRotation(up, Vector3.up);
                Vector3 dir = q * (cameraPos - Follow.position);
                Vector3 flatDir = dir; flatDir.y = 0;
                if (!flatDir.AlmostZero())
                {
                    float angle = Vector3.SignedAngle(flatDir, Vector3.back, Vector3.up);
                    dir = Quaternion.AngleAxis(angle, Vector3.up) * dir;
                }
                dir.x = 0;

                // Sample the spline in a few places, find the 2 closest, and lerp
                int i0 = 0, i1 = 0;
                float a0 = 0, a1 = 0;
                const int NumSamples = 13;
                float step = 1f / (NumSamples-1);
                for (int i = 0; i < NumSamples; ++i)
                {
                    float a = Vector3.SignedAngle(
                        dir, GetLocalPositionForCameraFromInput(i * step), Vector3.right);
                    if (i == 0)
                        a0 = a1 = a;
                    else
                    {
                        if (Mathf.Abs(a) < Mathf.Abs(a0))
                        {
                            a1 = a0;
                            i1 = i0;
                            a0 = a;
                            i0 = i;
                        }
                        else if (Mathf.Abs(a) < Mathf.Abs(a1))
                        {
                            a1 = a;
                            i1 = i;
                        }
                    }
                }
                if (Mathf.Sign(a0) == Mathf.Sign(a1))
                    return i0 * step;
                float t = Mathf.Abs(a0) / (Mathf.Abs(a0) + Mathf.Abs(a1));
                return Mathf.Lerp(i0 * step, i1 * step, t);
            }
            return m_VerticalAxis.Value; // stay conservative
        }

        /// <summary>Internal use only.  Called by CinemachineCore at designated update time
        /// so the vcam can position itself and track its targets.  All 3 child rigs are updated,
        /// and a blend calculated, depending on the value of the Y axis.</summary>
        /// <param name="worldUp">Default world Up, set by the CinemachineBrain</param>
        /// <param name="deltaTime">Delta time for time-based effects (ignore if less than 0)</param>
        override public void InternalUpdateCameraState(Vector3 worldUp, float deltaTime)
        {
            UpdateTargetCache();

            FollowTargetAttachment = 1;
            LookAtTargetAttachment = 1;

            // Initialize the camera state, in case the game object got moved in the editor
            m_State = PullStateFromVirtualCamera(worldUp, ref m_Lens);
            m_Rigs[(int)RigID.Top].m_Lens.SnapshotCameraReadOnlyProperties(ref m_Lens);
            m_Rigs[(int)RigID.Bottom].m_Lens.SnapshotCameraReadOnlyProperties(ref m_Lens);

            // Update our axes
            bool activeCam = PreviousStateIsValid || CinemachineCore.Instance.IsLive(this);
            if (!activeCam || deltaTime < 0)
                m_VerticalAxis.m_Recentering.DoRecentering(ref m_VerticalAxis, -1, 0.5f);
            else
            {
                if (m_VerticalAxis.Update(deltaTime))
                    m_VerticalAxis.m_Recentering.CancelRecentering();
                m_RadialAxis.Update(deltaTime);
                m_VerticalAxis.m_Recentering.DoRecentering(ref m_VerticalAxis, deltaTime, 0.5f);
            }

            // Blend the components
            if (mBlender == null)
                mBlender = new ComponentBlender(this);
            mBlender.Blend(GetVerticalAxisValue());

            // Blend the lens
            if (m_Rigs[mBlender.OtherRig].m_CustomLens)
                m_State.Lens = LensSettings.Lerp(
                    m_State.Lens, m_Rigs[mBlender.OtherRig].m_Lens, mBlender.BlendAmount);

            // Do our stuff
            SetReferenceLookAtTargetInState(ref m_State);
            InvokeComponentPipeline(ref m_State, worldUp, deltaTime);
            ApplyPositionBlendMethod(ref m_State, m_Transitions.m_BlendHint);

            // Restore the components
            mBlender.Restore();

            // Push the raw position back to the game object's transform, so it
            // moves along with the camera.
            if (Follow != null)
                transform.position = State.RawPosition;
            if (LookAt != null)
                transform.rotation = State.RawOrientation;
            
            // Signal that it's all done
            InvokePostPipelineStageCallback(this, CinemachineCore.Stage.Finalize, ref m_State, deltaTime);
            PreviousStateIsValid = true;
        }

        ComponentBlender mBlender;

        protected override void OnComponentCacheUpdated()
        {
            var transposer = Transposer;
            if (transposer != null)
            {
                transposer.HideOffsetInInspector = true;
                transposer.m_FollowOffset = new Vector3(
                    0, m_Orbits[1].m_Height, -m_Orbits[1].m_Radius);
            }
        }

        private float GetVerticalAxisValue()
        {
            float range = m_VerticalAxis.m_MaxValue - m_VerticalAxis.m_MinValue;
            return (range > UnityVectorExtensions.Epsilon) ? m_VerticalAxis.Value / range : 0.5f;
        }

        /// <summary>
        /// Returns the local position of the camera along the spline used to connect the
        /// three camera rigs. Does not take into account the current heading of the
        /// camera (or its target)
        /// </summary>
        /// <param name="t">The t-value for the camera on its spline. Internally clamped to
        /// the value [0,1]</param>
        /// <returns>The local offset (back + up) of the camera WRT its target based on the
        /// supplied t-value</returns>
        public Vector3 GetLocalPositionForCameraFromInput(float t)
        {
            UpdateCachedSpline();
            int n = 1;
            if (t > 0.5f)
            {
                t -= 0.5f;
                n = 2;
            }
            Vector3 pos = SplineHelpers.Bezier3(
                t * 2f, m_CachedKnots[n], m_CachedCtrl1[n], m_CachedCtrl2[n], m_CachedKnots[n+1]);
            pos *= Mathf.Max(0, m_RadialAxis.Value);
            return pos;
        }

        Vector2[] m_CachedOrbits;
        float m_CachedTension;
        Vector4[] m_CachedKnots;
        Vector4[] m_CachedCtrl1;
        Vector4[] m_CachedCtrl2;
        void UpdateCachedSpline()
        {
            bool cacheIsValid = (m_CachedOrbits != null && m_CachedTension == m_SplineCurvature);
            for (int i = 0; i < 3 && cacheIsValid; ++i)
                cacheIsValid = (m_CachedOrbits[i].y == m_Orbits[i].m_Height
                    && m_CachedOrbits[i].x == m_Orbits[i].m_Radius);
            if (!cacheIsValid)
            {
                float t = m_SplineCurvature;
                m_CachedKnots = new Vector4[5];
                m_CachedCtrl1 = new Vector4[5];
                m_CachedCtrl2 = new Vector4[5];
                m_CachedKnots[1] = new Vector4(0, m_Orbits[2].m_Height, -m_Orbits[2].m_Radius, 0);
                m_CachedKnots[2] = new Vector4(0, m_Orbits[1].m_Height, -m_Orbits[1].m_Radius, 0);
                m_CachedKnots[3] = new Vector4(0, m_Orbits[0].m_Height, -m_Orbits[0].m_Radius, 0);
                m_CachedKnots[0] = Vector4.Lerp(m_CachedKnots[0], Vector4.zero, t);
                m_CachedKnots[4] = Vector4.Lerp(m_CachedKnots[3], Vector4.zero, t);
                SplineHelpers.ComputeSmoothControlPoints(
                    ref m_CachedKnots, ref m_CachedCtrl1, ref m_CachedCtrl2);
                m_CachedOrbits = new Vector2[3];
                for (int i = 0; i < 3; ++i)
                    m_CachedOrbits[i] = new Vector2(m_Orbits[i].m_Radius, m_Orbits[i].m_Height);
                m_CachedTension = m_SplineCurvature;
            }
        }

        // Crazy damn thing for blending components at the source level
        internal class ComponentBlender
        {
            Rig.TransposerSettings orbitalSaved = new Rig.TransposerSettings();
            Rig.ComposerSettings composerSaved = new Rig.ComposerSettings();
            Rig.PerlinNoiseSettings noiseSaved = new Rig.PerlinNoiseSettings();

            public int OtherRig;
            public float BlendAmount;
            CinemachineNewFreeLook mFreeLook;

            public ComponentBlender(CinemachineNewFreeLook freeLook) { mFreeLook = freeLook; }

            public void Blend(float y)
            {
                if (y < 0.5f)
                {
                    BlendAmount = 1 - (y * 2);
                    OtherRig = (int)RigID.Bottom;
                }
                else
                {
                    BlendAmount = (y - 0.5f) * 2f;
                    OtherRig = (int)RigID.Top;
                }

                var orbital = mFreeLook.Transposer;
                if (orbital != null && mFreeLook.m_Rigs[OtherRig].m_CustomBody)
                {
                    orbitalSaved.PullFrom(orbital);
                    mFreeLook.m_Rigs[OtherRig].m_Body.Lerp(orbital, BlendAmount);
                }
                if (orbital != null)
                    orbital.m_FollowOffset = mFreeLook.GetLocalPositionForCameraFromInput(y);

                var components = mFreeLook.ComponentCache;
                var composer = components[(int)CinemachineCore.Stage.Aim] as CinemachineComposer;
                if (composer != null && mFreeLook.m_Rigs[OtherRig].m_CustomAim)
                {
                    composerSaved.PullFrom(composer);
                    mFreeLook.m_Rigs[OtherRig].m_Aim.Lerp(composer, BlendAmount);
                }

                var noise = components[(int)CinemachineCore.Stage.Noise] as CinemachineBasicMultiChannelPerlin;
                if (noise != null && mFreeLook.m_Rigs[OtherRig].m_CustomNoise)
                {
                    noiseSaved.PullFrom(noise);
                    mFreeLook.m_Rigs[OtherRig].m_Noise.Lerp(noise, BlendAmount);
                }
            }

            public void Restore()
            {
                var orbital = mFreeLook.Transposer;
                if (orbital != null && mFreeLook.m_Rigs[OtherRig].m_CustomBody)
                    orbitalSaved.PushTo(orbital);
                if (orbital != null)
                    orbital.m_FollowOffset = new Vector3(
                        0, mFreeLook.m_Orbits[1].m_Height, -mFreeLook.m_Orbits[1].m_Radius);
                var components = mFreeLook.ComponentCache;
                var composer = components[(int)CinemachineCore.Stage.Aim] as CinemachineComposer;
                if (composer != null && mFreeLook.m_Rigs[OtherRig].m_CustomAim)
                    composerSaved.PushTo(composer);

                var noise = components[(int)CinemachineCore.Stage.Noise] as CinemachineBasicMultiChannelPerlin;
                if (noise != null && mFreeLook.m_Rigs[OtherRig].m_CustomNoise)
                    noiseSaved.PushTo(noise);
            }
        }
    }
}
#endif

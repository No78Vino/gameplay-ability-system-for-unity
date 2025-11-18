using UnityEngine;
using Cinemachine.Utility;
using UnityEngine.Serialization;
using System;

namespace Cinemachine
{
    /// <summary>
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
    [ExcludeFromPreset]
    [AddComponentMenu("Cinemachine/CinemachineFreeLook")]
    [HelpURL(Documentation.BaseURL + "manual/CinemachineFreeLook.html")]
    public class CinemachineFreeLook : CinemachineVirtualCameraBase
    {
        /// <summary>Object for the camera children to look at (the aim target)</summary>
        [Tooltip("Object for the camera children to look at (the aim target).")]
        [NoSaveDuringPlay]
        [VcamTargetProperty]
        public Transform m_LookAt = null;

        /// <summary>Object for the camera children wants to move with (the body target)</summary>
        [Tooltip("Object for the camera children wants to move with (the body target).")]
        [NoSaveDuringPlay]
        [VcamTargetProperty]
        public Transform m_Follow = null;

        /// <summary>If enabled, this lens setting will apply to all three child rigs, 
        /// otherwise the child rig lens settings will be used</summary>
        [Tooltip("If enabled, this lens setting will apply to all three child rigs, "
            + "otherwise the child rig lens settings will be used")]
        [FormerlySerializedAs("m_UseCommonLensSetting")]
        public bool m_CommonLens = true;

        /// <summary>Specifies the lens properties of this Virtual Camera.
        /// This generally mirrors the Unity Camera's lens settings, and will be used to drive
        /// the Unity camera when the vcam is active</summary>
        [FormerlySerializedAs("m_LensAttributes")]
        [Tooltip("Specifies the lens properties of this Virtual Camera.  This generally "
            + "mirrors the Unity Camera's lens settings, and will be used to drive the "
            + "Unity camera when the vcam is active")]
        public LensSettings m_Lens = LensSettings.Default;

        /// <summary> Collection of parameters that influence how this virtual camera transitions from
        /// other virtual cameras </summary>
        public TransitionParams m_Transitions;

        /// <summary>Legacy support</summary>
        [SerializeField] [HideInInspector]
        [FormerlySerializedAs("m_BlendHint")]
        [FormerlySerializedAs("m_PositionBlending")] private BlendHint m_LegacyBlendHint;

        /// <summary>The Vertical axis.  Value is 0..1.  Chooses how to blend the child rigs</summary>
        [Header("Axis Control")]
        [Tooltip("The Vertical axis.  Value is 0..1.  Chooses how to blend the child rigs")]
        [AxisStateProperty]
        public AxisState m_YAxis = new AxisState(0, 1, false, true, 2f, 0.2f, 0.1f, "Mouse Y", false);

        /// <summary>Controls how automatic recentering of the Y axis is accomplished</summary>
        [Tooltip("Controls how automatic recentering of the Y axis is accomplished")]
        public AxisState.Recentering m_YAxisRecentering = new AxisState.Recentering(false, 1, 2);

        /// <summary>The Horizontal axis.  Value is -180...180.  This is passed on to 
        /// the rigs' OrbitalTransposer component</summary>
        [Tooltip("The Horizontal axis.  Value is -180...180.  "
            + "This is passed on to the rigs' OrbitalTransposer component")]
        [AxisStateProperty]
        public AxisState m_XAxis = new AxisState(-180, 180, true, false, 300f, 0.1f, 0.1f, "Mouse X", true);

        /// <summary>The definition of Forward.  Camera will follow behind</summary>
        [OrbitalTransposerHeadingProperty]
        [Tooltip("The definition of Forward.  Camera will follow behind.")]
        public CinemachineOrbitalTransposer.Heading m_Heading
            = new CinemachineOrbitalTransposer.Heading(
                CinemachineOrbitalTransposer.Heading.HeadingDefinition.TargetForward, 4, 0);

        /// <summary>Controls how automatic recentering of the X axis is accomplished</summary>
        [Tooltip("Controls how automatic recentering of the X axis is accomplished")]
        public AxisState.Recentering m_RecenterToTargetHeading = new AxisState.Recentering(false, 1, 2);

        /// <summary>The coordinate space to use when interpreting the offset from the target</summary>
        [Header("Orbits")]
        [Tooltip("The coordinate space to use when interpreting the offset from the target.  "
            + "This is also used to set the camera's Up vector, which will be maintained "
            + "when aiming the camera.")]
        public CinemachineOrbitalTransposer.BindingMode m_BindingMode
            = CinemachineOrbitalTransposer.BindingMode.SimpleFollowWithWorldUp;

        /// <summary></summary>
        [Tooltip("Controls how taut is the line that connects the rigs' orbits, which "
            + "determines final placement on the Y axis")]
        [Range(0f, 1f)]
        [FormerlySerializedAs("m_SplineTension")]
        public float m_SplineCurvature = 0.2f;

        /// <summary>Defines the height and radius of the Rig orbit</summary>
        [Serializable]
        public struct Orbit
        {
            /// <summary>Height relative to target</summary>
            public float m_Height;
            /// <summary>Radius of orbit</summary>
            public float m_Radius;
            /// <summary>Constructor with specific values</summary>
            /// <param name="h">Orbit height</param>
            /// <param name="r">Orbit radius</param>
            public Orbit(float h, float r) { m_Height = h; m_Radius = r; }
        }

        /// <summary>The radius and height of the three orbiting rigs</summary>
        [Tooltip("The radius and height of the three orbiting rigs.")]
        public Orbit[] m_Orbits = new Orbit[3]
        {
            // These are the default orbits
            new Orbit(4.5f, 1.75f),
            new Orbit(2.5f, 3f),
            new Orbit(0.4f, 1.3f)
        };

        // Legacy support
        [SerializeField] [HideInInspector] [FormerlySerializedAs("m_HeadingBias")]
        private float m_LegacyHeadingBias = float.MaxValue;
        bool mUseLegacyRigDefinitions = false;

        /// <summary>Enforce bounds for fields, when changed in inspector.</summary>
        protected override void OnValidate()
        {
            base.OnValidate();

            // Upgrade after a legacy deserialize
            if (m_LegacyHeadingBias != float.MaxValue)
            {
                m_Heading.m_Bias= m_LegacyHeadingBias;
                m_LegacyHeadingBias = float.MaxValue;
                int heading = (int)m_Heading.m_Definition;
                if (m_RecenterToTargetHeading.LegacyUpgrade(ref heading, ref m_Heading.m_VelocityFilterStrength))
                    m_Heading.m_Definition = (CinemachineOrbitalTransposer.Heading.HeadingDefinition)heading;
                mUseLegacyRigDefinitions = true;
            }
            if (m_LegacyBlendHint != BlendHint.None)
            {
                m_Transitions.m_BlendHint = m_LegacyBlendHint;
                m_LegacyBlendHint = BlendHint.None;
            }
            m_YAxis.Validate();
            m_XAxis.Validate();
            m_RecenterToTargetHeading.Validate();
            m_YAxisRecentering.Validate();
            m_Lens.Validate();

            InvalidateRigCache();
            
#if UNITY_EDITOR
            for (int i = 0; m_Rigs != null && i < 3 && i < m_Rigs.Length; ++i)
                if (m_Rigs[i] != null)
                    CinemachineVirtualCamera.SetFlagsForHiddenChild(m_Rigs[i].gameObject);
#endif
        }

        /// <summary>Get a child rig</summary>
        /// <param name="i">Rig index.  Can be 0, 1, or 2</param>
        /// <returns>The rig, or null if index is bad.</returns>
        public CinemachineVirtualCamera GetRig(int i)
        {
            UpdateRigCache();
            return (i < 0 || i > 2) ? null : m_Rigs[i];
        }

        /// <summary>Names of the 3 child rigs</summary>
        public static string[] RigNames { get { return new string[] { "TopRig", "MiddleRig", "BottomRig" }; } }

        bool mIsDestroyed = false;

        /// <summary>Updates the child rig cache</summary>
        protected override void OnEnable()
        {
            mIsDestroyed = false;
            base.OnEnable();
            InvalidateRigCache();
            UpdateInputAxisProvider();
        }

        /// <summary>
        /// API for the inspector.  Internal use only
        /// </summary>
        public void UpdateInputAxisProvider()
        {
            m_XAxis.SetInputAxisProvider(0, null);
            m_YAxis.SetInputAxisProvider(1, null);
            var provider = GetInputAxisProvider();
            if (provider != null)
            {
                m_XAxis.SetInputAxisProvider(0, provider);
                m_YAxis.SetInputAxisProvider(1, provider);
            }
        }

        /// <summary>Makes sure that the child rigs get destroyed in an undo-firndly manner.
        /// Invalidates the rig cache.</summary>
        protected override void OnDestroy()
        {
            // Make the rigs visible instead of destroying - this is to keep Undo happy
            if (m_Rigs != null)
                foreach (var rig in m_Rigs)
                    if (rig != null && rig.gameObject != null)
                        rig.gameObject.hideFlags
                            &= ~(HideFlags.HideInHierarchy | HideFlags.HideInInspector);

            mIsDestroyed = true;
            base.OnDestroy();
        }

        /// <summary>Invalidates the rig cache</summary>
        void OnTransformChildrenChanged()
        {
            InvalidateRigCache();
        }

        void Reset()
        {
#if UNITY_EDITOR
            if (RuntimeUtility.IsPrefab(gameObject))
            {
                Debug.Log("You cannot reset a prefab instance.  "
                    + "First disconnect this instance from the prefab, or enter Prefab Edit mode");
                return;
            }
#endif
            DestroyRigs();
        }

        /// <summary>Set this to force the next update to ignore deltaTime and reset itself</summary>
        public override bool PreviousStateIsValid
        {
            get { return base.PreviousStateIsValid; }
            set
            {
                if (value == false)
                    for (int i = 0; m_Rigs != null && i < m_Rigs.Length; ++i)
                        if (m_Rigs[i] != null)
                            m_Rigs[i].PreviousStateIsValid = value;
                base.PreviousStateIsValid = value;
            }
        }

        /// <summary>The cacmera state, which will be a blend of the child rig states</summary>
        override public CameraState State { get { return m_State; } }

        /// <summary>Get the current LookAt target.  Returns parent's LookAt if parent
        /// is non-null and no specific LookAt defined for this camera</summary>
        override public Transform LookAt
        {
            get { return ResolveLookAt(m_LookAt); }
            set { m_LookAt = value; }
        }

        /// <summary>Get the current Follow target.  Returns parent's Follow if parent
        /// is non-null and no specific Follow defined for this camera</summary>
        override public Transform Follow
        {
            get { return ResolveFollow(m_Follow); }
            set { m_Follow = value; }
        }

        /// <summary>Check whether the vcam a live child of this camera.
        /// Returns true if the child is currently contributing actively to the camera state.</summary>
        /// <param name="vcam">The Virtual Camera to check</param>
        /// <param name="dominantChildOnly">If truw, will only return true if this vcam is the dominat live child</param>
        /// <returns>True if the vcam is currently actively influencing the state of this vcam</returns>
        public override bool IsLiveChild(ICinemachineCamera vcam, bool dominantChildOnly = false)
        {
            // Do not update the rig cache here or there will be infinite loop at creation time
            if (m_Rigs == null || m_Rigs.Length != 3)
                return false;
            var y = GetYAxisValue();
            if (dominantChildOnly)
            {
                if (vcam == (ICinemachineCamera)m_Rigs[0])
                    return y > 0.666f;
                if (vcam == (ICinemachineCamera)m_Rigs[2])
                    return y < 0.333;
                if (vcam == (ICinemachineCamera)m_Rigs[1])
                    return y >= 0.333f && y <= 0.666f;
                return false;
            }
            if (vcam == (ICinemachineCamera)m_Rigs[1])
                return true;
            if (y < 0.5f)
                return vcam == (ICinemachineCamera)m_Rigs[2];
            return vcam == (ICinemachineCamera)m_Rigs[0];
        }

        /// <summary>This is called to notify the vcam that a target got warped,
        /// so that the vcam can update its internal state to make the camera
        /// also warp seamlessy.</summary>
        /// <param name="target">The object that was warped</param>
        /// <param name="positionDelta">The amount the target's position changed</param>
        public override void OnTargetObjectWarped(Transform target, Vector3 positionDelta)
        {
            UpdateRigCache();
            if (m_Rigs != null)
                foreach (var vcam in m_Rigs)
                    vcam.OnTargetObjectWarped(target, positionDelta);
            base.OnTargetObjectWarped(target, positionDelta);
        }

        /// <summary>
        /// Force the virtual camera to assume a given position and orientation.  
        /// Procedural placement then takes over
        /// </summary>
        /// <param name="pos">Worldspace pposition to take</param>
        /// <param name="rot">Worldspace orientation to take</param>
        public override void ForceCameraPosition(Vector3 pos, Quaternion rot)
        {
            var up = State.ReferenceUp;
            m_YAxis.Value = GetYAxisClosestValue(pos, up);

            PreviousStateIsValid = true;
            transform.position = pos;
            transform.rotation = rot;
            m_State.RawPosition = pos;
            m_State.RawOrientation = rot;

            UpdateRigCache();
            if (m_BindingMode != CinemachineTransposer.BindingMode.SimpleFollowWithWorldUp)
                m_XAxis.Value = mOrbitals[1].GetAxisClosestValue(pos, up);

            PushSettingsToRigs();
            for (int i = 0; i < 3; ++i)
                m_Rigs[i].ForceCameraPosition(pos, rot);

            InternalUpdateCameraState(up, -1);

            base.ForceCameraPosition(pos, rot);
        }
        
        /// <summary>Internal use only.  Called by CinemachineCore at designated update time
        /// so the vcam can position itself and track its targets.  All 3 child rigs are updated,
        /// and a blend calculated, depending on the value of the Y axis.</summary>
        /// <param name="worldUp">Default world Up, set by the CinemachineBrain</param>
        /// <param name="deltaTime">Delta time for time-based effects (ignore if less than 0)</param>
        override public void InternalUpdateCameraState(Vector3 worldUp, float deltaTime)
        {
            UpdateTargetCache();
            UpdateRigCache();

            // Update the current state by invoking the component pipeline
            m_State = CalculateNewState(worldUp, deltaTime);
            ApplyPositionBlendMethod(ref m_State, m_Transitions.m_BlendHint);

            // Push the raw position back to the game object's transform, so it
            // moves along with the camera.  Leave the orientation alone, because it
            // screws up camera dragging when there is a LookAt behaviour.
            if (Follow != null)
            {
                Vector3 delta = State.RawPosition - transform.position;
                transform.position = State.RawPosition;
                m_Rigs[0].transform.position -= delta;
                m_Rigs[1].transform.position -= delta;
                m_Rigs[2].transform.position -= delta;
            }

            InvokePostPipelineStageCallback(this, CinemachineCore.Stage.Finalize, ref m_State, deltaTime);
            PreviousStateIsValid = true;

            // Set up for next frame
            bool activeCam = PreviousStateIsValid && CinemachineCore.Instance.IsLive(this);
            if (activeCam && deltaTime >= 0)
            {
                if (m_YAxis.Update(deltaTime))
                    m_YAxisRecentering.CancelRecentering();
            }
            PushSettingsToRigs();
            if (m_BindingMode == CinemachineTransposer.BindingMode.SimpleFollowWithWorldUp)
                m_XAxis.Value = 0;
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
            bool forceUpdate = false;
//            m_RecenterToTargetHeading.DoRecentering(ref m_XAxis, -1, 0);
//              m_YAxis.m_Recentering.DoRecentering(ref m_YAxis, -1, 0.5f);
//            m_RecenterToTargetHeading.CancelRecentering();
//            m_YAxis.m_Recentering.CancelRecentering();
            if (fromCam != null && m_Transitions.m_InheritPosition 
                && !CinemachineCore.Instance.IsLiveInBlend(this))
            {
                var cameraPos = fromCam.State.RawPosition;

                // Special handling for FreeLook: get an undamped outgoing position
                if (fromCam is CinemachineFreeLook)
                {
                    var flFrom = (fromCam as CinemachineFreeLook);
                    var orbital = flFrom.mOrbitals != null ? flFrom.mOrbitals[1] : null;
                    if (orbital != null)
                        cameraPos = orbital.GetTargetCameraPosition(worldUp);
                }
                ForceCameraPosition(cameraPos, fromCam.State.FinalOrientation);
            }
            if (forceUpdate)
            {
                for (int i = 0; i < 3; ++i)
                    m_Rigs[i].InternalUpdateCameraState(worldUp, deltaTime);
                InternalUpdateCameraState(worldUp, deltaTime);
            }
            else
                UpdateCameraState(worldUp, deltaTime);
            if (m_Transitions.m_OnCameraLive != null)
                m_Transitions.m_OnCameraLive.Invoke(this, fromCam);
        }
        
        /// <summary>
        /// Returns true, because FreeLook requires input.
        /// </summary>
        internal override bool RequiresUserInput()
        {
            return true;
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
            return m_YAxis.Value; // stay conservative
        }

        CameraState m_State = CameraState.Default;          // Current state this frame

        /// Serialized in order to support copy/paste
        [SerializeField][HideInInspector][NoSaveDuringPlay]
        CinemachineVirtualCamera[] m_Rigs = new CinemachineVirtualCamera[3];

        void InvalidateRigCache() { mOrbitals = null; }
        CinemachineOrbitalTransposer[] mOrbitals = null;
        CinemachineBlend mBlendA;
        CinemachineBlend mBlendB;

        /// <summary>
        /// Override component pipeline creation.
        /// This needs to be done by the editor to support Undo.
        /// The override must do exactly the same thing as the CreatePipeline method in this class.
        /// </summary>
        public static CreateRigDelegate CreateRigOverride;

        /// <summary>
        /// Override component pipeline creation.
        /// This needs to be done by the editor to support Undo.
        /// The override must do exactly the same thing as the CreatePipeline method in this class.
        /// </summary>
        public delegate CinemachineVirtualCamera CreateRigDelegate(
            CinemachineFreeLook vcam, string name, CinemachineVirtualCamera copyFrom);

        /// <summary>
        /// Override component pipeline destruction.
        /// This needs to be done by the editor to support Undo.
        /// </summary>
        public static DestroyRigDelegate DestroyRigOverride;

        /// <summary>
        /// Override component pipeline destruction.
        /// This needs to be done by the editor to support Undo.
        /// </summary>
        public delegate void DestroyRigDelegate(GameObject rig);

        private void DestroyRigs()
        {
            CinemachineVirtualCamera[] oldRigs = new CinemachineVirtualCamera[RigNames.Length];
            for (int i = 0; i < RigNames.Length; ++i)
            {
                foreach (Transform child in transform)
                    if (child.gameObject.name == RigNames[i])
                        oldRigs[i] = child.GetComponent<CinemachineVirtualCamera>();
            }
            for (int i = 0; i < oldRigs.Length; ++i)
            {
                if (oldRigs[i] != null)
                {
                    if (DestroyRigOverride != null)
                        DestroyRigOverride(oldRigs[i].gameObject);
                    else
                        Destroy(oldRigs[i].gameObject);
                }
            }
            m_Rigs = null;
            mOrbitals = null;
        }

        private CinemachineVirtualCamera[] CreateRigs(CinemachineVirtualCamera[] copyFrom)
        {
            // Invalidate the cache
            mOrbitals = null;
            float[] softCenterDefaultsV = new float[] { 0.5f, 0.55f, 0.6f };
            CinemachineVirtualCamera[] newRigs = new CinemachineVirtualCamera[3];
            for (int i = 0; i < RigNames.Length; ++i)
            {
                CinemachineVirtualCamera src = null;
                if (copyFrom != null && copyFrom.Length > i)
                    src = copyFrom[i];

                if (CreateRigOverride != null)
                    newRigs[i] = CreateRigOverride(this, RigNames[i], src);
                else
                {
                    // Create a new rig with default components
                    // Note: copyFrom only supported in Editor, not build
                    GameObject go = new GameObject(RigNames[i]);
                    go.transform.parent = transform;
                    newRigs[i] = go.AddComponent<CinemachineVirtualCamera>();
                    go = newRigs[i].GetComponentOwner().gameObject;
                    go.AddComponent<CinemachineOrbitalTransposer>();
                    go.AddComponent<CinemachineComposer>();
                }

                // Set up the defaults
                newRigs[i].InvalidateComponentPipeline();
                CinemachineOrbitalTransposer orbital = newRigs[i].GetCinemachineComponent<CinemachineOrbitalTransposer>();
                if (orbital == null)
                    orbital = newRigs[i].AddCinemachineComponent<CinemachineOrbitalTransposer>(); // should not happen
                if (src == null)
                {
                    // Only set defaults if not copying
                    orbital.m_YawDamping = 0;
                    CinemachineComposer composer = newRigs[i].GetCinemachineComponent<CinemachineComposer>();
                    if (composer != null)
                    {
                        composer.m_HorizontalDamping = composer.m_VerticalDamping = 0;
                        composer.m_ScreenX = 0.5f;
                        composer.m_ScreenY = softCenterDefaultsV[i];
                        composer.m_DeadZoneWidth = composer.m_DeadZoneHeight = 0f;
                        composer.m_SoftZoneWidth = composer.m_SoftZoneHeight = 0.8f;
                        composer.m_BiasX = composer.m_BiasY = 0;
                    }
                }
            }
            return newRigs;
        }

        private void UpdateRigCache()
        {
            if (mIsDestroyed)
                return;

            bool isPrefab = RuntimeUtility.IsPrefab(gameObject);

#if UNITY_EDITOR
            // Special condition: Did we just get copy/pasted?
            if (m_Rigs != null && m_Rigs.Length == 3
                && m_Rigs[0] != null && m_Rigs[0].transform.parent != transform)
            {
                if (!isPrefab) // can't paste to a prefab
                {
                    var copyFrom = m_Rigs;
                    DestroyRigs();
                    m_Rigs = CreateRigs(copyFrom);
                }
            }
#endif

            // Early out if we're up to date
            if (mOrbitals != null && mOrbitals.Length == 3)
                return;

            // Locate existing rigs, and recreate them if any are missing
            if (LocateExistingRigs(RigNames, false) != 3 && !isPrefab)
            {
                DestroyRigs();
                m_Rigs = CreateRigs(null);
                LocateExistingRigs(RigNames, true);
            }

#if UNITY_EDITOR
            foreach (var rig in m_Rigs)
            {
                // Configure the UI
                if (rig == null)
                    continue;
                rig.m_ExcludedPropertiesInInspector = m_CommonLens
                    ? new string[] { "m_Script", "Header", "Extensions", "m_Priority", "m_Transitions", "m_Follow", "m_StandbyUpdate", "m_Lens" }
                    : new string[] { "m_Script", "Header", "Extensions", "m_Priority", "m_Transitions", "m_Follow", "m_StandbyUpdate" };
                rig.m_LockStageInInspector = new CinemachineCore.Stage[] { CinemachineCore.Stage.Body };
            }
#endif

            // Create the blend objects
            mBlendA = new CinemachineBlend(m_Rigs[1], m_Rigs[0], AnimationCurve.Linear(0, 0, 1, 1), 1, 0);
            mBlendB = new CinemachineBlend(m_Rigs[2], m_Rigs[1], AnimationCurve.Linear(0, 0, 1, 1), 1, 0);
        }

        private int LocateExistingRigs(string[] rigNames, bool forceOrbital)
        {
            m_CachedXAxisHeading = 0;
            mOrbitals = new CinemachineOrbitalTransposer[rigNames.Length];
            m_Rigs = new CinemachineVirtualCamera[rigNames.Length];
            int rigsFound = 0;
            foreach (Transform child in transform)
            {
                CinemachineVirtualCamera vcam = child.GetComponent<CinemachineVirtualCamera>();
                if (vcam != null)
                {
                    GameObject go = child.gameObject;
                    for (int i = 0; i < rigNames.Length; ++i)
                    {
                        if (mOrbitals[i] == null && go.name == rigNames[i])
                        {
                            // Must have an orbital transposer or it's no good
                            mOrbitals[i] = vcam.GetCinemachineComponent<CinemachineOrbitalTransposer>();
                            if (mOrbitals[i] == null && forceOrbital)
                                mOrbitals[i] = vcam.AddCinemachineComponent<CinemachineOrbitalTransposer>();
                            if (mOrbitals[i] != null)
                            {
                                mOrbitals[i].m_HeadingIsSlave = true;
                                mOrbitals[i].HideOffsetInInspector = true;
                                mOrbitals[i].m_XAxis.m_InputAxisName = string.Empty;
                                mOrbitals[i].HeadingUpdater = UpdateXAxisHeading;
                                mOrbitals[i].m_RecenterToTargetHeading.m_enabled = false;
                                m_Rigs[i] = vcam;
                                m_Rigs[i].m_StandbyUpdate = m_StandbyUpdate;
                                ++rigsFound;
                            }
                        }
                    }
                }
            }
            return rigsFound;
        }

        float m_CachedXAxisHeading;

        float UpdateXAxisHeading(CinemachineOrbitalTransposer orbital, float deltaTime, Vector3 up)
        {
            if (this == null)   
                return 0; // deleted
            if (mOrbitals != null && mOrbitals[1] == orbital)
            {
                var oldValue = m_XAxis.Value;
                m_CachedXAxisHeading = orbital.UpdateHeading(
                    PreviousStateIsValid ? deltaTime : -1, up,
                    ref m_XAxis, ref m_RecenterToTargetHeading,
                    CinemachineCore.Instance.IsLive(this));
                // Allow externally-driven values to work in this mode
                if (m_BindingMode == CinemachineTransposer.BindingMode.SimpleFollowWithWorldUp)
                    m_XAxis.Value = oldValue;
            }
            return m_CachedXAxisHeading;
        }

        void PushSettingsToRigs()
        {
            UpdateRigCache();
            for (int i = 0; i < m_Rigs.Length; ++i)
            {
                if (m_Rigs[i] == null)
                    continue;

                if (m_CommonLens)
                    m_Rigs[i].m_Lens = m_Lens;

                // If we just deserialized from a legacy version,
                // pull the orbits and targets from the rigs
                if (mUseLegacyRigDefinitions)
                {
                    mUseLegacyRigDefinitions = false;
                    m_Orbits[i].m_Height = mOrbitals[i].m_FollowOffset.y;
                    m_Orbits[i].m_Radius = -mOrbitals[i].m_FollowOffset.z;
                    if (m_Rigs[i].Follow != null)
                        Follow = m_Rigs[i].Follow;
                }
                m_Rigs[i].Follow = null;
                m_Rigs[i].m_StandbyUpdate = m_StandbyUpdate;
                m_Rigs[i].FollowTargetAttachment = FollowTargetAttachment;
                m_Rigs[i].LookAtTargetAttachment = LookAtTargetAttachment;
                if (!PreviousStateIsValid)
                {
                    m_Rigs[i].PreviousStateIsValid = false;
                    m_Rigs[i].transform.position = transform.position;
                    m_Rigs[i].transform.rotation = transform.rotation;
                }
#if UNITY_EDITOR
                // Hide the rigs from prying eyes
                CinemachineVirtualCamera.SetFlagsForHiddenChild(m_Rigs[i].gameObject);
#endif
                mOrbitals[i].m_FollowOffset = GetLocalPositionForCameraFromInput(GetYAxisValue());
                mOrbitals[i].m_BindingMode = m_BindingMode;
                mOrbitals[i].m_Heading = m_Heading;
                mOrbitals[i].m_XAxis.Value = m_XAxis.Value;

                // Hack to get SimpleFollow with heterogeneous dampings to work
                if (m_BindingMode == CinemachineTransposer.BindingMode.SimpleFollowWithWorldUp)
                    m_Rigs[i].SetStateRawPosition(State.RawPosition);
            }
        }

        private float GetYAxisValue()
        {
            float range = m_YAxis.m_MaxValue - m_YAxis.m_MinValue;
            return (range > UnityVectorExtensions.Epsilon) ? m_YAxis.Value / range : 0.5f;
        }

        private CameraState CalculateNewState(Vector3 worldUp, float deltaTime)
        {
            CameraState state = PullStateFromVirtualCamera(worldUp, ref m_Lens);
            m_YAxisRecentering.DoRecentering(ref m_YAxis, deltaTime, 0.5f);

            // Blend from the appropriate rigs
            float t = GetYAxisValue();
            if (t > 0.5f)
            {
                if (mBlendA != null)
                {
                    mBlendA.TimeInBlend = (t - 0.5f) * 2f;
                    mBlendA.UpdateCameraState(worldUp, deltaTime);
                    state = mBlendA.State;
                }
            }
            else
            {
                if (mBlendB != null)
                {
                    mBlendB.TimeInBlend = t * 2f;
                    mBlendB.UpdateCameraState(worldUp, deltaTime);
                    state = mBlendB.State;
                }
            }
            return state;
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
            if (mOrbitals == null)
                return Vector3.zero;
            UpdateCachedSpline();
            int n = 1;
            if (t > 0.5f)
            {
                t -= 0.5f;
                n = 2;
            }
            return SplineHelpers.Bezier3(
                t * 2f, m_CachedKnots[n], m_CachedCtrl1[n], m_CachedCtrl2[n], m_CachedKnots[n+1]);
        }

        Orbit[] m_CachedOrbits;
        float m_CachedTension;
        Vector4[] m_CachedKnots;
        Vector4[] m_CachedCtrl1;
        Vector4[] m_CachedCtrl2;
        void UpdateCachedSpline()
        {
            bool cacheIsValid = (m_CachedOrbits != null && m_CachedOrbits.Length == 3 
                && m_CachedTension == m_SplineCurvature);
            for (int i = 0; i < 3 && cacheIsValid; ++i)
                cacheIsValid = (m_CachedOrbits[i].m_Height == m_Orbits[i].m_Height
                    && m_CachedOrbits[i].m_Radius == m_Orbits[i].m_Radius);
            if (!cacheIsValid)
            {
                float t = m_SplineCurvature;
                m_CachedKnots = new Vector4[5];
                m_CachedCtrl1 = new Vector4[5];
                m_CachedCtrl2 = new Vector4[5];
                m_CachedKnots[1] = new Vector4(0, m_Orbits[2].m_Height, -m_Orbits[2].m_Radius, 0);
                m_CachedKnots[2] = new Vector4(0, m_Orbits[1].m_Height, -m_Orbits[1].m_Radius, 0);
                m_CachedKnots[3] = new Vector4(0, m_Orbits[0].m_Height, -m_Orbits[0].m_Radius, 0);
                m_CachedKnots[0] = Vector4.Lerp(m_CachedKnots[1], Vector4.zero, t);
                m_CachedKnots[4] = Vector4.Lerp(m_CachedKnots[3], Vector4.zero, t);
                SplineHelpers.ComputeSmoothControlPoints(
                    ref m_CachedKnots, ref m_CachedCtrl1, ref m_CachedCtrl2);
                m_CachedOrbits = new Orbit[3];
                for (int i = 0; i < 3; ++i)
                    m_CachedOrbits[i] = m_Orbits[i];
                m_CachedTension = m_SplineCurvature;
            }
        }
    }
}

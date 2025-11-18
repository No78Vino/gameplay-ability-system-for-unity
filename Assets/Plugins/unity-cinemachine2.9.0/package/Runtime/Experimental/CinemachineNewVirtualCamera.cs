#if CINEMACHINE_EXPERIMENTAL_VCAM
using UnityEngine;
using System;
using System.Linq;

namespace Cinemachine
{
    /// <summary>
    ///
    /// NOTE: THIS CLASS IS EXPERIMENTAL, AND NOT FOR PUBLIC USE
    ///
    /// Lighter-weight version of the CinemachineVirtualCamera.
    ///
    /// </summary>
    [DocumentationSorting(DocumentationSortingAttribute.Level.UserRef)]
    [DisallowMultipleComponent]
    [ExecuteAlways]
    [AddComponentMenu("Cinemachine/CinemachineNewVirtualCamera")]
    public class CinemachineNewVirtualCamera : CinemachineVirtualCameraBase
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

        /// <summary>Specifies the LensSettings of this Virtual Camera.
        /// These settings will be transferred to the Unity camera when the vcam is live.</summary>
        [Tooltip("Specifies the lens properties of this Virtual Camera.  This generally mirrors the "
            + "Unity Camera's lens settings, and will be used to drive the Unity camera when the vcam is active.")]
        public LensSettings m_Lens = LensSettings.Default;

        /// <summary> Collection of parameters that influence how this virtual camera transitions from
        /// other virtual cameras </summary>
        public TransitionParams m_Transitions;

        /// <summary>Updates the child rig cache</summary>
        protected override void OnEnable()
        {
            base.OnEnable();
            InvalidateComponentCache();
        }

        void Reset()
        {
            DestroyComponents();
        }

        /// <summary>Validates the settings avter inspector edit</summary>
        protected override void OnValidate()
        {
            base.OnValidate();
            m_Lens.Validate();
        }

        /// <summary>The camera state, which will be a blend of the child rig states</summary>
        override public CameraState State { get { return m_State; } }

        /// <summary>The camera state, which will be a blend of the child rig states</summary>
        protected CameraState m_State = CameraState.Default;

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

        /// <summary>This is called to notify the vcam that a target got warped,
        /// so that the vcam can update its internal state to make the camera
        /// also warp seamlessy.</summary>
        /// <param name="target">The object that was warped</param>
        /// <param name="positionDelta">The amount the target's position changed</param>
        public override void OnTargetObjectWarped(Transform target, Vector3 positionDelta)
        {
            if (target == Follow)
            {
                transform.position += positionDelta;
                m_State.RawPosition += positionDelta;
            }
            UpdateComponentCache();
            for (int i = 0; i < m_Components.Length; ++i)
            {
                if (m_Components[i] != null)
                    m_Components[i].OnTargetObjectWarped(target, positionDelta);
            }
            base.OnTargetObjectWarped(target, positionDelta);
        }

        /// <summary>
        /// Force the virtual camera to assume a given position and orientation
        /// </summary>
        /// <param name="pos">Worldspace pposition to take</param>
        /// <param name="rot">Worldspace orientation to take</param>
        public override void ForceCameraPosition(Vector3 pos, Quaternion rot)
        {
            PreviousStateIsValid = false;
            transform.position = pos;
            transform.rotation = rot;
            m_State.RawPosition = pos;
            m_State.RawOrientation = rot;

            UpdateComponentCache();
            for (int i = 0; i < m_Components.Length; ++i)
                if (m_Components[i] != null)
                    m_Components[i].ForceCameraPosition(pos, rot);

            base.ForceCameraPosition(pos, rot);
        }
        
        /// <summary>
        /// Query components and extensions for the maximum damping time.
        /// </summary>
        /// <returns>Highest damping setting in this vcam</returns>
        public override float GetMaxDampTime()
        {
            float maxDamp = base.GetMaxDampTime();
            UpdateComponentCache();
            for (int i = 0; i < m_Components.Length; ++i)
                if (m_Components[i] != null)
                    maxDamp = Mathf.Max(maxDamp, m_Components[i].GetMaxDampTime());
            return maxDamp;
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
            if (m_Transitions.m_InheritPosition && fromCam != null  
                && !CinemachineCore.Instance.IsLiveInBlend(this))
            {
                ForceCameraPosition(fromCam.State.FinalPosition, fromCam.State.FinalOrientation);
            }
            UpdateComponentCache();
            for (int i = 0; i < m_Components.Length; ++i)
            {
                if (m_Components[i] != null
                        && m_Components[i].OnTransitionFromCamera(
                            fromCam, worldUp, deltaTime, ref m_Transitions))
                    forceUpdate = true;
            }
            if (forceUpdate)
            {
                InternalUpdateCameraState(worldUp, deltaTime);
                InternalUpdateCameraState(worldUp, deltaTime);
            }
            else
                UpdateCameraState(worldUp, deltaTime);
            if (m_Transitions.m_OnCameraLive != null)
                m_Transitions.m_OnCameraLive.Invoke(this, fromCam);
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

            // Do our stuff
            SetReferenceLookAtTargetInState(ref m_State);
            InvokeComponentPipeline(ref m_State, worldUp, deltaTime);
            ApplyPositionBlendMethod(ref m_State, m_Transitions.m_BlendHint);

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
        
        /// <summary>
        /// Returns true, when the vcam has extensions or components that require input.
        /// </summary>
        internal override bool RequiresUserInput()
        {
            return base.RequiresUserInput() ||
                m_Components != null && m_Components.Any(t => t != null && t.RequiresUserInput);
        }

        private Transform mCachedLookAtTarget;
        private CinemachineVirtualCameraBase mCachedLookAtTargetVcam;

        /// <summary>Set the state's refeenceLookAt target to our lookAt, with some smarts
        /// in case our LookAt points to a vcam</summary>
        protected void SetReferenceLookAtTargetInState(ref CameraState state)
        {
            Transform lookAtTarget = LookAt;
            if (lookAtTarget != mCachedLookAtTarget)
            {
                mCachedLookAtTarget = lookAtTarget;
                mCachedLookAtTargetVcam = null;
                if (lookAtTarget != null)
                    mCachedLookAtTargetVcam = lookAtTarget.GetComponent<CinemachineVirtualCameraBase>();
            }
            if (lookAtTarget != null)
            {
                if (mCachedLookAtTargetVcam != null)
                    state.ReferenceLookAt = mCachedLookAtTargetVcam.State.FinalPosition;
                else
                    state.ReferenceLookAt = TargetPositionCache.GetTargetPosition(lookAtTarget);
            }
        }

        protected CameraState InvokeComponentPipeline(
            ref CameraState state, Vector3 worldUp, float deltaTime)
        {
            UpdateComponentCache();

            // Extensions first
            InvokePrePipelineMutateCameraStateCallback(this, ref state, deltaTime);

            // Apply the component pipeline
            for (CinemachineCore.Stage stage = CinemachineCore.Stage.Body;
                stage <= CinemachineCore.Stage.Finalize; ++stage)
            {
                var c = m_Components[(int)stage];
                if (c != null)
                    c.PrePipelineMutateCameraState(ref state, deltaTime);
            }
            CinemachineComponentBase postAimBody = null;
            for (CinemachineCore.Stage stage = CinemachineCore.Stage.Body;
                stage <= CinemachineCore.Stage.Finalize; ++stage)
            {
                var c = m_Components[(int)stage];
                if (c != null)
                {
                    if (stage == CinemachineCore.Stage.Body && c.BodyAppliesAfterAim)
                    {
                        postAimBody = c;
                        continue; // do the body stage of the pipeline after Aim
                    }
                    c.MutateCameraState(ref state, deltaTime);
                }
                InvokePostPipelineStageCallback(this, stage, ref state, deltaTime);
                if (stage == CinemachineCore.Stage.Aim)
                {
                    if (c == null)
                        state.BlendHint |= CameraState.BlendHintValue.IgnoreLookAtTarget; // no aim
                     // If we have saved a Body for after Aim, do it now
                    if (postAimBody != null)
                    {
                        postAimBody.MutateCameraState(ref state, deltaTime);
                        InvokePostPipelineStageCallback(this, CinemachineCore.Stage.Body, ref state, deltaTime);
                    }
                }
            }

            return state;
        }

        // Component Cache - serialized only for copy/paste
        [SerializeField, HideInInspector, NoSaveDuringPlay]
        CinemachineComponentBase[] m_Components;

        /// For inspector
        internal CinemachineComponentBase[] ComponentCache
        {
            get
            {
                UpdateComponentCache();
                return m_Components;
            }
        }

        /// <summary>Call this when CinemachineCompponentBase compponents are added
        /// or removed.  If you don't call this, you may get null reference errors.</summary>
        public void InvalidateComponentCache()
        {
            m_Components = null;
        }

        /// <summary>Bring the component cache up to date if needed</summary>
        protected void UpdateComponentCache()
        {
#if UNITY_EDITOR
            // Special case: if we have serialized in with some other game object's
            // components, then we have just been pasted so we should clone them
            for (int i = 0; m_Components != null && i < m_Components.Length; ++i)
            {
                if (m_Components[i] != null && m_Components[i].gameObject != gameObject)
                {
                    var copyFrom = m_Components;
                    DestroyComponents();
                    CopyComponents(copyFrom);
                    break;
                }
            }
#endif
            if (m_Components != null && m_Components.Length == (int)CinemachineCore.Stage.Finalize + 1)
                return; // up to date

            m_Components = new CinemachineComponentBase[(int)CinemachineCore.Stage.Finalize + 1];
            var existing = GetComponents<CinemachineComponentBase>();
            for (int i = 0; existing != null && i < existing.Length; ++i)
                m_Components[(int)existing[i].Stage] = existing[i];

            for (int i = 0; i < m_Components.Length; ++i)
            {
                if (m_Components[i] != null)
                {
                    if (CinemachineCore.sShowHiddenObjects)
                        m_Components[i].hideFlags &= ~HideFlags.HideInInspector;
                    else
                        m_Components[i].hideFlags |= HideFlags.HideInInspector;
                }
            }
            OnComponentCacheUpdated();
        }

        /// <summary>Notification that the component cache has just been update,
        /// in case a subclass needs to do something extra</summary>
        protected virtual void OnComponentCacheUpdated() {}

        /// <summary>Destroy all the CinmachineComponentBase components</summary>
        protected void DestroyComponents()
        {
            var existing = GetComponents<CinemachineComponentBase>();
            for (int i = 0; i < existing.Length; ++i)
            {
#if UNITY_EDITOR
                UnityEditor.Undo.DestroyObjectImmediate(existing[i]);
#else
                UnityEngine.Object.Destroy(existing[i]);
#endif
            }
            InvalidateComponentCache();
        }

#if UNITY_EDITOR
        // This gets called when user pastes component values
        void CopyComponents(CinemachineComponentBase[] copyFrom)
        {
            foreach (CinemachineComponentBase c in copyFrom)
            {
                if (c != null)
                {
                    Type type = c.GetType();
                    var copy = UnityEditor.Undo.AddComponent(gameObject, type);
                    UnityEditor.Undo.RecordObject(copy, "copying pipeline");

                    System.Reflection.BindingFlags bindingAttr
                        = System.Reflection.BindingFlags.Public
                        | System.Reflection.BindingFlags.NonPublic
                        | System.Reflection.BindingFlags.Instance;

                    System.Reflection.FieldInfo[] fields = type.GetFields(bindingAttr);
                    for (int i = 0; i < fields.Length; ++i)
                        if (!fields[i].IsStatic)
                            fields[i].SetValue(copy, fields[i].GetValue(c));
                }
            }
        }
#endif

        /// Legacy support for an old API.  GML todo: deprecate these methods

        /// <summary>Get the component set for a specific stage.</summary>
        /// <param name="stage">The stage for which we want the component</param>
        /// <returns>The Cinemachine component for that stage, or null if not defined</returns>
        public CinemachineComponentBase GetCinemachineComponent(CinemachineCore.Stage stage)
        {
            var cache = ComponentCache;
            var i = (int)stage;
            return i >= 0 && i < cache.Length ? cache[i] : null;
        }

        /// <summary>Get an existing component of a specific type from the cinemachine pipeline.</summary>
        public T GetCinemachineComponent<T>() where T : CinemachineComponentBase
        {
            var components = ComponentCache;
            foreach (var c in components)
                if (c is T)
                    return c as T;
            return null;
        }

        /// <summary>Add a component to the cinemachine pipeline.</summary>
        public T AddCinemachineComponent<T>() where T : CinemachineComponentBase
        {
            var components = ComponentCache;
            T c = gameObject.AddComponent<T>();
            var oldC = components[(int)c.Stage];
            if (oldC != null)
            {
                oldC.enabled = false;
                RuntimeUtility.DestroyObject(oldC);
            }
            InvalidateComponentCache();
            return c;
        }

        /// <summary>Remove a component from the cinemachine pipeline.</summary>
        public void DestroyCinemachineComponent<T>() where T : CinemachineComponentBase
        {
            var components = ComponentCache;
            foreach (var c in components)
            {
                if (c is T)
                {
                    c.enabled = false;
                    RuntimeUtility.DestroyObject(c);
                    InvalidateComponentCache();
                    return;
                }
            }
        }
    }
}
#endif

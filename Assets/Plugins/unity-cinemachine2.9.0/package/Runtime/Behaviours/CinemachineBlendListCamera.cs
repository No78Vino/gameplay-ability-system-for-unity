#if !UNITY_2019_3_OR_NEWER
#define CINEMACHINE_UNITY_IMGUI
#endif

using Cinemachine.Utility;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Cinemachine
{
    /// <summary>
    /// This is a virtual camera "manager" that owns and manages a collection
    /// of child Virtual Cameras.  When the camera goes live, these child vcams
    /// are enabled, one after another, holding each camera for a designated time.
    /// Blends between cameras are specified.
    /// The last camera is held indefinitely.
    /// </summary>
    [DocumentationSorting(DocumentationSortingAttribute.Level.UserRef)]
    [DisallowMultipleComponent]
    [ExecuteAlways]
    [ExcludeFromPreset]
    [AddComponentMenu("Cinemachine/CinemachineBlendListCamera")]
    [HelpURL(Documentation.BaseURL + "manual/CinemachineBlendListCamera.html")]
    public class CinemachineBlendListCamera : CinemachineVirtualCameraBase
    {
        /// <summary>Default object for the camera children to look at (the aim target), 
        /// if not specified in a child rig.  May be empty</summary>
        [Tooltip("Default object for the camera children to look at (the aim target), if not "
            + "specified in a child camera.  May be empty if all of the children define targets of their own.")]
        [NoSaveDuringPlay]
        [VcamTargetProperty]
        public Transform m_LookAt;

        /// <summary>Default object for the camera children wants to move with (the body target), 
        /// if not specified in a child rig.  May be empty</summary>
        [Tooltip("Default object for the camera children wants to move with (the body target), "
            + "if not specified in a child camera.  May be empty if all of the children define targets of their own.")]
        [NoSaveDuringPlay]
        [VcamTargetProperty]
        public Transform m_Follow;

        /// <summary>When enabled, the current camera and blend will be indicated in the game window, for debugging</summary>
        [Tooltip("When enabled, the current child camera and blend will be indicated in the game window, for debugging")]
        public bool m_ShowDebugText;

        /// <summary>When enabled, the child vcams will cycle indefinitely instead of just stopping at the last one</summary>
        [Tooltip("When enabled, the child vcams will cycle indefinitely instead of just stopping at the last one")]
        public bool m_Loop;

        /// <summary>Internal API for the editor.  Do not use this field</summary>
        [SerializeField][HideInInspector][NoSaveDuringPlay]
        internal CinemachineVirtualCameraBase[] m_ChildCameras;

        /// <summary>This represents a single entry in the instrunction list of the BlendListCamera.</summary>
        [Serializable]
        public struct Instruction
        {
            /// <summary>The virtual camera to activate when this instruction becomes active</summary>
            [Tooltip("The virtual camera to activate when this instruction becomes active")]
            public CinemachineVirtualCameraBase m_VirtualCamera;
            /// <summary>How long to wait (in seconds) before activating the next virtual camera in the list (if any)</summary>
            [Tooltip("How long to wait (in seconds) before activating the next virtual camera in the list (if any)")]
            public float m_Hold;
            /// <summary>How to blend to the next virtual camera in the list (if any)</summary>
            [CinemachineBlendDefinitionProperty]
            [Tooltip("How to blend to the next virtual camera in the list (if any)")]
            public CinemachineBlendDefinition m_Blend;
        };

        /// <summary>The set of instructions associating virtual cameras with states.
        /// The set of instructions for enabling child cameras</summary>
        [Tooltip("The set of instructions for enabling child cameras.")]
        public Instruction[] m_Instructions;

        /// <summary>Gets a brief debug description of this virtual camera, for use when displayiong debug info</summary>
        public override string Description
        {
            get
            {
                // Show the active camera and blend
                if (mActiveBlend != null)
                    return mActiveBlend.Description;

                ICinemachineCamera vcam = LiveChild;
                if (vcam == null)
                    return "(none)";
                var sb = CinemachineDebug.SBFromPool();
                sb.Append("["); sb.Append(vcam.Name); sb.Append("]");
                string text = sb.ToString();
                CinemachineDebug.ReturnToPool(sb);
                return text;
            }
        }

        void Reset()
        {
            m_LookAt = null;
            m_Follow = null;
            m_ShowDebugText = false;
            m_Loop = false;
            m_Instructions = null;
            m_ChildCameras = null;
        }
        
        /// <summary>Get the current "best" child virtual camera, that would be chosen
        /// if the State Driven Camera were active.</summary>
        public ICinemachineCamera LiveChild { get; set; }

        /// <summary>Check whether the vcam a live child of this camera.</summary>
        /// <param name="vcam">The Virtual Camera to check</param>
        /// <param name="dominantChildOnly">If truw, will only return true if this vcam is the dominat live child</param>
        /// <returns>True if the vcam is currently actively influencing the state of this vcam</returns>
        public override bool IsLiveChild(ICinemachineCamera vcam, bool dominantChildOnly = false)
        {
            return vcam == LiveChild || (mActiveBlend != null && mActiveBlend.Uses(vcam));
        }

        /// <summary>The State of the current live child</summary>
        public override CameraState State { get { return m_State; } }

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
            UpdateListOfChildren();
            foreach (var vcam in m_ChildCameras)
                vcam.OnTargetObjectWarped(target, positionDelta);
            base.OnTargetObjectWarped(target, positionDelta);
        }

        /// <summary>
        /// Force the virtual camera to assume a given position and orientation
        /// </summary>
        /// <param name="pos">Worldspace pposition to take</param>
        /// <param name="rot">Worldspace orientation to take</param>
        public override void ForceCameraPosition(Vector3 pos, Quaternion rot)
        {
            UpdateListOfChildren();
            foreach (var vcam in m_ChildCameras)
                vcam.ForceCameraPosition(pos, rot);
            base.ForceCameraPosition(pos, rot);
        }

        /// <summary>Notification that this virtual camera is going live.</summary>
        /// <param name="fromCam">The camera being deactivated.  May be null.</param>
        /// <param name="worldUp">Default world Up, set by the CinemachineBrain</param>
        /// <param name="deltaTime">Delta time for time-based effects (ignore if less than or equal to 0)</param>
        public override void OnTransitionFromCamera(
            ICinemachineCamera fromCam, Vector3 worldUp, float deltaTime)
        {
            base.OnTransitionFromCamera(fromCam, worldUp, deltaTime);
            InvokeOnTransitionInExtensions(fromCam, worldUp, deltaTime);
            mActivationTime = CinemachineCore.CurrentTime;
            mCurrentInstruction = 0;
            LiveChild = null;
            mActiveBlend = null;
            m_TransitioningFrom = fromCam;
            InternalUpdateCameraState(worldUp, deltaTime);
        }

        ICinemachineCamera m_TransitioningFrom;

        /// <summary>Called by CinemachineCore at designated update time
        /// so the vcam can position itself and track its targets.  This implementation
        /// updates all the children, chooses the best one, and implements any required blending.</summary>
        /// <param name="worldUp">Default world Up, set by the CinemachineBrain</param>
        /// <param name="deltaTime">Delta time for time-based effects (ignore if less than or equal to 0)</param>
        public override void InternalUpdateCameraState(Vector3 worldUp, float deltaTime)
        {
            if (!PreviousStateIsValid)
            {
                mCurrentInstruction = -1;
                mActiveBlend = null;
            }

            UpdateListOfChildren();
            AdvanceCurrentInstruction(deltaTime);

            CinemachineVirtualCameraBase best = null;
            if (mCurrentInstruction >= 0 && mCurrentInstruction < m_Instructions.Length)
                best = m_Instructions[mCurrentInstruction].m_VirtualCamera;

            if (best != null)
            {
                if (!best.gameObject.activeInHierarchy)
                {
                    best.gameObject.SetActive(true);
                    best.UpdateCameraState(worldUp, deltaTime);
                }
                ICinemachineCamera previousCam = LiveChild;
                LiveChild = best;

                // Are we transitioning cameras?
                if (previousCam != LiveChild && LiveChild != null)
                {
                    // Notify incoming camera of transition
                    LiveChild.OnTransitionFromCamera(previousCam, worldUp, deltaTime);

                    // Generate Camera Activation event in the brain if live
                    CinemachineCore.Instance.GenerateCameraActivationEvent(LiveChild, previousCam);

                    if (previousCam != null)
                    {
                        // Create a blend (will be null if a cut)
                        mActiveBlend = CreateBlend(
                                previousCam, LiveChild,
                                m_Instructions[mCurrentInstruction].m_Blend,
                                mActiveBlend);

                        // If cutting, generate a camera cut event if live
                        if (mActiveBlend == null || !mActiveBlend.Uses(previousCam))
                            CinemachineCore.Instance.GenerateCameraCutEvent(LiveChild);
                    }
                }
            }

            // Advance the current blend (if any)
            if (mActiveBlend != null)
            {
                mActiveBlend.TimeInBlend += (deltaTime >= 0) ? deltaTime : mActiveBlend.Duration;
                if (mActiveBlend.IsComplete)
                    mActiveBlend = null;
            }

            if (mActiveBlend != null)
            {
                mActiveBlend.UpdateCameraState(worldUp, deltaTime);
                m_State = mActiveBlend.State;
            }
            else if (LiveChild != null)
            {
                if (m_TransitioningFrom != null)
                    LiveChild.OnTransitionFromCamera(m_TransitioningFrom, worldUp, deltaTime);
                m_State =  LiveChild.State;
            }
            m_TransitioningFrom = null;
            InvokePostPipelineStageCallback(
                this, CinemachineCore.Stage.Finalize, ref m_State, deltaTime);
            PreviousStateIsValid = true;
        }

        /// <summary>Makes sure the internal child cache is up to date</summary>
        protected override void OnEnable()
        {
            base.OnEnable();
            InvalidateListOfChildren();
            LiveChild = null;
            mActiveBlend = null;
            CinemachineDebug.OnGUIHandlers -= OnGuiHandler;
            CinemachineDebug.OnGUIHandlers += OnGuiHandler;
        }

        /// <summary>
        /// Uninstall the GUI handler
        /// </summary>
        protected override void OnDisable()
        {
            base.OnDisable();
            CinemachineDebug.OnGUIHandlers -= OnGuiHandler;
        }

        /// <summary>Makes sure the internal child cache is up to date</summary>
        void OnTransformChildrenChanged()
        {
            InvalidateListOfChildren();
        }

        /// Will only be called if Unity Editor - never in build
        private void OnGuiHandler()
        {
#if CINEMACHINE_UNITY_IMGUI
            if (!m_ShowDebugText)
                CinemachineDebug.ReleaseScreenPos(this);
            else
            {
                var sb = CinemachineDebug.SBFromPool();
                sb.Append(Name); sb.Append(": "); sb.Append(Description);
                string text = sb.ToString();
                Rect r = CinemachineDebug.GetScreenPos(this, text, GUI.skin.box);
                GUI.Label(r, text, GUI.skin.box);
                CinemachineDebug.ReturnToPool(sb);
            }
#endif
        }

        CameraState m_State = CameraState.Default;

        /// <summary>The list of child cameras.  These are just the immediate children in the hierarchy.</summary>
        public CinemachineVirtualCameraBase[] ChildCameras { get { UpdateListOfChildren(); return m_ChildCameras; }}

        /// <summary>Is there a blend in progress?</summary>
        public bool IsBlending { get { return mActiveBlend != null; } }

        /// <summary>The time at which the current instruction went live</summary>
        float mActivationTime = -1;
        int mCurrentInstruction = 0;
        private CinemachineBlend mActiveBlend = null;

        void InvalidateListOfChildren() { m_ChildCameras = null; LiveChild = null; }

        void UpdateListOfChildren()
        {
            if (m_ChildCameras != null)
                return;
            List<CinemachineVirtualCameraBase> list = new List<CinemachineVirtualCameraBase>();
            CinemachineVirtualCameraBase[] kids = GetComponentsInChildren<CinemachineVirtualCameraBase>(true);
            foreach (CinemachineVirtualCameraBase k in kids)
                if (k.transform.parent == transform)
                    list.Add(k);
            m_ChildCameras = list.ToArray();
            ValidateInstructions();
        }

        /// <summary>Internal API for the inspector editor.</summary>
        /// // GML todo: make this private, part of UpdateListOfChildren()
        internal void ValidateInstructions()
        {
            if (m_Instructions == null)
                m_Instructions = new Instruction[0];
            for (int i = 0; i < m_Instructions.Length; ++i)
            {
                if (m_Instructions[i].m_VirtualCamera != null
                    && m_Instructions[i].m_VirtualCamera.transform.parent != transform)
                {
                    m_Instructions[i].m_VirtualCamera = null;
                }
            }
            mActiveBlend = null;
        }

        private void AdvanceCurrentInstruction(float deltaTime)
        {
            if (m_ChildCameras == null || m_ChildCameras.Length == 0
                || mActivationTime < 0 || m_Instructions.Length == 0)
            {
                mActivationTime = -1;
                mCurrentInstruction = -1;
                mActiveBlend = null;
                return;
            }

            float now = CinemachineCore.CurrentTime;
            if (mCurrentInstruction < 0 || deltaTime < 0)
            {
                mActivationTime = now;
                mCurrentInstruction = 0;
            }
            if (mCurrentInstruction > m_Instructions.Length - 1)
            {
                mActivationTime = now;
                mCurrentInstruction = m_Instructions.Length - 1;
            }

            var holdTime = m_Instructions[mCurrentInstruction].m_Hold 
                + m_Instructions[mCurrentInstruction].m_Blend.BlendTime;
            var minHold = mCurrentInstruction < m_Instructions.Length - 1 || m_Loop 
                ? 0 : float.MaxValue;
            if (now - mActivationTime > Mathf.Max(minHold, holdTime))
            {
                mActivationTime = now;
                ++mCurrentInstruction;
                if (m_Loop && mCurrentInstruction == m_Instructions.Length)
                    mCurrentInstruction = 0;
            }
        }
    }
}

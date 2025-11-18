#if !UNITY_2019_3_OR_NEWER
#define CINEMACHINE_UNITY_ANIMATION
#endif

using Cinemachine.Utility;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Cinemachine
{
#if CINEMACHINE_UNITY_ANIMATION
    /// <summary>
    /// This is a virtual camera "manager" that owns and manages a collection
    /// of child Virtual Cameras.  These child vcams are mapped to individual states in
    /// an animation state machine, allowing you to associate specific vcams to specific
    /// animation states.  When that state is active in the state machine, then the
    /// associated camera will be activated.
    ///
    /// You can define custom blends and transitions between child cameras.
    ///
    /// In order to use this behaviour, you must have an animated target (i.e. an object
    /// animated with a state machine) to drive the behaviour.
    /// </summary>
    [DocumentationSorting(DocumentationSortingAttribute.Level.UserRef)]
    [DisallowMultipleComponent]
    [ExecuteAlways]
    [ExcludeFromPreset]
    [AddComponentMenu("Cinemachine/CinemachineStateDrivenCamera")]
    [HelpURL(Documentation.BaseURL + "manual/CinemachineStateDrivenCamera.html")]
    public class CinemachineStateDrivenCamera : CinemachineVirtualCameraBase
    {
        /// <summary>Default object for the camera children to look at (the aim target), 
        /// if not specified in a child rig.  May be empty</summary>
        [Tooltip("Default object for the camera children to look at (the aim target), "
            + "if not specified in a child camera.  May be empty if all of the children "
            + "define targets of their own.")]
        [NoSaveDuringPlay]
        [VcamTargetProperty]
        public Transform m_LookAt = null;

        /// <summary>Default object for the camera children wants to move with (the body target), 
        /// if not specified in a child rig.  May be empty</summary>
        [Tooltip("Default object for the camera children wants to move with (the body target), "
            + "if not specified in a child camera.  May be empty if all of the children "
            + "define targets of their own.")]
        [NoSaveDuringPlay]
        [VcamTargetProperty]
        public Transform m_Follow = null;

        /// <summary>The state machine whose state changes will drive this camera's choice of active child</summary>
        [Space]
        [Tooltip("The state machine whose state changes will drive this camera's choice of active child")]
        [NoSaveDuringPlay]
        public Animator m_AnimatedTarget;

        /// <summary>Which layer in the target FSM to observe</summary>
        [Tooltip("Which layer in the target state machine to observe")]
        [NoSaveDuringPlay]
        public int m_LayerIndex;

        /// <summary>When enabled, the current camera and blend will be indicated in 
        /// the game window, for debugging</summary>
        [Tooltip("When enabled, the current child camera and blend will be indicated in "
            + "the game window, for debugging")]
        public bool m_ShowDebugText = false;

        /// <summary>Internal API for the editor.  Do not use this field</summary>
        [SerializeField][HideInInspector][NoSaveDuringPlay]
        internal CinemachineVirtualCameraBase[] m_ChildCameras = null;

        /// <summary>This represents a single instrunction to the StateDrivenCamera.  It associates
        /// an state from the state machine with a child Virtual Camera, and also holds
        /// activation tuning parameters.</summary>
        [Serializable]
        public struct Instruction
        {
            /// <summary>The full hash of the animation state</summary>
            [Tooltip("The full hash of the animation state")]
            public int m_FullHash;
            /// <summary>The virtual camera to activate when the animation state becomes active</summary>
            [Tooltip("The virtual camera to activate when the animation state becomes active")]
            public CinemachineVirtualCameraBase m_VirtualCamera;
            /// <summary>How long to wait (in seconds) before activating the virtual camera.
            /// This filters out very short state durations</summary>
            [Tooltip("How long to wait (in seconds) before activating the virtual camera. "
                + "This filters out very short state durations")]
            public float m_ActivateAfter;
            /// <summary>The minimum length of time (in seconds) to keep a virtual camera active</summary>
            [Tooltip("The minimum length of time (in seconds) to keep a virtual camera active")]
            public float m_MinDuration;
        };

        /// <summary>The set of instructions associating virtual cameras with states.
        /// These instructions are used to choose the live child at any given moment</summary>
        [Tooltip("The set of instructions associating virtual cameras with states.  "
        + "These instructions are used to choose the live child at any given moment")]
        public Instruction[] m_Instructions;

        /// <summary>
        /// The blend which is used if you don't explicitly define a blend between two Virtual Camera children.
        /// </summary>
        [CinemachineBlendDefinitionProperty]
        [Tooltip("The blend which is used if you don't explicitly define a blend between two Virtual Camera children")]
        public CinemachineBlendDefinition m_DefaultBlend
            = new CinemachineBlendDefinition(CinemachineBlendDefinition.Style.EaseInOut, 0.5f);

        /// <summary>
        /// This is the asset which contains custom settings for specific child blends.
        /// </summary>
        [Tooltip("This is the asset which contains custom settings for specific child blends")]
        public CinemachineBlenderSettings m_CustomBlends = null;

        /// <summary>Internal API for the Inspector editor.  This implements nested states.</summary>
        [Serializable]
        [DocumentationSorting(DocumentationSortingAttribute.Level.Undoc)]
        internal struct ParentHash
        {
            /// <summary>Internal API for the Inspector editor</summary>
            public int m_Hash;
            /// <summary>Internal API for the Inspector editor</summary>
            public int m_ParentHash;
            /// <summary>Internal API for the Inspector editor</summary>
            public ParentHash(int h, int p) { m_Hash = h; m_ParentHash = p; }
        }
        /// <summary>Internal API for the Inspector editor</summary>
        [HideInInspector][SerializeField] internal ParentHash[] m_ParentHash = null;

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

        /// <summary>Get the current "best" child virtual camera, that would be chosen
        /// if the State Driven Camera were active.</summary>
        public ICinemachineCamera LiveChild { get; set; }

        /// <summary>Check whether the vcam a live child of this camera.</summary>
        /// <param name="vcam">The Virtual Camera to check</param>
        /// <param name="dominantChildOnly">If true, will only return true if this vcam is the dominat live child</param>
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
        /// also warp seamlessly.</summary>
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
            m_TransitioningFrom  = fromCam;
            InternalUpdateCameraState(worldUp, deltaTime);
        }

        ICinemachineCamera m_TransitioningFrom;

        /// <summary>Internal use only.  Do not call this method.
        /// Called by CinemachineCore at designated update time
        /// so the vcam can position itself and track its targets.  This implementation
        /// updates all the children, chooses the best one, and implements any required blending.</summary>
        /// <param name="worldUp">Default world Up, set by the CinemachineBrain</param>
        /// <param name="deltaTime">Delta time for time-based effects (ignore if less than or equal to 0)</param>
        public override void InternalUpdateCameraState(Vector3 worldUp, float deltaTime)
        {
            UpdateListOfChildren();
            CinemachineVirtualCameraBase best = ChooseCurrentCamera();
            if (best != null && !best.gameObject.activeInHierarchy)
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
                            LookupBlend(previousCam, LiveChild), mActiveBlend);

                    // If cutting, generate a camera cut event if live
                    if (mActiveBlend == null || !mActiveBlend.Uses(previousCam))
                        CinemachineCore.Instance.GenerateCameraCutEvent(LiveChild);
                }
            }

            // Advance the current blend (if any)
            if (mActiveBlend != null)
            {
                mActiveBlend.TimeInBlend += (deltaTime >= 0)
                    ? deltaTime : mActiveBlend.Duration;
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
                if (m_TransitioningFrom  != null)
                    LiveChild.OnTransitionFromCamera(m_TransitioningFrom , worldUp, deltaTime);
                m_State =  LiveChild.State;
            }
            m_TransitioningFrom  = null;
            InvokePostPipelineStageCallback(this, CinemachineCore.Stage.Finalize, ref m_State, deltaTime);
            PreviousStateIsValid = true;
        }

        /// <summary>Makes sure the internal child cache is up to date</summary>
        protected override void OnEnable()
        {
            base.OnEnable();
            InvalidateListOfChildren();
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
        public void OnTransformChildrenChanged()
        {
            InvalidateListOfChildren();
        }

        ///  Will only be called if Unity Editor - never in build
        private void OnGuiHandler()
        {
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
        }

        CameraState m_State = CameraState.Default;

        /// <summary>The list of child cameras.  These are just the immediate children in the hierarchy.</summary>
        public CinemachineVirtualCameraBase[] ChildCameras { get { UpdateListOfChildren(); return m_ChildCameras; }}

        /// <summary>Is there a blend in progress?</summary>
        public bool IsBlending => mActiveBlend != null;

        /// <summary>
        /// Get the current active blend in progress.  Will return null if no blend is in progress.
        /// </summary>
        public CinemachineBlend ActiveBlend => mActiveBlend;

        /// <summary>API for the inspector editor.  Animation module does not have hashes
        /// for state parents, so we have to invent them in order to implement nested state
        /// handling</summary>
        /// <param name="parentHash">Parent state's hash</param>
        /// <param name="clip">The clip to create the fake hash for</param>
        /// <returns>The fake hash</returns>
        public static int CreateFakeHash(int parentHash, AnimationClip clip)
        {
            return Animator.StringToHash(parentHash.ToString() + "_" + clip.name);
        }

        // Avoid garbage string manipulations at runtime
        struct HashPair { public int parentHash; public int hash; }
        Dictionary<AnimationClip, List<HashPair>> mHashCache;
        int LookupFakeHash(int parentHash, AnimationClip clip)
        {
            if (mHashCache == null)
                mHashCache = new Dictionary<AnimationClip, List<HashPair>>();
            List<HashPair> list = null;
            if (!mHashCache.TryGetValue(clip, out list))
            {
                list = new List<HashPair>();
                mHashCache[clip] = list;
            }
            for (int i = 0; i < list.Count; ++i)
                if (list[i].parentHash == parentHash)
                    return list[i].hash;
            int newHash = CreateFakeHash(parentHash, clip);
            list.Add(new HashPair() { parentHash = parentHash, hash = newHash });
            mStateParentLookup[newHash] = parentHash;
            return newHash;
        }


        float mActivationTime = 0;
        Instruction mActiveInstruction;
        float mPendingActivationTime = 0;
        Instruction mPendingInstruction;
        private CinemachineBlend mActiveBlend = null;

        void InvalidateListOfChildren() { m_ChildCameras = null; LiveChild = null; }

        void UpdateListOfChildren()
        {
            if (m_ChildCameras != null && mInstructionDictionary != null && mStateParentLookup != null)
                return;
            List<CinemachineVirtualCameraBase> list = new List<CinemachineVirtualCameraBase>();
            CinemachineVirtualCameraBase[] kids = GetComponentsInChildren<CinemachineVirtualCameraBase>(true);
            foreach (CinemachineVirtualCameraBase k in kids)
                if (k.transform.parent == transform)
                    list.Add(k);
            m_ChildCameras = list.ToArray();
            ValidateInstructions();
        }

        private Dictionary<int, int> mInstructionDictionary;
        private Dictionary<int, int> mStateParentLookup;
        /// <summary>Internal API for the inspector editor.</summary>
        internal void ValidateInstructions()
        {
            if (m_Instructions == null)
                m_Instructions = new Instruction[0];
            mInstructionDictionary = new Dictionary<int, int>();
            for (int i = 0; i < m_Instructions.Length; ++i)
            {
                if (m_Instructions[i].m_VirtualCamera != null
                    && m_Instructions[i].m_VirtualCamera.transform.parent != transform)
                {
                    m_Instructions[i].m_VirtualCamera = null;
                }
                mInstructionDictionary[m_Instructions[i].m_FullHash] = i;
            }

            // Create the parent lookup
            mStateParentLookup = new Dictionary<int, int>();
            if (m_ParentHash != null)
                foreach (var i in m_ParentHash)
                    mStateParentLookup[i.m_Hash] = i.m_ParentHash;

            // Zap the cached current instructions
            mHashCache = null;
            mActivationTime = mPendingActivationTime = 0;
            mActiveBlend = null;
        }

        List<AnimatorClipInfo>  m_clipInfoList = new List<AnimatorClipInfo>();
        private CinemachineVirtualCameraBase ChooseCurrentCamera()
        {
            if (m_ChildCameras == null || m_ChildCameras.Length == 0)
            {
                mActivationTime = 0;
                return null;
            }
            CinemachineVirtualCameraBase defaultCam = m_ChildCameras[0];
            if (m_AnimatedTarget == null || !m_AnimatedTarget.gameObject.activeSelf
                || m_AnimatedTarget.runtimeAnimatorController == null
                || m_LayerIndex < 0 || !m_AnimatedTarget.hasBoundPlayables
                || m_LayerIndex >= m_AnimatedTarget.layerCount)
            {
                mActivationTime = 0;
                return defaultCam;
            }

            // Get the current state
            int hash;
            if (m_AnimatedTarget.IsInTransition(m_LayerIndex))
            {
                // Force "current" state to be the state we're transitioning to
                AnimatorStateInfo info = m_AnimatedTarget.GetNextAnimatorStateInfo(m_LayerIndex);
                m_AnimatedTarget.GetNextAnimatorClipInfo(m_LayerIndex, m_clipInfoList);
                hash = GetClipHash(info.fullPathHash, m_clipInfoList);
            }
            else
            {
                AnimatorStateInfo info = m_AnimatedTarget.GetCurrentAnimatorStateInfo(m_LayerIndex);
                m_AnimatedTarget.GetCurrentAnimatorClipInfo(m_LayerIndex, m_clipInfoList);
                hash = GetClipHash(info.fullPathHash, m_clipInfoList);
            }

            // If we don't have an instruction for this state, find a suitable default
            while (hash != 0 && !mInstructionDictionary.ContainsKey(hash))
                hash = mStateParentLookup.ContainsKey(hash) ? mStateParentLookup[hash] : 0;

            float now = CinemachineCore.CurrentTime;
            if (mActivationTime != 0)
            {
                // Is it active now?
                if (mActiveInstruction.m_FullHash == hash)
                {
                    // Yes, cancel any pending
                    mPendingActivationTime = 0;
                    return mActiveInstruction.m_VirtualCamera;
                }

                // Is it pending?
                if (PreviousStateIsValid)
                {
                    if (mPendingActivationTime != 0 && mPendingInstruction.m_FullHash == hash)
                    {
                        // Has it been pending long enough, and are we allowed to switch away
                        // from the active action?
                        if ((now - mPendingActivationTime) > mPendingInstruction.m_ActivateAfter
                            && ((now - mActivationTime) > mActiveInstruction.m_MinDuration
                                || mPendingInstruction.m_VirtualCamera.Priority
                                > mActiveInstruction.m_VirtualCamera.Priority))
                        {
                            // Yes, activate it now
                            mActiveInstruction = mPendingInstruction;
                            mActivationTime = now;
                            mPendingActivationTime = 0;
                        }
                        return mActiveInstruction.m_VirtualCamera;
                    }
                }
            }
            // Neither active nor pending.
            mPendingActivationTime = 0; // cancel the pending, if any

            if (!mInstructionDictionary.ContainsKey(hash))
            {
                // No defaults set, we just ignore this state
                if (mActivationTime != 0)
                    return mActiveInstruction.m_VirtualCamera;
                return defaultCam;
            }

            // Can we activate it now?
            Instruction newInstr = m_Instructions[mInstructionDictionary[hash]];
            if (newInstr.m_VirtualCamera == null)
                newInstr.m_VirtualCamera = defaultCam;
            if (PreviousStateIsValid && mActivationTime > 0)
            {
                if (newInstr.m_ActivateAfter > 0
                    || ((now - mActivationTime) < mActiveInstruction.m_MinDuration
                        && newInstr.m_VirtualCamera.Priority
                        <= mActiveInstruction.m_VirtualCamera.Priority))
                {
                    // Too early - make it pending
                    mPendingInstruction = newInstr;
                    mPendingActivationTime = now;
                    if (mActivationTime != 0)
                        return mActiveInstruction.m_VirtualCamera;
                    return defaultCam;
                }
            }
            // Activate now
            mActiveInstruction = newInstr;
            mActivationTime = now;
            return mActiveInstruction.m_VirtualCamera;
        }

        int GetClipHash(int hash, List<AnimatorClipInfo> clips)
        {
            // Find the strongest-weighted animation clip substate
            int bestClip = -1;
            for (int i = 0; i < clips.Count; ++i)
                if (bestClip < 0 || clips[i].weight > clips[bestClip].weight)
                    bestClip = i;

            // Use its hash
            if (bestClip >= 0 && clips[bestClip].weight > 0)
                hash = LookupFakeHash(hash, clips[bestClip].clip);

            return hash;
        }

        private CinemachineBlendDefinition LookupBlend(
            ICinemachineCamera fromKey, ICinemachineCamera toKey)
        {
            // Get the blend curve that's most appropriate for these cameras
            CinemachineBlendDefinition blend = m_DefaultBlend;
            if (m_CustomBlends != null)
            {
                string fromCameraName = (fromKey != null) ? fromKey.Name : string.Empty;
                string toCameraName = (toKey != null) ? toKey.Name : string.Empty;
                blend = m_CustomBlends.GetBlendForVirtualCameras(
                        fromCameraName, toCameraName, blend);
            }
            if (CinemachineCore.GetBlendOverride != null)
                blend = CinemachineCore.GetBlendOverride(fromKey, toKey, blend, this);
            return blend;
        }
    }
#endif
}
